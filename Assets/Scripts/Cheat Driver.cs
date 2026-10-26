using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatDriver : MonoBehaviour
{
    [SerializeField] GameObject textbox;
    [SerializeField] GameObject textbox2;

    // Update is called once per frame
    void Update()
    {
        if (Cheat.count == 6)
        {
            PlayerController2D.playeractive = false;
            textbox.SetActive(true);
            if (textbox2 != null)
            {
                textbox2.SetActive(true);
            }
            Cheat.count = 7;
        }
    }
}
