using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Changer : MonoBehaviour
{
    [SerializeField] GameObject thispage;
    [SerializeField] GameObject lastpage;

    void OnMouseDown()
    {
        thispage.SetActive(true);
        lastpage.SetActive(false);
    }
}
