using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject toSpawn;
    [SerializeField] Transform toSpawnLocation;
    bool spawned = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && spawned == false)
        {
            Instantiate(toSpawn, toSpawnLocation.position, toSpawnLocation.rotation);
            spawned = true;
        }
    }
}
