using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public string speakerName;
    public float interactionDistance = 2.0f;
    public string dialogueJsonFileName;
    public DialogueManager dialogueManager; 

    private Camera mainCamera;
    private GameObject playerObject;
    private DialogueData dialogueData;
    private bool hasBeenClicked = false;

    void Start()
    {
        LoadDialogue(dialogueJsonFileName);
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        dialogueManager = GameObject.FindGameObjectWithTag("DialogueBox").GetComponent<DialogueManager>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if(hasBeenClicked)
        {
            float distance = Vector3.Distance(gameObject.transform.position, playerObject.transform.position);
            if (distance <= interactionDistance)
            {
                dialogueManager.StartDialogue(gameObject.GetComponent<NPCDialogue>());
                hasBeenClicked = false;
            }
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if(hit.collider.gameObject.CompareTag("NPC")) {
                    hasBeenClicked = true;
                }
            }
        }
    }

    private void LoadDialogue(string fileName)
    {
        TextAsset dialogueAsset = Resources.Load<TextAsset>($"NPCDialogue/{fileName}");

        if (dialogueAsset == null)
        {
            Debug.LogError($"Failed to load dialogue file: {fileName}");
            return;
        }

        dialogueData = JsonUtility.FromJson<DialogueData>(dialogueAsset.text);
    }

    public DialogueData GetDialogueData()
    {
        return dialogueData;
    }

    public string GetSpeakerName()
    {
        return speakerName;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}