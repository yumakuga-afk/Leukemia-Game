using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chat2 : MonoBehaviour
{
    int tracker;
    TMP_Text chatbox;
    string[] text;
    [SerializeField] GameObject NotebookCanvas;
    TaskManager TaskList;
    [SerializeField] GameObject Notebook;
    [SerializeField] GameObject CutsceneTrigger;

    private void Awake()
    {
        TaskList = NotebookCanvas.GetComponent<TaskManager>();
        //Gets the text component in the child box so we can set text to it
        chatbox = GetComponent<TMP_Text>();
        text = new string[] {
            "Detective: Rie! It's a pleasure to meet you Rie!",
            "Detective: Sorry I’m late! What a delay, huh?",
            "Rie: Detective, you’re not from around here are you?",
            "Detective: Sorry, no. I'm not from around here.",
            "Rie: That explains it. Any local would know how to work around the VTS.",
            "Detective: Work around?",
            "Rie: The VTS always has something going on. If it actually managed to get you where you were going by the posted time it’d be nothing short of a miracle.",
            "Rie: An hour’s a bit much, even for it.",
            "*Observation added to Detective Notebook: Routine Delays.*",
            "Whenever something that might be important to the case comes up, the detective will make a memo in the Detective’s Notebook. Remember to check it! An observation might not seem helpful at the moment, but who knows when it can come around to crack the case!",
            "Rie: I’m sure a detective like yourself is busy enough without having to listen to me complain about the old VTS.",
            "Detective: Well…That’s the thing.",
            "Detective: I was hoping you would have had the work for me.",
            "Rie: Well *I* was told to accompany you.",
            "...",
            "......",
            "............",
            "*Observation added to Detective Notebook: Incompetent Management.*",
            "Right, okay. Did someone you know give you that order?",
            "My superior officer.",
            "She should be at Outpost 6, which just so happens to be on the opposite side of town",
            "Then that’s who we’re looking for!"            
        };
        //"Player 1:\r\n \r\n So he will. To start, click on the Iron element behind me."
        //Sets default text
        chatbox.text = text[0];
    }
    void OnMouseDown()
    {
        if(tracker + 1 == text.Length)
        {
            GameObject dimmer = GameObject.Find("Second Cutscene");
            dimmer.SetActive(false);
            Notebook.SetActive(true);
            TaskList.AddTask("\r\n*Case #2: New in Town* \r\n \r\n Accompany Rie on a walk through the Radiant City. \r\n Client: Rie");
            CutsceneTrigger.SetActive(false);
            PlayerController2D.playeractive = true;
        }
        else{
            chatbox.text = text[tracker + 1];
            tracker++;
        }
    }
}
