using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Make this class a singleton
    public static GameController Instance { get; private set; }

    public bool isAIEnabled;
    public int gameDifficulty;

    private void Awake()
    {
        // Ensure there is only one instance of this class
        if (Instance == null)
        {
            Instance = this;
            // This ensures that your GameManager is not destroyed when changing scenes.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnableAI()
    {
        isAIEnabled = true;
    }

    public void DisableAI()
    {
        isAIEnabled = false;
    }

    public void SetDifficulty(int difficulty)
    {
        gameDifficulty = difficulty;
    }
}
