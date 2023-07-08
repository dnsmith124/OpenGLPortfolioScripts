using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using ModelShark;

public class InventoryManager : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public GameObject uiParent;
    public GameObject uiItemsPanel;
    public GameObject uiItemPrefab;
    private bool isOpen;
    private GameObject pausePanel;
    private TooltipTrigger tooltipTrigger;

    private void Start()
    {
        uiParent.gameObject.SetActive(false);
        isOpen = false;
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        pausePanel = GameObject.FindGameObjectWithTag("PauseScreen");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            if(!isOpen)
                HandleOpenUI();
            else
                HandleCloseUI();
        }
    }

    private void HandleOpenUI()
    {
        UpdateUI();
        isOpen = true;
        uiParent.SetActive(true);
        GameController.Instance.PauseGame();
        pausePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    private void HandleCloseUI()
    {
        isOpen = false;
        uiParent.SetActive(false);
        GameController.Instance.ResumeGame();
        pausePanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void UpdateUI()
    {
        // First, clear out any existing child objects
        foreach (Transform child in uiItemsPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Then, create a new child for each item in the inventory
        List<InventoryItem> inventoryItems = playerInventory.GetAllItems();
        foreach (InventoryItem item in inventoryItems)
        {
            GameObject uiItem = Instantiate(uiItemPrefab, uiItemsPanel.transform);

            DragDropItem dragDropItem = uiItem.AddComponent<DragDropItem>();
            dragDropItem.RepresentedItem = item;
            Debug.Log(dragDropItem.RepresentedItem);

            // Set image from itemImage
            Image[] uiImages = uiItem.GetComponentsInChildren<Image>();
            if (uiImages[1] != null)
            {
                uiImages[1].sprite = item.itemImage;
            }

            // Set text from itemName and description
            TextMeshProUGUI uiText = uiItem.GetComponentInChildren<TextMeshProUGUI>();
            if (uiText != null)
            {
                //uiText.text = $"{item.itemName} ({item.type}): {item.description}";
                uiText.text = $"{item.itemName} ({item.type})";
            }

            tooltipTrigger = uiItem.GetComponent<TooltipTrigger>();
            tooltipTrigger.SetText("BodyText", item.description);


            string boostText = CalculateBoostText(item);
            tooltipTrigger.SetText("BoostText", boostText);

        }
    }

    // Call UpdateUI when something changes, such as adding or removing an item
    public void OnInventoryChanged()
    {
        UpdateUI();
    }

    public string CalculateBoostText(InventoryItem item)
    {
        string boostText = "";


        if (item.healthBoost > 0)
        {
            boostText += $"Health +{item.healthBoost}";
        }
        if (item.manaBoost > 0)
        {
            boostText += $"Mana +{item.manaBoost}";
        }
        if (item.spellPowerBoost > 0)
        {
            boostText += $"Spell Power +{item.spellPowerBoost}";
        }
        if (item.charmBoost > 0)
        {
            boostText += $"Charm +{item.charmBoost}";
        }

        return boostText;
    }
}
