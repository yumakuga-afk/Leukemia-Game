
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    // Save file version.
    public int saveVersion = 2;

    // Time the game was saved.
    public string saveDate;

    // Scene and player location.
    public string sceneName;
    public Vector3 playerPosition;

    // Inventory.
    public List<string> collectedItems =
        new List<string>();

    // Previous interactions.
    public List<string> interactions =
        new List<string>();

    // Discoveries, progression, and world-object flags.
    public List<string> progressionFlags =
        new List<string>();
}
