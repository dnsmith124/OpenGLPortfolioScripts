using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    private List<InventoryItem> inventoryItems = new List<InventoryItem>();
    [SerializeField]
    private int goldCount;
    [SerializeField]
    private int healthPotionCount;
    [SerializeField]
    private int manaPotionCount;

    public GameObject healthPotionDisplay;
    public GameObject manaPotionDisplay;

    private void Start()
    {
        UpdatePotionCount();
    }

    public void adjustGoldCount(int amount)
    {
        goldCount += amount;
        Debug.Log($"New gold count: {goldCount}");
    }

    public int getGoldCount()
    {
        return goldCount;
    }

    public void adjustHealthPotionCount(int amount)
    {
        healthPotionCount += amount;
        UpdatePotionCount();
    }

    public int getHealthPotionCount()
    {
        return healthPotionCount;
    }

    public void FlashHealthPotion(bool healthOrMana)
    {
        if(healthOrMana)
            StartCoroutine(FlashCoroutine(1f, healthPotionDisplay.GetComponent<Image>()));
        else 
            StartCoroutine(FlashCoroutine(1f, manaPotionDisplay.GetComponent<Image>()));
    }

    public void adjustManaPotionCount(int amount)
    {
        manaPotionCount += amount;
        UpdatePotionCount();
    }

    public int getManaPotionCount()
    {
        return healthPotionCount;
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

    private void UpdatePotionCount()
    {
        Debug.Log($"new m potion count {manaPotionCount}");
        Debug.Log($"new h potion count {healthPotionCount}");
        manaPotionDisplay.GetComponentInChildren<TextMeshProUGUI>().text = manaPotionCount.ToString();
        healthPotionDisplay.GetComponentInChildren<TextMeshProUGUI>().text = healthPotionCount.ToString();
    }

    IEnumerator FlashCoroutine(float flashTime, Image image)
    {
        // Calculate how long one flash should take (two flashes = flash on and off)
        float singleFlashTime = flashTime / 4f;

        // Do this twice
        for (int i = 0; i < 2; i++)
        {
            // Lerp color to red
            for (float t = 0; t < 1; t += Time.deltaTime / singleFlashTime)
            {
                image.color = Color.Lerp(Color.white, Color.red, t);
                yield return null;
            }

            // Lerp color back to white
            for (float t = 0; t < 1; t += Time.deltaTime / singleFlashTime)
            {
                image.color = Color.Lerp(Color.red, Color.white, t);
                yield return null;
            }
        }
    }
}

