using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskBar : MonoBehaviour
{
    [SerializeField] private string name;
    [SerializeField] private string description;

    public string Name => name;
    public string Description => description;

    public TaskBar(string name, string description = "")
    {
        this.name = name;
        this.description = description;
    }

    public override string ToString() =>
        string.IsNullOrEmpty(description) ? name : $"{name}: {description}";

}
