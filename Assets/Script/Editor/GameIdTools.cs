// ============ Editor/GameIdTools.cs ============
// 整套 GUID 工具，放在 Editor 文件夹下 = Unity 只在编辑器编译，不进游戏包。
// ① 底层工具  ② 自定义 Inspector  ③ 顶部菜单  ④ 自动回填
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

// ① 底层工具：读 .meta GUID、写回资产的 id 字段
public static class ScriptableGuidObjectUtil
{
    public static string GetAssetGuid(ScriptableGuidObject so)
    {
        string path = AssetDatabase.GetAssetPath(so);
        return string.IsNullOrEmpty(path) ? null : AssetDatabase.AssetPathToGUID(path);
    }

    /// <summary>id := 资产 GUID（幂等）。通过 SerializedObject 写私有字段。返回是否发生改变。</summary>
    public static bool SyncId(ScriptableGuidObject so)
    {
        string guid = GetAssetGuid(so);
        if (string.IsNullOrEmpty(guid)) return false;

        var obj = new SerializedObject(so);
        var prop = obj.FindProperty("id");
        if (prop.stringValue == guid) return false;

        prop.stringValue = guid;
        obj.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(so);
        return true;
    }

    public static List<ScriptableGuidObject> FindAll()
    {
        var all = new List<ScriptableGuidObject>();
        foreach (var t in TypeCache.GetTypesDerivedFrom<ScriptableGuidObject>())
            foreach (var guid in AssetDatabase.FindAssets("t:" + t.Name))
            {
                var so = AssetDatabase.LoadAssetAtPath<ScriptableGuidObject>(
                    AssetDatabase.GUIDToAssetPath(guid));
                if (so != null) all.Add(so);
            }
        return all;
    }
}

// ② 自定义 Inspector：把「手填 ID 文本框」换成「只读 GUID + 生成/修复按钮」
[CustomEditor(typeof(ScriptableGuidObject), editorForChildClasses: true)]
public class GuidDefinitionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var so = (ScriptableGuidObject)target;
        string guid = ScriptableGuidObjectUtil.GetAssetGuid(so);

        EditorGUILayout.LabelField("GUID (ReadOnly, Auto)", so.HasId ? so.Id : "<empty>");

        if (!so.HasId)
        {
            EditorGUILayout.HelpBox("尚未生成 GUID，点击写入资产 GUID。", MessageType.Warning);
            if (GUILayout.Button("Generate ID") && ScriptableGuidObjectUtil.SyncId(so))
                AssetDatabase.SaveAssets();
        }
        else if (guid != null && so.Id != guid)
        {
            EditorGUILayout.HelpBox("GUID 与资产 .meta GUID 不一致（复制/漂移），请修复。", MessageType.Error);
            if (GUILayout.Button("Fix ID") && ScriptableGuidObjectUtil.SyncId(so))
                AssetDatabase.SaveAssets();
        }

        EditorGUILayout.Space();
        DrawDefaultInspector(); // HideInInspector 的 id 不会被画出
    }
}

// ③ 顶部菜单：批量生成 / 校验 / 导出对照表
public static class GameIdMenu
{
    [MenuItem("Game/Regenerate All IDs")]
    public static void RegenerateAll()
    {
        int changed = 0;
        foreach (var so in ScriptableGuidObjectUtil.FindAll())
            if (ScriptableGuidObjectUtil.SyncId(so)) changed++;
        if (changed > 0) AssetDatabase.SaveAssets();
        Debug.Log($"[GameId] Regenerate 完成，修复 {changed} 个资产。");
    }

    [MenuItem("Game/Validate IDs")]
    public static void Validate()
    {
        var byId = new Dictionary<string, List<ScriptableGuidObject>>();
        int issues = 0;

        foreach (var so in ScriptableGuidObjectUtil.FindAll())
        {
            string guid = ScriptableGuidObjectUtil.GetAssetGuid(so);
            if (!so.HasId) { Debug.LogWarning($"缺失 GUID: {so.name}", so); issues++; }
            else if (guid != null && so.Id != guid) { Debug.LogWarning($"漂移/复制: {so.name}", so); issues++; }

            if (so.HasId)
            {
                if (!byId.TryGetValue(so.Id, out var list)) byId[so.Id] = list = new List<ScriptableGuidObject>();
                list.Add(so);
            }

            if (so is TaskDefinition task && task.changesMap && string.IsNullOrEmpty(task.mapFlagKey))
            {
                Debug.LogWarning($"changesMap 为 true 但 mapFlagKey 为空: {so.name}", so);
                issues++;
            }
        }

        foreach (var kv in byId)
            if (kv.Value.Count > 1)
            {
                issues++;
                Debug.LogWarning($"重复 GUID {kv.Key}: {string.Join(", ", kv.Value.ConvertAll(x => x.name))}");
            }

        Debug.Log(issues == 0 ? "[GameId] 校验通过：0 个问题。" : $"[GameId] 发现 {issues} 个问题（见上方 Warning）。");
    }

    [MenuItem("Game/Export ID Reference")]
    public static void ExportReference()
    {
        string dir = "Assets/GameData";
        Directory.CreateDirectory(dir);

        var sb = new StringBuilder("# 名字 → GUID 对照表\n\n");
        foreach (var so in ScriptableGuidObjectUtil.FindAll())
            sb.AppendLine($"- {so.name}  `{so.Id}`");

        string path = Path.Combine(dir, "id-reference.md");
        File.WriteAllText(path, sb.ToString());
        AssetDatabase.Refresh();
        Debug.Log($"[GameId] 已导出 {path}");
    }
}

// ④（可选）新建/导入资产时自动回填空 GUID，延迟到 import 之后执行
class GameIdAutoImporter : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
    {
        var needs = new List<ScriptableGuidObject>();
        foreach (var p in imported)
        {
            var so = AssetDatabase.LoadAssetAtPath<ScriptableGuidObject>(p);
            if (so != null && !so.HasId) needs.Add(so);
        }
        if (needs.Count == 0) return;

        EditorApplication.delayCall += () =>
        {
            int changed = 0;
            foreach (var so in needs)
                if (so != null && ScriptableGuidObjectUtil.SyncId(so)) changed++;
            if (changed > 0) AssetDatabase.SaveAssets();
        };
    }
}
