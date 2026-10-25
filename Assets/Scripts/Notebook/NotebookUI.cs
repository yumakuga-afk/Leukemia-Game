
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NotebookUI : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference toggleNotebookAction;

    [Header("Main UI")]
    [SerializeField] private GameObject notebookPanel;
    [SerializeField] private Button closeButton;

    [Header("Entry Navigation")]
    [SerializeField] private TMP_Dropdown categoryDropdown;
    [SerializeField] private Transform entryListContent;
    [SerializeField] private Button entryButtonPrefab;

    [Header("Entry Information")]
    [SerializeField] private TMP_Text entryTitle;
    [SerializeField] private TMP_Text entryDescription;
    [SerializeField] private TMP_Text additionalSectionsText;
    [SerializeField] private Image entryIcon;

    [Header("Settings")]
    [SerializeField] private bool openOnStart = false;

    private NotebookManager notebookManager;
    private NotebookCategory selectedCategory;
    private string selectedEntryID;

    private bool isOpen;
    private bool subscribedToManager;

    private readonly List<Button> spawnedButtons =
        new List<Button>();

    // ==========================================
    // INITIALIZATION
    // ==========================================

    private void Awake()
    {
        selectedCategory = NotebookCategory.CaseFiles;

        if (notebookPanel != null)
        {
            notebookPanel.SetActive(false);
        }

        SetupCategoryDropdown();

        if (categoryDropdown != null)
        {
            categoryDropdown.onValueChanged.AddListener(
                OnCategoryChanged
            );
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseNotebook);
        }
    }

    private void OnEnable()
    {
        if (toggleNotebookAction != null)
        {
            toggleNotebookAction.action.performed += OnToggleNotebook;
            toggleNotebookAction.action.Enable();
        }

        ConnectToNotebookManager();
    }

    private IEnumerator Start()
    {
        // Allow persistent managers to finish Start()
        // before the first notebook refresh.
        yield return null;

        ConnectToNotebookManager();

        if (openOnStart)
        {
            OpenNotebook();
        }
        else
        {
            CloseNotebook();
        }
    }

    private void OnDisable()
    {
        if (toggleNotebookAction != null)
        {
            toggleNotebookAction.action.performed -= OnToggleNotebook;
            toggleNotebookAction.action.Disable();
        }

        DisconnectFromNotebookManager();
    }

    private void OnDestroy()
    {
        if (categoryDropdown != null)
        {
            categoryDropdown.onValueChanged.RemoveListener(
                OnCategoryChanged
            );
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(CloseNotebook);
        }

        DisconnectFromNotebookManager();
    }

    // ==========================================
    // NOTEBOOK MANAGER CONNECTION
    // ==========================================

    private void ConnectToNotebookManager()
    {
        NotebookManager currentManager = NotebookManager.Instance;

        if (notebookManager == currentManager &&
            subscribedToManager)
        {
            return;
        }

        DisconnectFromNotebookManager();

        notebookManager = currentManager;

        if (notebookManager == null)
        {
            Debug.LogWarning(
                "[NotebookUI] NotebookManager not found."
            );
            return;
        }

        notebookManager.OnNotebookChanged += HandleNotebookChanged;
        subscribedToManager = true;
    }

    private void DisconnectFromNotebookManager()
    {
        if (notebookManager != null && subscribedToManager)
        {
            notebookManager.OnNotebookChanged -= HandleNotebookChanged;
        }

        subscribedToManager = false;
        notebookManager = null;
    }

    private void HandleNotebookChanged()
    {
        // No reason to redraw while the notebook is closed.
        // The next opening will retrieve the latest information.
        if (isOpen)
        {
            RefreshNotebookUI();
        }
    }

    // ==========================================
    // OPENING AND CLOSING
    // ==========================================

    private void OnToggleNotebook(
        InputAction.CallbackContext context)
    {
        ToggleNotebook();
    }

    public void ToggleNotebook()
    {
        if (isOpen)
        {
            CloseNotebook();
        }
        else
        {
            OpenNotebook();
        }
    }

    public void OpenNotebook()
    {
        if (notebookPanel == null)
        {
            Debug.LogWarning(
                "[NotebookUI] Notebook Panel is not assigned."
            );
            return;
        }

        ConnectToNotebookManager();

        isOpen = true;
        notebookPanel.SetActive(true);

        RefreshNotebookUI();
    }

    public void CloseNotebook()
    {
        isOpen = false;

        if (notebookPanel != null)
        {
            notebookPanel.SetActive(false);
        }
    }

    // ==========================================
    // CATEGORY NAVIGATION
    // ==========================================

    private void SetupCategoryDropdown()
    {
        if (categoryDropdown == null)
            return;

        categoryDropdown.ClearOptions();

        List<string> categoryNames = new List<string>();

        foreach (NotebookCategory category in
                 Enum.GetValues(typeof(NotebookCategory)))
        {
            categoryNames.Add(category.ToString());
        }

        categoryDropdown.AddOptions(categoryNames);
        categoryDropdown.SetValueWithoutNotify(
            (int)selectedCategory
        );

        categoryDropdown.RefreshShownValue();
    }

    private void OnCategoryChanged(int categoryIndex)
    {
        selectedCategory = (NotebookCategory)categoryIndex;
        selectedEntryID = null;

        RefreshNotebookUI();
    }

    // ==========================================
    // NOTEBOOK UI REFRESH
    // ==========================================

    public void RefreshNotebookUI()
    {
        if (!isOpen)
            return;

        ConnectToNotebookManager();

        ClearEntryButtons();

        if (notebookManager == null)
        {
            ShowEmptyEntry("Notebook data unavailable.");
            return;
        }

        List<NotebookEntry> entries =
            notebookManager.GetEntriesByCategory(
                selectedCategory
            );

        if (entries.Count == 0)
        {
            selectedEntryID = null;
            ShowEmptyEntry(
                "No information discovered in this category."
            );
            return;
        }

        // Keep the currently selected entry when possible.
        NotebookEntry selectedEntry = null;

        foreach (NotebookEntry entry in entries)
        {
            if (entry.entryID == selectedEntryID)
            {
                selectedEntry = entry;
                break;
            }
        }

        // Select the first entry if none is currently selected.
        if (selectedEntry == null)
        {
            selectedEntry = entries[0];
            selectedEntryID = selectedEntry.entryID;
        }

        // Generate the buttons for available entries.
        foreach (NotebookEntry entry in entries)
        {
            CreateEntryButton(entry);
        }

        DisplayEntry(selectedEntry);
    }

    // ==========================================
    // ENTRY BUTTON GENERATION
    // ==========================================

    private void CreateEntryButton(NotebookEntry entry)
    {
        if (entryButtonPrefab == null ||
            entryListContent == null)
        {
            return;
        }

        Button newButton = Instantiate(
            entryButtonPrefab,
            entryListContent
        );

        newButton.gameObject.SetActive(true);
        newButton.name = "Entry_" + entry.entryID;

        TMP_Text buttonText =
            newButton.GetComponentInChildren<TMP_Text>();

        if (buttonText != null)
        {
            buttonText.text = entry.entryTitle;
        }

        NotebookEntry capturedEntry = entry;

        newButton.onClick.AddListener(
            () => SelectEntry(capturedEntry)
        );

        spawnedButtons.Add(newButton);
    }

    private void ClearEntryButtons()
    {
        foreach (Button button in spawnedButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }

        spawnedButtons.Clear();
    }

    // ==========================================
    // ENTRY SELECTION
    // ==========================================

    private void SelectEntry(NotebookEntry entry)
    {
        if (entry == null)
            return;

        selectedEntryID = entry.entryID;
        DisplayEntry(entry);
    }

    // ==========================================
    // ENTRY DISPLAY
    // ==========================================

    private void DisplayEntry(NotebookEntry entry)
    {
        if (entry == null ||
            notebookManager == null ||
            !notebookManager.IsEntryUnlocked(entry.entryID))
        {
            ShowEmptyEntry("Entry unavailable.");
            return;
        }

        if (entryTitle != null)
        {
            entryTitle.text = entry.entryTitle;
        }

        if (entryDescription != null)
        {
            entryDescription.text = entry.description;
        }

        if (entryIcon != null)
        {
            bool hasIcon = entry.entryIcon != null;

            entryIcon.enabled = hasIcon;

            if (hasIcon)
            {
                entryIcon.sprite = entry.entryIcon;
            }
        }

        if (additionalSectionsText != null)
        {
            StringBuilder sectionText = new StringBuilder();

            List<NotebookEntrySection> sections =
                entry.GetUnlockedSections();

            foreach (NotebookEntrySection section in sections)
            {
                sectionText.AppendLine();
                sectionText.AppendLine(
                    "<b>" + section.sectionTitle + "</b>"
                );
                sectionText.AppendLine();
                sectionText.AppendLine(section.sectionContent);
                sectionText.AppendLine();
            }

            additionalSectionsText.text = sectionText.ToString();
        }
    }

    // ==========================================
    // EMPTY ENTRY DISPLAY
    // ==========================================

    private void ShowEmptyEntry(string message)
    {
        if (entryTitle != null)
        {
            entryTitle.text = "No Entry Selected";
        }

        if (entryDescription != null)
        {
            entryDescription.text = message;
        }

        if (additionalSectionsText != null)
        {
            additionalSectionsText.text = "";
        }

        if (entryIcon != null)
        {
            entryIcon.enabled = false;
        }
    }
}
