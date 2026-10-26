using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    [SerializeField] GameObject textbox;
    [SerializeField] GameObject textbox2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerController2D.playeractive = false;
            textbox.SetActive(true);
            if (textbox2 != null)
            {
                textbox2.SetActive(true);
            }
        }
    }
}
