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
    private CanvasGroup pauseScreen;

    public Dictionary<string, bool> conditions = new Dictionary<string, bool>();

    public Texture2D cursorTexture;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;

    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

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

        pauseScreen = GameObject.FindGameObjectWithTag("PauseScreen").GetComponent<CanvasGroup>();
        pauseScreen.alpha = 0;
    }

    void Update()
    {
        if (Input.GetButtonDown("Menu"))
        {
            if (isPaused)
            {
                ResumeGame();
                pauseScreen.alpha = 0;
            }
            else
            {
                PauseGame();
                pauseScreen.alpha = 1;
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
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }
}
