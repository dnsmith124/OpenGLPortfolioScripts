using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private List<InventoryItem> inventoryItems = new List<InventoryItem>();
    [SerializeField]
    private List<InventoryItem> equippedItems = new List<InventoryItem>();
    [SerializeField]
    private int goldCount;
    [SerializeField]
    private int healthPotionCount;
    [SerializeField]
    private int manaPotionCount;

    public GameObject healthPotionDisplay;
    public GameObject manaPotionDisplay;
    public TextMeshProUGUI healthPotionInvCount;
    public TextMeshProUGUI manaPotionInvCount;
    public TextMeshProUGUI goldInvCount;
    public CanvasGroup pickupChimePanel;
    public float pickupChimeDelayTime = 2.0f;
    public float pickupChimeFadeDuration = 1.0f;
    private PlayerStats playerStats;


    private Queue<string> pickupQueue = new Queue<string>();
    private bool isShowingChime = false;

    private void Start()
    {
        UpdatePotionCount();
        playerStats = GetComponent<PlayerStats>();
    }

    public void adjustGoldCount(int amount)
    {
        if (amount > 0)
        {
            pickupQueue.Enqueue($"{amount} gold");

            if (!isShowingChime)
            {
                StartCoroutine(HandlePickupChimeToggle());
            }
        }
        goldCount += amount;
        goldInvCount.text = goldCount.ToString();
        Debug.Log($"New gold count: {goldCount}");
    }

    public int getGoldCount()
    {
        return goldCount;
    }

    public void adjustHealthPotionCount(int amount)
    {
        if (amount > 0)
        {
            pickupQueue.Enqueue("Health Potion");

            if (!isShowingChime)
            {
                StartCoroutine(HandlePickupChimeToggle());
            }
        }
        healthPotionCount += amount;
        UpdatePotionCount();
    }

    public int getHealthPotionCount()
    {
        return healthPotionCount;
    }

    public void FlashPotion(bool healthOrMana)
    {
        if(healthOrMana)
            StartCoroutine(FlashCoroutine(1f, healthPotionDisplay.GetComponent<Image>()));
        else 
            StartCoroutine(FlashCoroutine(1f, manaPotionDisplay.GetComponent<Image>()));
    }

    public void adjustManaPotionCount(int amount)
    {
        if (amount > 0)
        {
            pickupQueue.Enqueue("Mana Potion");

            if (!isShowingChime)
            {
                StartCoroutine(HandlePickupChimeToggle());
            }
        }

        manaPotionCount += amount;
        UpdatePotionCount();
    }

    public int getManaPotionCount()
    {
        return manaPotionCount;
    }

    // Method to add item to inventory
    public void AddItem(InventoryItem item)
    {
        inventoryItems.Add(item);
        pickupQueue.Enqueue(item.itemName);

        if (!isShowingChime)
        {
            StartCoroutine(HandlePickupChimeToggle());
        }
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

    public void AddEquippedItem(InventoryItem item)
    {
        equippedItems.Add(item);
        playerStats.UpdateStatsBasedOnEquipment();
    }

    public void RemoveEquippedItem(InventoryItem item)
    {
        equippedItems.Remove(item);
        playerStats.UpdateStatsBasedOnEquipment();
    }

    public bool HasEquippedItem(InventoryItem item)
    {
        return equippedItems.Contains(item);
    }

    public List<InventoryItem> GetAllEquippedItems()
    {
        return equippedItems;
    }

    private void UpdatePotionCount()
    {
        manaPotionDisplay.GetComponentInChildren<TextMeshProUGUI>().text = manaPotionCount.ToString();
        manaPotionInvCount.text = manaPotionCount.ToString();
        healthPotionDisplay.GetComponentInChildren<TextMeshProUGUI>().text = healthPotionCount.ToString();
        healthPotionInvCount.text = healthPotionCount.ToString();
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

    private IEnumerator HandlePickupChimeToggle()
    {
        isShowingChime = true;

        while (pickupQueue.Count > 0)
        {
            string name = pickupQueue.Dequeue();

            pickupChimePanel.GetComponentInChildren<TextMeshProUGUI>().text = $"Picked up {name}.";

            // Fade in
            for (float t = 0; t < pickupChimeFadeDuration; t += Time.deltaTime)
            {
                pickupChimePanel.alpha = t / pickupChimeFadeDuration;
                yield return null;
            }
            pickupChimePanel.alpha = 1;

            // wait
            yield return new WaitForSeconds(pickupChimeDelayTime);

            // Fade out
            for (float t = 0; t < pickupChimeFadeDuration; t += Time.deltaTime)
            {
                pickupChimePanel.alpha = 1 - (t / pickupChimeFadeDuration);
                yield return null;
            }
            pickupChimePanel.alpha = 0;
        }

        isShowingChime = false;
    }

}

