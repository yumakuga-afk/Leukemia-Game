using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chat8 : MonoBehaviour
{
    int tracker;
    TMP_Text chatbox;
    string[] text;

    private void Awake()
    {
        //Gets the text component in the child box so we can set text to it
        chatbox = GetComponent<TMP_Text>();
        text = new string[] {
            "Detective: Do you see that Rie? I think we found the source of our blockage.",
            "Rie: How could I not see!?",
            "Rie: So Bo, do you think your delivery's in there?",
            "Bo: It'd take a bit of lookin', but I'm sure it's gotta be one of em.",
            "Rie: Do you want a hand?",
            "Detective: We're always ready assist.",
            "Bo: Ah! A little thing like this is nothing!", 
            "Bo: Besides, sweet as you all have been, you've already done more than enough.",
            "Bo: Why, after this I bet I could have this whole place sprucened up by tomorrow!",
            "Detective & Rie: We'll be looking forward to it!",
            "*Case #3: Demo Discovery* \r\n \r\n Case Closed!"       
        };
        //"Player 1:\r\n \r\n So he will. To start, click on the Iron element behind me."
        //Sets default text
        chatbox.text = text[0];
    }
    void OnMouseDown()
    {
        if(tracker + 1 == text.Length)
        {
    
        }
        else{
            chatbox.text = text[tracker + 1];
            tracker++;
        }
    }
}
