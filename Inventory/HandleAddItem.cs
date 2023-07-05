using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleAddItem : MonoBehaviour
{
    public enum ItemType
    {
        Gold,
        Item
    }
    public ItemType typeToAdd;
    public InventoryItem item;
    public int goldAmount;
    public float interactionRadius = 2f;
    private PlayerInventory playerInventory;
    private GameObject playerObject;
    private bool clicked;
    private bool itemAdded;

    private void Start()
    {
        // Find the player GameObject
        playerObject = GameObject.FindGameObjectWithTag("Player");

        // Ensure playerInventory is assigned
        if (!playerInventory)
        {
            playerInventory = playerObject.GetComponent<PlayerInventory>();
        }
    }

    private void Update()
    {
        if(!clicked || itemAdded)
            return;

        // Calculate the distance between the player and the item
        float distance = Vector3.Distance(playerObject.transform.position, transform.position);

        // If the player is within the interaction radius, add the item to the inventory
        if (distance <= interactionRadius)
        {
            itemAdded = true;
            switch (typeToAdd)
            {
                case ItemType.Gold:
                    // play gold sound effect

                    playerInventory.adjustGoldCount(goldAmount);
                    StartCoroutine(DestroyGold());
                    break;
                case ItemType.Item:
                    playerInventory.AddItem(item);
                    break;
                default:
                    break;
            }
        }
    }

    void OnMouseDown()
    {
        clicked = true;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

    private IEnumerator DestroyGold()
    {
        yield return new WaitForSeconds(.25f);
        Destroy(gameObject);
    }

}