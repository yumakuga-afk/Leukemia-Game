using UnityEngine;

/// <summary>
/// 带稳定 GUID 身份的 ScriptableObject 基类。
/// id 恒等于资产 .meta GUID，由编辑器工具（Assets/Script/Editor/GameIdTools.cs）写入，运行时只读。
/// </summary>
public abstract class ScriptableGuidObject : ScriptableObject
{
    [SerializeField, HideInInspector] private string id;

    public string Id => id;
    public bool HasId => !string.IsNullOrEmpty(id);
}
