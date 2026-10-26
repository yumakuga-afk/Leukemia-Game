using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event1 : MonoBehaviour
{
    [SerializeField] GameObject textbox;
    [SerializeField] GameObject textbox2;
    PlayerController2D playerdata;
    bool eventactive = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerdata = other.GetComponent<PlayerController2D>();
            if (playerdata.investigating == true)
            {
                if (eventactive == true) {return;}
                PlayerController2D.playeractive = false;
                textbox.SetActive(true);
                if (textbox2 != null)
                {
                    textbox2.SetActive(true);
                }
                eventactive = true;
            }
        }
    }
}
