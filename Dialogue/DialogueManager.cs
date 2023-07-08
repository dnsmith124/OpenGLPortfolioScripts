using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueUI; 
    public TextMeshProUGUI npcText; 
    public TextMeshProUGUI speakerName; 
    public Button[] playerButtons; 
    [Tooltip("Determines Text Reveal Speed. Best between .25 and 1. Lower is faster.")]
    public float textSpeed = 0.5f;
    [Tooltip("How long to wait after the text has been revealed to display the responses")]
    public float showResponsesDelayTime = 0.5f;
    [Tooltip("Duration in seconds for the fade in/out animation.")]
    public float fadeDuration = .5f;

    private CanvasGroup dialogueCanvasGroup;
    private Dictionary<int, DialogueNode> dialogueTree;
    private DialogueNode currentNode;
    private PlayerController playerController;
    private PlayerStats playerStats;

    private bool isTextFinished = true;
    private string fullText;
    private Coroutine typingCoroutine;
    private ColorBlock conditionNotMetColorBlock;

    private void Start()
    {
        conditionNotMetColorBlock = ColorBlock.defaultColorBlock;
        conditionNotMetColorBlock.disabledColor = new Color(255, 0, 0, .5f);

        dialogueUI = gameObject;
        dialogueCanvasGroup = dialogueUI.GetComponent<CanvasGroup>();
        dialogueCanvasGroup.alpha = 0f; 
        dialogueCanvasGroup.interactable = false; 
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    public void StartDialogue(NPCDialogue npcDialogue)
    {
        dialogueTree = npcDialogue.GetDialogueData().nodes.ToDictionary(node => node.Id);
        speakerName.text = npcDialogue.GetSpeakerName();
        StartCoroutine(FadeDialogueUI(true));

        playerController.EnterUIMode();

        dialogueUI.SetActive(true);
        currentNode = dialogueTree[0]; 
        DisplayNode();
    }

    public void CloseDialogue()
    {
        StartCoroutine(FadeDialogueUI(false));
        playerController.SetCanMove(true);
    }

    public void ChooseOption(int optionIndex, Button[] buttons)
    {

        if (isTextFinished && optionIndex < currentNode.Options.Count)
        {
            currentNode = dialogueTree[currentNode.Options[optionIndex].NextNodeId];
            for (int i = 0; i < buttons.Length; i++)
            {
                // if its not the selected option, hide it
                if (i != optionIndex)
                    buttons[i].gameObject.SetActive(false);
                // if it is, make it non-interactable
                else
                    buttons[i].GetComponent<Button>().interactable = false;
            }
            // highlight selected option, unhighlight deselected options
            DisplayNode();
            
            if (currentNode.Goodbye)
            {
                CloseDialogue();
            }
        }
    }

    private bool CheckCondition(string condition)
    {
        if (string.IsNullOrEmpty(condition))
            return true;

        string[] parts = condition.Split(':');
        if (parts.Length != 2)
            return GameController.Instance.conditions[condition];

        switch (parts[0])
        {
            //case "Strength":
            //    return playerStats.Strength >= int.Parse(parts[1]);
            //case "Intelligence":
            //    return playerStats.Intelligence >= int.Parse(parts[1]);
            // ... other cases ...
            default:
                return false;
        }
    }

    private void DisplayNode()
    {
        // Start typing out the text
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        fullText = currentNode.DialogueText;
        typingCoroutine = StartCoroutine(TypeText(fullText));

    }

    private void EnableButtons(Button[] buttons)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i >= currentNode.Options.Count)
            {
                buttons[i].gameObject.SetActive(false);
            }
            else
            {
                Button button = buttons[i].GetComponent<Button>();
                string condition = currentNode.Options[i].Condition;
                // default button text option
                string buttonText = currentNode.Options[i].Text;

                // if the condition is met (also returns true for null)
                if (CheckCondition(condition))
                {
                    int index = i;
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => ChooseOption(index, buttons));
                    button.interactable = true;
                    button.colors = ColorBlock.defaultColorBlock;
                    buttonText = currentNode.Options[i].Text;
                }
                // if the condition is not met, apply fail text (if it exists), set the color and make it uninteractable
                else
                {
                    // overwrite default text if failText exists
                    if (currentNode.Options[i].FailText != null)
                        buttonText = currentNode.Options[i].FailText;

                    button.colors = conditionNotMetColorBlock;
                    button.interactable = false;
                }

                // if it has a condition, and the condition is 2 parts (i.e. has a value beyond true/false), add that to the text
                if (condition != null && condition.Split(':').Length == 2)
                {
                    string[] parts = currentNode.Options[i].Condition.Split(':');
                    string originalText = buttonText;
                    PropertyInfo propertyInfo = playerStats.GetType().GetProperty(parts[0]);
                    int playerStatValue = (int)propertyInfo.GetValue(playerStats);
                    buttonText = $"{originalText} [{parts[0]}: {playerStatValue}/{parts[1]}]";
                }

                button.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
                button.gameObject.SetActive(true);
            }
        }
    }

    private IEnumerator TypeText(string textToType)
    {
        isTextFinished = false;
        npcText.text = "";
        foreach (char letter in textToType.ToCharArray())
        {
            npcText.text += letter;
            yield return new WaitForSeconds(textSpeed * 0.1f);
        }
        isTextFinished = true;

        yield return new WaitForSeconds(showResponsesDelayTime);

        EnableButtons(playerButtons);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTextFinished)
        {
            StopCoroutine(typingCoroutine);
            npcText.text = fullText;
            isTextFinished = true;
            // Re-enable the buttons
            EnableButtons(playerButtons);
        }
    }
    private IEnumerator FadeDialogueUI(bool fadeIn)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // If fadeIn is true, we fade in. Otherwise, we fade out
            if (fadeIn)
            {
                dialogueCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            }
            else
            {
                dialogueCanvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            }

            yield return null;
        }

        // If fadeIn is true, we make the UI interactable. Otherwise, we make it non-interactable
        dialogueCanvasGroup.interactable = fadeIn;
    }
}
