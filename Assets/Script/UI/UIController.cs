
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 负责打开/关闭 TaskUI 与 NoteUI（NoteUI 暂为占位）。
/// 输入沿用 InputActionReference，参照 PlayerInteraction / DialogueUI 的写法。
/// </summary>
public class UIController : MonoBehaviour
{

    //Got Prefab get TaskUICanva
    [SerializeField] private TaskUI taskUI;
    [SerializeField] private GameObject noteRoot; // NoteUI 占位，本次留空

    [SerializeField] private InputActionReference taskToggle;
    [SerializeField] private InputActionReference noteToggle;

    private void OnEnable()
    {
        if (taskToggle != null)
        {
            taskToggle.action.Enable();
            taskToggle.action.performed += OnTaskToggle;
        }

        if (noteToggle != null)
        {
            noteToggle.action.Enable();
            noteToggle.action.performed += OnNoteToggle;
        }
    }

    private void OnDisable()
    {
        if (taskToggle != null)
        {
            taskToggle.action.performed -= OnTaskToggle;
            taskToggle.action.Disable();
        }

        if (noteToggle != null)
        {
            noteToggle.action.performed -= OnNoteToggle;
            noteToggle.action.Disable();
        }
    }

    private void OnTaskToggle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        bool willOpen = taskUI != null && !taskUI.IsVisible;
        CloseAll();
        if (willOpen)
            taskUI.SetVisible(true);
    }

    private void OnNoteToggle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // NoteUI 未实现，占位。
        if (noteRoot == null)
        {
            Debug.Log("[UIController] NoteUI 尚未实现（占位）");
            return;
        }

        bool willOpen = !noteRoot.activeSelf;
        CloseAll();
        if (willOpen)
            noteRoot.SetActive(true);
    }

    private void CloseAll()
    {
        if (taskUI != null)
            taskUI.SetVisible(false);
        if (noteRoot != null)
            noteRoot.SetActive(false);
    }
}
