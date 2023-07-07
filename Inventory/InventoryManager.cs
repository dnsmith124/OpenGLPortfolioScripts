using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public GameObject uiParent;
    public GameObject uiItemsPanel;
    public GameObject uiItemPrefab;
    private bool isOpen;

    private void Start()
    {
        uiParent.gameObject.SetActive(false);
        isOpen = false;
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
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
    }

    private void HandleCloseUI()
    {
        isOpen = false;
        uiParent.SetActive(false);
        GameController.Instance.ResumeGame();
    }

    private void UpdateUI()
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
                uiText.text = $"{item.itemName} ({item.type}): {item.description}";
            }
        }
    }

    // Call UpdateUI when something changes, such as adding or removing an item
    public void OnInventoryChanged()
    {
        UpdateUI();
    }
}
