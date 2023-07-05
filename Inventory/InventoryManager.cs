using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public PlayerInventory playerInventory; // assign in Inspector
    public Transform uiParent; // the parent object containing the UI elements
    public GameObject uiItemPrefab; // the prefab for a single inventory UI element

    private void UpdateUI()
    {
        // First, clear out any existing child objects
        foreach (Transform child in uiParent)
        {
            Destroy(child.gameObject);
        }

        // Then, create a new child for each item in the inventory
        List<InventoryItem> inventoryItems = playerInventory.GetAllItems();
        foreach (InventoryItem item in inventoryItems)
        {
            GameObject uiItem = Instantiate(uiItemPrefab, uiParent);

            // Set image from itemImage
            Image uiImage = uiItem.GetComponentInChildren<Image>();
            if (uiImage != null)
            {
                uiImage.sprite = item.itemImage;
            }

            // Set text from itemName and description
            Text uiText = uiItem.GetComponentInChildren<Text>();
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
