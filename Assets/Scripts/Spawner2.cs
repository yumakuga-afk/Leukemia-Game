using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner2 : MonoBehaviour
{
    [SerializeField] GameObject toSpawn;
    [SerializeField] Transform toSpawnLocation;
    [SerializeField] GameObject toSpawn2;
    [SerializeField] Transform toSpawnLocation2;
    bool spawned = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && spawned == false)
        {
            Instantiate(toSpawn, toSpawnLocation.position, toSpawnLocation.rotation);
            Instantiate(toSpawn2, toSpawnLocation2.position, toSpawnLocation2.rotation);
            spawned = true;
        }
    }
}
