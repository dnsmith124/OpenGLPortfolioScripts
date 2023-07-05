using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<InventoryItem> inventoryItems = new List<InventoryItem>();
    [SerializeField]
    private int goldCount;

    public void adjustGoldCount(int amount)
    {
        goldCount += amount;
        Debug.Log($"New gold count: {goldCount}");
    }

    public int getGoldCount()
    {
        return goldCount;
    }

    // Method to add item to inventory
    public void AddItem(InventoryItem item)
    {
        inventoryItems.Add(item);
        Debug.Log($"Adding item {item.itemName}");
    }

    // Method to remove item from inventory
    public bool RemoveItem(InventoryItem item)
    {
        return inventoryItems.Remove(item);
    }

    // Method to check if an item exists in the inventory
    public bool HasItem(InventoryItem item)
    {
        return inventoryItems.Contains(item);
    }

    // Method to get all items in the inventory
    public List<InventoryItem> GetAllItems()
    {
        return inventoryItems;
    }
}

