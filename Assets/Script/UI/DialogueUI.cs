// ============ UI/DialogueUI.cs ============
using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [Header("Panel")]
    public GameObject panel;
    public TMP_Text speakerText;
    public TMP_Text contentText;
    public GameObject continueIndicator;
    public GameObject choiceContainer;

    [Header("Choice")]
    public Button choiceButtonPrefab;

    [Header("Press continue")]
    [SerializeField] private InputActionReference clickCatcher;

    
    [Tooltip("Seconds between each character")]
    public float charInterval = 0.03f;

    
    private bool advanceRequested;      
    private Coroutine typeRoutine;
    private readonly List<Button> choiceButtons = new();

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
       
    }

    void OnEnable()
    {
        SpeakerInputLock.OnLockChanged +=ShowDialogue;
        clickCatcher.action.Enable();
        clickCatcher.action.performed += OnClickCatcher;

    }
    void OnDisable()
    {
        SpeakerInputLock.OnLockChanged -=ShowDialogue;
        clickCatcher.action.Disable();
        clickCatcher.action.performed -= OnClickCatcher;

    }

    private void ShowDialogue(bool shouldShow)
    {
        panel.SetActive(shouldShow);
    }




    // ============ 单行显示 ============
   public IEnumerator ShowLine(string line, List<string> currentTags, bool waitForClick)
    {
        ClearChoices();
        speakerText.text = ExtractSpeaker(currentTags) ?? "";
        contentText.text = line;
        contentText.maxVisibleCharacters = 0;
        // contentText.ForceMeshUpdate();
        // just wait a frame to updata mesh
        yield return null;
        int total = contentText.textInfo.characterCount;

        advanceRequested = false;
        continueIndicator.SetActive(false);

        
        float timer = 0f;
        int shown = 0;
        while (shown < total)
        {
            if (advanceRequested)          
            {
                shown = total;
                advanceRequested = false;  // Consume this click so that it does not trigger a "page turn" again.
            }
            else
            {
                timer += Time.deltaTime;
                while (timer >= charInterval && shown < total)
                {
                    timer -= charInterval;
                    shown++;
                }
            }
            contentText.maxVisibleCharacters = shown;
            yield return null;
        }
        contentText.maxVisibleCharacters = total;

        if (!waitForClick) yield break;

        continueIndicator.SetActive(true);
        advanceRequested = false;
        yield return new WaitUntil(() => advanceRequested);
        continueIndicator.SetActive(false);
    }


    public IEnumerator ShowChoices(List<Choice> choices, Action<int> onPicked)
    {
        ClearChoices();
        choiceContainer.SetActive(true);

        int picked = -1;
        for (int i = 0; i < choices.Count; i++)
        {
            int index = i;
            var btn = Instantiate(choiceButtonPrefab, choiceContainer.transform);
            btn.GetComponentInChildren<TMP_Text>().text = choices[i].text;
            btn.onClick.AddListener(() => picked = index);
            choiceButtons.Add(btn);
        }

        yield return new WaitUntil(() => picked >= 0);

        choiceContainer.SetActive(false);
        ClearChoices();
        onPicked(picked);

    }

    private void ClearChoices()
    {
        choiceContainer.SetActive(false);

        foreach (var b in choiceButtons)
            if (b != null) Destroy(b.gameObject);
        choiceButtons.Clear();
    }


    private void OnClickCatcher(InputAction.CallbackContext context)
    {

        if (choiceContainer.activeSelf) return;
        Debug.Log("work");

        advanceRequested = true;
    }

    private string ExtractSpeaker(List<string> tags)
    {
        foreach (var t in tags)
        {
            var trim = t.Trim();
            if (trim.StartsWith("speaker:"))
                return trim.Substring("speaker:".Length).Trim();
        }
        return null;
    }
}