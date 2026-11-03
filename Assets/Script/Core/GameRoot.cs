// ============ Core/GameRoot.cs ============
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    public static GameRoot Instance;

    public GameDatabase database;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}