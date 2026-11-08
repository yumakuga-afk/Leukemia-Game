using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chat6 : MonoBehaviour
{
    int tracker;
    TMP_Text chatbox;
    string[] text;
    [SerializeField] GameObject NotebookCanvas;
    ObservationManager ObservationList;

    private void Awake()
    {
        ObservationList = NotebookCanvas.GetComponent<ObservationManager>();
        //Gets the text component in the child box so we can set text to it
        chatbox = GetComponent<TMP_Text>();
        text = new string[] {
            "It looks like this sign has directions. ",
            "VTS <-- \r\n \r\n Pulmonary Windworks -->",
            "*Observation added to Detective Notebook: General Location.*"          
        };
        //"Player 1:\r\n \r\n So he will. To start, click on the Iron element behind me."
        //Sets default text
        chatbox.text = text[0];
    }
    void OnMouseDown()
    {
        if(tracker + 1 == text.Length)
        {
            GameObject dimmer = GameObject.Find("Sign Cutscene");
            dimmer.SetActive(false);
            ObservationList.AddTask("\r\n*General Location* \r\n \r\n Right now we're between the VTS and the Pulmonary Windworks. Looks like we're still a long ways away from the Lymphatic Outpost.");
            PlayerController2D.playeractive = true;
            Cheat.count += 1;
        }
        else{
            chatbox.text = text[tracker + 1];
            tracker++;
        }
    }
}
