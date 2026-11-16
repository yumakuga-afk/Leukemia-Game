using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;


public class TaskUI : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;

    [SerializeField]
    private RectTransform taskContent;
    [SerializeField] 
    private Button taskButtonPrefab;

    [Header("Description")]
    [SerializeField]
    private TMP_Text title;
    [SerializeField]

    private TMP_Text description;
    [SerializeField]

    private TMP_Text target;

    private List<TaskDefinition> lastDisplayTask;
    private readonly List<Button> rows = new();

    public bool IsVisible => panel != null && panel.activeSelf;



    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    void OnEnable()
    {
        GameEvents.TaskAccepted += OnTaskChanged;
        GameEvents.TaskCompleted += OnTaskChanged;
    }

    void OnDisable()
    {
        GameEvents.TaskAccepted -= OnTaskChanged;
        GameEvents.TaskCompleted -= OnTaskChanged;
    }

    public void SetVisible(bool visible)
    {
        if (panel != null)
            panel.SetActive(visible);

        if (visible)
            Refresh();
    }

    private void OnTaskChanged(TaskDefinition task) => Refresh();

    // Rebuild the list of accepted tasks; skip if the list remains unchanged.
    public void Refresh()
    {
        if (TaskManager.Instance == null)
            return;

        List<TaskDefinition> temp = TaskManager.Instance.GetTasksByState(TaskState.Accepted);

        if (lastDisplayTask != null)
        {
            bool same = temp.Select(x => x.Id).OrderBy(x => x)
                .SequenceEqual(lastDisplayTask.Select(x => x.Id).OrderBy(x => x));
            if (same)
                return;
        }

        ClearRows();

        foreach (var task in temp)
            AddTaskRow(task);

        lastDisplayTask = temp;

        if (rows.Count > 0)
            EventSystem.current?.SetSelectedGameObject(rows[0].gameObject);
    }

    private void ClearRows()
    {
        foreach (var row in rows)
            if (row != null)
                Destroy(row.gameObject);
        rows.Clear();
    }
    

    //Add Task Botton to UI
    private Button AddRow(string label)
    {
        var button = Instantiate(taskButtonPrefab, taskContent);
        button.GetComponentInChildren<TMP_Text>().text = label;
        rows.Add(button);
        return button;
    }

    //Add Task
    private void AddTaskRow(TaskDefinition task)
    {
        var button = AddRow(GetTaskTitle(task));
        //Bind Display Detail
        BindSelection(button, () => ShowDetails(task));
    }

    // 键盘或手柄移到按钮时，Select 事件立即更新右侧。
    private static void BindSelection(Button button, UnityEngine.Events.UnityAction show)
    {
        //Add EventTrigger to object, 
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        //Make Event for EventRigger, this mean select event
        var entry = new EventTrigger.Entry { eventID = EventTriggerType.Select };

        //Add show to this event
        entry.callback.AddListener(x => show());
        //Add event to EventTrigger
        trigger.triggers.Add(entry);

        button.onClick.AddListener(show);
    }

    private void ShowDetails(TaskDefinition task)
    {
        title.text = task == null ? "" : GetTaskTitle(task);
        description.text = task?.description ?? "";
        target.text = task?.target ?? "";
    }

    private static string GetTaskTitle(TaskDefinition task)
    {
        //if not null sent back title
        if (!string.IsNullOrWhiteSpace(task.title))
            return task.title;

        //
        return task.Id;
    }



    

}
