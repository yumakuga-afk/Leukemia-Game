using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.InputSystem;


public class TaskUI : MonoBehaviour
{
    // Start is called before the first frame update

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

    private bool completedExpanded;
    private bool refreshPending;

    [SerializeField]
    private InputActionReference openTask;



    void Start()
    {
        //Get all Accepted task
        lastDisplayTask = TaskManager.Instance.GetTasksByState(TaskState.Accepted);
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }



    private void BuildRows(TaskManager manager)
    {   
        List<TaskDefinition> temp = manager.GetTasksByState(TaskState.Accepted);
        if(lastDisplayTask != null)
        {
            bool x = temp.Select(x =>x.Id).OrderBy(x=>x).SequenceEqual(lastDisplayTask.Select(x =>x.Id).OrderBy(x=>x));
            if(x)
                return;
        }
      

        foreach (var task in temp)
            AddTaskRow(task);
        

        // This for All Finish Task
        // var completed = manager.GetTasksByState(TaskState.Completed);
        // if (completed.Count == 0)
        //     return;

        // var toggle = AddRow(completedExpanded ? "Unfinish" : "Finish");

        // //bindSelection event
        // BindSelection(toggle, () => ShowDetails(null));

        // toggle.onClick.AddListener(() =>
        // {
        //     completedExpanded = !completedExpanded;
        //     refreshPending = true;
        // });

        // if (completedExpanded)
        // {
        //     foreach (var task in completed)
        //         AddTaskRow(task);
        // }
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
