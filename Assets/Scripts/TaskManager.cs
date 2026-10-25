using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    /*
     * To add a task, call TaskManager.Instance.AddTask("Task Name", "Task Description");
     * To remove a task, call TaskManager.Instance.RemoveTask("Task Name");
     */
    public static TaskManager Instance { get; private set; }

    [SerializeField] private List<TaskBar> tasks = new List<TaskBar>();

    public List<TaskBar> Tasks => tasks;
    public int Count => tasks.Count;
    [SerializeField] private TMP_Text taskText;
    [SerializeField] private GameObject minibook;
    [SerializeField] private GameObject mainBook;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
       AddTask("Welcome", "Welcome to the game! Complete tasks to progress.");
       AddTask("Test Two", "We LOVE CHILIES!");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public bool AddTask(string name, string description = "")
    {
        tasks.Add(new TaskBar(name, description));
        Debug.Log($"TaskManager: Added task '{name}'.");
        updateTask();
        return true;
    }

    public bool RemoveTask(string name)
    {
        int removed = tasks.RemoveAll(t => t.Name == name);
        if (removed == 0)
        {
            Debug.LogWarning($"TaskManager: No task named '{name}' to remove.");
            return false;
        }

        //Debug.Log($"TaskManager: Removed task '{name}'.");
        updateTask();
        return true;
    }
    private void updateTask()
    {
        var lines = new List<string>();

        foreach (var t in tasks)
        {
            if (string.IsNullOrEmpty(t.Description))
                lines.Add(t.Name);
            else
                lines.Add($"{t.Name}: {t.Description}");
        }

        taskText.text = string.Join("\n ", lines);
    }
    public TaskBar GetTask(string name) => tasks.Find(t => t.Name == name);
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            minibook.SetActive(!minibook.activeSelf);
            mainBook.SetActive(!mainBook.activeSelf);
        }
    }
}
