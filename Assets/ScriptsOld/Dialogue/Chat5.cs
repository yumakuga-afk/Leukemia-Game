using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chat5 : MonoBehaviour
{
    int tracker;
    TMP_Text chatbox;
    string[] text;

    private void Awake()
    {
        //Gets the text component in the child box so we can set text to it
        chatbox = GetComponent<TMP_Text>();
        text = new string[] {
            "Detective: Do you know if these vessels flow a certain way?",
            "Rie: These look like A-Line vessels, so they'd be coming from the Windworks.",
            "*Observation added to Detective Notebook: A-Line Vessel.*",
            "Detective: Which way is the Windworks from here?",
            "Rie: I'm not too sure.",
            "Rie: Maybe there's something around that could give us an idea?"
        };
        //"Player 1:\r\n \r\n So he will. To start, click on the Iron element behind me."
        //Sets default text
        chatbox.text = text[0];
    }
    void OnMouseDown()
    {
        if(tracker + 1 == text.Length)
        {
            GameObject dimmer = GameObject.Find("Rie Cutscene");
            dimmer.SetActive(false);
            PlayerController2D.playeractive = true;
            Cheat.count += 1;
        }
        else{
            chatbox.text = text[tracker + 1];
            tracker++;
        }
    }
}
