using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawner : MonoBehaviour
{
    [SerializeField] GameObject toDespawn;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && toDespawn != null)
        {
            Destroy(toDespawn);
        }
    }
}
