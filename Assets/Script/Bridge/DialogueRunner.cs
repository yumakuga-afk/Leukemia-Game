// ============ Bridge/DialogueRunner.cs ============
using Ink.Runtime;
using System;
using System.Collections;
using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
    public static DialogueRunner Instance;

    void Awake() => Instance = this;

    public void Run(Story story, Action onEnd)
        => StartCoroutine(RunRoutine(story, onEnd));

  private IEnumerator RunRoutine(Story story, Action onEnd)
    {
        SpeakerInputLock.Lock();

        while (story.canContinue || story.currentChoices.Count > 0)
        {
            if (story.canContinue)
            {
                string line = story.Continue().Trim();
                if (string.IsNullOrEmpty(line)) continue;

                // if there have choice follow, then don't have to waith player click, just show the choice
                bool choicesFollow = !story.canContinue && story.currentChoices.Count > 0;
                yield return DialogueUI.Instance.ShowLine(line, story.currentTags, !choicesFollow);
            }
            else
            {
                int picked = -1;
                yield return DialogueUI.Instance.ShowChoices(story.currentChoices, i => picked = i);
                story.ChooseChoiceIndex(picked);
            }
        }

        SpeakerInputLock.Unlock();

        onEnd?.Invoke();   
    }
}