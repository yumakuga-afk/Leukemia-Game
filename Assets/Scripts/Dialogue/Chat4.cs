using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chat4 : MonoBehaviour
{
    int tracker;
    TMP_Text chatbox;
    string[] text;

    private void Awake()
    {
        //Gets the text component in the child box so we can set text to it
        chatbox = GetComponent<TMP_Text>();
        text = new string[] {
            "Detective: Do you know what this package will look like?",
            "Bo: ...?",
            "Rie: The detective is new to the city.",
            "Rie: What we'll be looking for is a delivery robot an RBR.",
            "Rie: They're red ring-shaped, and can be found just about anywhere.",
            "Rie: Although you're most likely to find them in the vessels.",
            "Rie: Well, they *should* be in the vessels.",
            "Detective: Have any been passing by?",
            "Bo: ...",
            "Bo: Not that I remember",
            "*Observation added to Detective Notebook: Vacant Vessel.*",
            "Detective: Excellent! We’ve already got a lead!",
            "Rie: How do you figure?",
            "Detective: If something is supposed to be flowing, but isn't, then there must be a cause.",
            "Rie: The real question is what's causing it?",
            "Detective: What indeed?"          
        };
        //"Player 1:\r\n \r\n So he will. To start, click on the Iron element behind me."
        //Sets default text
        chatbox.text = text[0];
    }
    void OnMouseDown()
    {
        if(tracker + 1 == text.Length)
        {
            GameObject dimmer = GameObject.Find("Bo Cutscene");
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
