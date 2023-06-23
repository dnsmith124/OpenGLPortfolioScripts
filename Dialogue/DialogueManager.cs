using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueUI; // Assigned in Start
    public TextMeshProUGUI npcText; // Assigned in Inspector
    public TextMeshProUGUI speakerName; // Assigned in Inspector
    public Button[] playerButtons; // Assigned in Inspector
    [Tooltip("Determines Text Reveal Speed. Best between .25 and 1. Lower is faster.")]
    public float textSpeed = 0.5f; // Assigned in Inspector
    [Tooltip("Duration in seconds for the fade in/out animation.")]
    public float fadeDuration = .5f; 

    private CanvasGroup dialogueCanvasGroup;
    private Dictionary<int, DialogueNode> dialogueTree;
    private DialogueNode currentNode;
    private PlayerController playerController;

    private bool isTextFinished = true;
    private string fullText;
    private Coroutine typingCoroutine;

    private void Start()
    {
        dialogueUI = gameObject;
        dialogueCanvasGroup = dialogueUI.GetComponent<CanvasGroup>();
        dialogueCanvasGroup.alpha = 0f; // Start invisible
        dialogueCanvasGroup.interactable = false; // Start non-interactable
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void StartDialogue(NPCDialogue npcDialogue)
    {
        dialogueTree = npcDialogue.GetDialogueData().nodes.ToDictionary(node => node.Id);
        speakerName.text = npcDialogue.GetSpeakerName();
        StartCoroutine(FadeDialogueUI(true));

        playerController.SetCanMove(false);

        dialogueUI.SetActive(true);
        currentNode = dialogueTree[0]; // Assuming first node has id 0
        DisplayNode();
    }

    public void CloseDialogue()
    {
        StartCoroutine(FadeDialogueUI(false)); // If so, fade out and disable the dialogue UI
        playerController.SetCanMove(true);
    }

    public void ChooseOption(int optionIndex)
    {
        if (isTextFinished && optionIndex < currentNode.Options.Count)
        {
            currentNode = dialogueTree[currentNode.Options[optionIndex].NextNodeId];
            DisplayNode();
            if (currentNode.Goodbye) // Check if this node is a 'goodbye' node
            {
                CloseDialogue();
            }
        }
    }

    private void DisplayNode()
    {
        // Start typing out the text
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        fullText = currentNode.DialogueText;
        typingCoroutine = StartCoroutine(TypeText(fullText));

        // Disable buttons until text is finished
        foreach (Button button in playerButtons)
        {
            button.interactable = false;
        }
    }

    private IEnumerator TypeText(string textToType)
    {
        isTextFinished = false;
        npcText.text = "";
        foreach (char letter in textToType.ToCharArray())
        {
            npcText.text += letter;
            yield return new WaitForSeconds(textSpeed * 0.1f); // Adjust this to change typing speed
        }
        isTextFinished = true;

        // Re-enable the buttons
        for (int i = 0; i < playerButtons.Length; i++)
        {
            if (i < currentNode.Options.Count)
            {
                playerButtons[i].gameObject.SetActive(true);
                playerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentNode.Options[i].Text;
                int index = i;
                playerButtons[i].onClick.RemoveAllListeners();
                playerButtons[i].onClick.AddListener(() => ChooseOption(index));
                playerButtons[i].interactable = true;
            }
            else
            {
                playerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTextFinished)
        {
            StopCoroutine(typingCoroutine);
            npcText.text = fullText;
            isTextFinished = true;
            // Re-enable the buttons
            for (int i = 0; i < playerButtons.Length; i++)
            {
                if (i < currentNode.Options.Count)
                {
                    playerButtons[i].gameObject.SetActive(true);
                    playerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentNode.Options[i].Text;
                    int index = i;
                    playerButtons[i].onClick.RemoveAllListeners();
                    playerButtons[i].onClick.AddListener(() => ChooseOption(index));
                    playerButtons[i].interactable = true;
                }
                else
                {
                    playerButtons[i].gameObject.SetActive(false);
                }
            }
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
