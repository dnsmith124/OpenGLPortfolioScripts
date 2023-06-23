using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public string speakerName; // Assign in Inspector
    public float interactionDistance = 2.0f;
    public string dialogueJsonFileName; // Assign in Inspector
    public DialogueManager dialogueManager; 

    private Camera mainCamera;
    private GameObject playerObject;
    private DialogueData dialogueData;
    private bool hasBeenClicked = false;

    // Start is called before the first frame update
    void Start()
    {
        LoadDialogue(dialogueJsonFileName);
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        dialogueManager = GameObject.FindGameObjectWithTag("DialogueBox").GetComponent<DialogueManager>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
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

                    float distance = Vector3.Distance(gameObject.transform.position, playerObject.transform.position);
                    if (distance > interactionDistance)
                        hasBeenClicked = true;
                }
            }
        }
    }

    private void LoadDialogue(string fileName)
    {
        string path = $"Assets/Resources/NPCDialogue/{fileName}.json";
        string json = File.ReadAllText(path);
        dialogueData = JsonUtility.FromJson<DialogueData>(json);
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