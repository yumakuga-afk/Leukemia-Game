using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chat7 : MonoBehaviour
{
    int tracker;
    TMP_Text chatbox;
    string[] text;
    [SerializeField] GameObject invisiblewall;

    private void Awake()
    {
        //Gets the text component in the child box so we can set text to it
        chatbox = GetComponent<TMP_Text>();
        text = new string[] {
            "Detective: I think I've learned everything I need.",
            "Detective: Rie, I think I've figured it out!",
            "Rie: You've cracked the case?",
            "Detective: Not quite, but I do know where we should go next.",
            "Detective: The Windworks are that way, meaning that whatever is stopping the delivery should be that way too.",
            "Detective: Let's make haste!",
            "Rie: Right!"        
        };
        //"Player 1:\r\n \r\n So he will. To start, click on the Iron element behind me."
        //Sets default text
        chatbox.text = text[0];
    }
    void OnMouseDown()
    {
        if(tracker + 1 == text.Length)
        {
            GameObject dimmer = GameObject.Find("Investigation End Cutscene");
            dimmer.SetActive(false);
            invisiblewall.SetActive(false);
            PlayerController2D.playeractive = true;
        }
        else{
            chatbox.text = text[tracker + 1];
            tracker++;
        }
    }
}
