using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Make this class a singleton
    public static GameController Instance { get; private set; }

    public bool isAIEnabled;
    public bool isPaused;
    public int gameDifficulty;

    public Dictionary<string, bool> conditions = new Dictionary<string, bool>();


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

        InitConditions();
    }

    void Update()
    {
        if (Input.GetButtonDown("Menu"))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void InitConditions()
    {
        conditions.Add("hasKey", false);
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

    public void AddCondition (string label, bool value)
    {
        conditions.Add(label, value);
    }

    public void UpdateCondition(string label, bool value)
    {
        conditions[label] = value;
    }

    public void RemoveCondition(string label)
    {
        conditions.Remove(label);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        // bring up pause menu here
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        // hide pause menu here
    }
}
