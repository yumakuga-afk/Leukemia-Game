using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialouge : MonoBehaviour
{
    /*
     * When facing NPC Call interactionbool() to show the interaction prompt. Call it again to hide the prompt.
     */
    [SerializeField] private GameObject interaction_prompt;
    [SerializeField] private TextAsset TextAsset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            interactionbool();
        }
    }
    private void interactionbool()
    {
        interaction_prompt.SetActive(!interaction_prompt.activeSelf);
    }
}
