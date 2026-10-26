using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chat1 : MonoBehaviour
{
    int tracker;
    TMP_Text chatbox;
    string[] text;

    private void Awake()
    {
        //Gets the text component in the child box so we can set text to it
        chatbox = GetComponent<TMP_Text>();
        text = new string[] {
            "???: Pleasure to meet you, detective! Assistant researcher Rie reporting in.",
            "Detective: It's a pleasure to meet you! Err...",
            "(It looks like the detective is having trouble remembering the name of who they're here to meet. Help remind them by checking the request.)",
            "*Tutorial: Detective Notebook*",
            "All kinds of handy things can be found in your trusty notebook. Open it up to go over collected clues, cases, and people of interest you meet during your investigation. Give it a try now to help remind the detective of who it is they’re speaking to!",
            "Press *Notebook Button* to open the Detective's Notebook."
        };
        //"Player 1:\r\n \r\n So he will. To start, click on the Iron element behind me."
        //Sets default text
        chatbox.text = text[0];
    }
    void OnMouseDown()
    {
        if(tracker + 1 == text.Length)
        {
            GameObject dimmer = GameObject.Find("First Cutscene");
            //dimmer.SetActive(false);
            //PlayerController2D.playeractive = true;
        }
        else{
            chatbox.text = text[tracker + 1];
            tracker++;
        }
    }
}
