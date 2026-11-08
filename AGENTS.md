# AGENTS.md

本文件给 Claude Code / Cursor 等 AI 编码工具提供项目约定。遵守以下规则，避免引入反模式。

## 工作范围红线

- 只改 `Assets/Script`（新系统）。
- `Assets/ScriptsOld` 是废弃的旧系统：不要改动、不要参考它的写法。
- `Assets/Ink/Editor`、`Assets/Ink/InkLibs`、`Assets/Ink/Demos`、`Assets/Ink/Documentation` 是 Ink 集成包本体，不要改。
- 游戏内容数据（Task/Item/Note/Interactable/Condition 定义）在 `Assets/GameData/`。

## ID 铁律（最重要）

- 所有内容定义（`TaskDefinition`/`ItemDefinition`/`InteractableDefinition`/`NoteDefinition`）继承自 `ScriptableGuidObject`，其身份 ID 就是资产自身的 `.meta` GUID：
  `ScriptableGuidObject.id == AssetDatabase.AssetPathToGUID(assetPath)`。
- **永不手填、永不手改 ID**。ID 由编辑器工具写入，运行时只读 `Id` 属性。
- 新增内容后必须运行菜单 `Game/Regenerate All IDs` 与 `Game/Validate IDs`（定义在 `Assets/Script/Editor/GameIdTools.cs`）。
- 需要在 Ink 里引用某个实体时，用 `Game/Export ID Reference` 导出 `Assets/GameData/id-reference.md`，从中查 GUID，不要凭记忆手打。

## 系统架构

- `TaskDefinition`（任务定义）→ `TaskManager`（任务状态机：NotStarted/Accepted/Completed，条件满足自动完成）→ `GameEvents`（静态事件总线）。
- `NotebookSystem` 监听事件、按 `NoteCondition`（`ItemHeldCondition`/`InteractedCondition`/`TaskStateCondition`）解锁笔记。
- `WorldState` 监听事件、生成字符串 flag（`item_`/`task_accepted_`/`task_completed_`/`interacted_`，值用 GUID），是给 Ink 查世界状态的接口。
- `InkStateBinder` 是 **C# ↔ Ink 的唯一边界**：把任务状态写入 Ink 变量 `task_id/task_state/has_required_items/has_required_objects`。

## 如何新增一个内容项

1. 用 CreateAssetMenu 建资产（命名约定 `Task_X`/`Item_X`/`Note_X`/`Int_X`/`Cond_X`）。
2. 填字段。
3. 跑 `Game/Regenerate All IDs` + `Game/Validate IDs`。
4. 若需 Ink 引用，查 `id-reference.md` 拿 GUID。

## 约定

- 注释中英混杂，以中文为主。
- 单例：`public static X Instance` + `Awake` 赋值。
- 事件用静态 `GameEvents` 总线，不要到处直接调用其它系统。
- 编辑器专用代码放 `Assets/Script/Editor/`（Unity 会自动只在编辑器编译），**不要**在运行时脚本里写 `#if UNITY_EDITOR`。

## 禁止清单

- 禁止手写 / 复制 ID 字符串（`.ink` 里也不得硬编码 GUID，引用对照表）。
- 禁止在 `OnValidate` 里做依赖 `AssetDatabase` 的事（import 期不可用、play mode 也会跑）。
