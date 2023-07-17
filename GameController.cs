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
    public bool isPauseScreenOpen;
    public bool startWithTutorialInfo;
    public int gameDifficulty;
    private CanvasGroup pauseScreen;
    private GameObject startText;
    private GameObject pausedScreenData;
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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitConditions();

        pauseScreen = GameObject.FindGameObjectWithTag("PauseScreen").GetComponent<CanvasGroup>();
        pausedScreenData = GameObject.FindGameObjectWithTag("PausedScreenData");
        startText = GameObject.FindGameObjectWithTag("StartButton");
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (startWithTutorialInfo)
        {
            OpenPauseMenu();
            startText.SetActive(true);
            pausedScreenData.SetActive(false);
        }
        else
        {
            pauseScreen.alpha = 0;
            startText.gameObject.SetActive(true);
            pausedScreenData.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Menu"))
        {
            if (!isPauseScreenOpen)
            {
                if (playerController.GetCanMove())
                    OpenPauseMenu();
            }
            else if(isPauseScreenOpen)
            {
                ClosePauseMenu();
            }
        }
    }

    private void InitConditions()
    {
        conditions.Add("hasKey", false);
        conditions.Add("beatBoss", false);
        conditions.Add("bossLives", true);
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
        isPauseScreenOpen = true;
        pauseScreen.alpha = 1;
        pauseScreen.interactable = true;
        pauseScreen.blocksRaycasts = true;

        playerController.EnterUIMode();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void ClosePauseMenu()
    {
        StartCoroutine(FadePausePanel(false));
        startText.SetActive(false);
        pausedScreenData.SetActive(true);
        ResumeGame();
        isPauseScreenOpen = false;
        playerController.SetCanMove(true);
    }

    private IEnumerator FadePausePanel(bool fadeIn)
    {
        float elapsedTime = 0f;
        float fadeDuration = .3f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // If fadeIn is true, we fade in. Otherwise, we fade out
            if (fadeIn)
            {
                pauseScreen.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            }
            else
            {
                pauseScreen.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            }

            yield return null;
        }

        // If fadeIn is true, we make the UI interactable. Otherwise, we make it non-interactable
        pauseScreen.interactable = fadeIn;
        pauseScreen.blocksRaycasts = fadeIn;
    }
}
