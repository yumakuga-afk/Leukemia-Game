using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chat3 : MonoBehaviour
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
            "Detective: Is everything alright?",
            "Worker Lady: Hah. About as much as it can be I suppose.",
            "Detective: What seems to be the problem?",
            "Worker Lady: That’s very kind of you, but I don’t really know what anyone could do about this.",
            "Worker Lady: I was supposed to get a delivery ages ago. I’ve done just about all I can do without it and I don’t know where it could be. I really can’t afford to miss it too, so all that’s left for me to do is wait.",
            "Rie: What kind of delivery?",
            "Worker Lady: Just some energy.",
            "Worker Lady: You’d think with there being less of us there’d be more than enough to go around, but things are still tight as ever.",
            "Detective: Would it help any if we found the delivery?",
            "Worker Lady: It sure would!",
            "Workey Lady: If only you could...",
            "Detective: We can.",
            "Rie: We can?",
            "Detective: I'm sure of it.",
            "Worker Lady: Well I'd really appreciate it.",
            "Worker Lady: You folks do look busy, so make sure not to go too far out of your way.",
            "Detective: It'll be no trouble at all!",
            "...",
            "Detective: It'll be one trouble at all!",
            "Worker Lady: And that'd be?",
            "Detective: Introductions!",
            "Detective: I'm the Detective, and this is Rie.",
            "Rie: Nice to meet you.",
            "Bo: You can call me Bo.",
            "Detective: Then, Bo, we're on the case!"           
        };
        //"Player 1:\r\n \r\n So he will. To start, click on the Iron element behind me."
        //Sets default text
        chatbox.text = text[0];
    }
    void OnMouseDown()
    {
        if(tracker + 1 == text.Length)
        {
            GameObject dimmer = GameObject.Find("Third Cutscene");
            dimmer.SetActive(false);
            Notebook.SetActive(true);
            TaskList.AddTask("\r\n*Case #3: Demo Discovery* \r\n \r\n Find the Delayed Delivery. \r\n \r\n Bo is at her wit’s end waiting for her package to be delivered. Help her find it! \r\n Client: Bo");
            CutsceneTrigger.SetActive(false);
            PlayerController2D.playeractive = true;
        }
        else{
            chatbox.text = text[tracker + 1];
            tracker++;
        }
    }
}
