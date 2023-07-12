using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Make this class a singleton
    public static GameController Instance { get; private set; }

    public bool isAIEnabled;
    public bool isPaused;
    public bool startWithTutorialInfo;
    public int gameDifficulty;
    private CanvasGroup pauseScreen;
    private Button startButton;
    private PlayerController playerController;

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
        startButton = GameObject.FindGameObjectWithTag("StartButton").GetComponent<Button>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (startWithTutorialInfo)
        {
            PauseGame();
            startButton.gameObject.SetActive(true);
        }
        else
        {
            pauseScreen.alpha = 0;
            startButton.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Menu"))
        {
            if (isPaused)
            {
                ClosePauseMenu();
            }
            else
            {
                OpenPauseMenu();
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
    public void OpenPauseMenu()
    {
        PauseGame();
        pauseScreen.alpha = 1;
        playerController.SetCanMove(false);
        pauseScreen.interactable = true;
        pauseScreen.blocksRaycasts = true;

    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void ClosePauseMenu()
    {
        startButton.gameObject.SetActive(false);
        pauseScreen.alpha = 0;
        playerController.SetCanMove(true);
        pauseScreen.blocksRaycasts = false;
        pauseScreen.interactable = false;
        ResumeGame();
    }
}
