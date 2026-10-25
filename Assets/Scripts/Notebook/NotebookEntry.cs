
using System;
using System.Collections.Generic;
using UnityEngine;

// Categories used to organize notebook entries.
public enum NotebookCategory
{
    CaseFiles,
    Evidence,
    Locations,
    MedicalInformation,
    General
}

// Represents an additional section of information
// that can be unlocked within a notebook entry.
[Serializable]
public class NotebookEntrySection
{
    [Header("Section Information")]

    [Tooltip("Unique identifier for this section.")]
    public string sectionID;

    [Tooltip("Heading displayed above the section.")]
    public string sectionTitle;

    [TextArea(3, 10)]
    [Tooltip("Information displayed when unlocked.")]
    public string sectionContent;

    [Header("Unlock Requirements")]

    [Tooltip("Conditions required to display this section.")]
    public NotebookConditionGroup unlockConditions;

    // Determines whether this section can be displayed.
    public bool IsUnlocked()
    {
        return unlockConditions == null ||
               unlockConditions.IsMet();
    }
}

// Defines an entry in the detective's notebook.
[CreateAssetMenu(
    fileName = "NewNotebookEntry",
    menuName = "Detective Notebook/Notebook Entry"
)]
public class NotebookEntry : ScriptableObject
{
    [Header("Entry Identification")]

    [Tooltip("Unique identifier for this notebook entry.")]
    public string entryID;

    [Tooltip("Name displayed in the notebook.")]
    public string entryTitle;

    [Tooltip("Category used to organize the entry.")]
    public NotebookCategory category;

    [Header("Entry Appearance")]

    [Tooltip("Optional image displayed with the entry.")]
    public Sprite entryIcon;

    [Header("Basic Information")]

    [TextArea(3, 10)]
    [Tooltip("Basic information displayed once the entry is unlocked.")]
    public string description;

    [Header("Entry Unlock Requirements")]

    [Tooltip("Conditions required to unlock this entry.")]
    public NotebookConditionGroup unlockConditions;

    [Header("Additional Information")]

    [Tooltip("Additional sections unlocked through investigation.")]
    public List<NotebookEntrySection> additionalSections =
        new List<NotebookEntrySection>();

    // ==========================================
    // ENTRY UNLOCK CHECK
    // ==========================================

    public bool IsUnlocked()
    {
        return unlockConditions == null ||
               unlockConditions.IsMet();
    }

    // ==========================================
    // SECTION UNLOCK CHECK
    // ==========================================

    public List<NotebookEntrySection> GetUnlockedSections()
    {
        List<NotebookEntrySection> unlockedSections =
            new List<NotebookEntrySection>();

        // Do not reveal sections if the entry is locked.
        if (!IsUnlocked())
            return unlockedSections;

        foreach (NotebookEntrySection section in additionalSections)
        {
            if (section != null && section.IsUnlocked())
            {
                unlockedSections.Add(section);
            }
        }

        return unlockedSections;
    }

    // ==========================================
    // ENTRY INFORMATION
    // ==========================================

    public bool HasUnlockedSections()
    {
        return GetUnlockedSections().Count > 0;
    }
}
