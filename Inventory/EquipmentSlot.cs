using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ModelShark;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    public ItemType type;
    [SerializeField]
    private InventoryItem _storedItem;
    public InventoryItem StoredItem
    {
        get { return _storedItem; }
        set { _storedItem = value; }
    }
    private Image itemImage;
    private PlayerInventory playerInventory;
    private InventoryManager inventoryManager;
    private TooltipTrigger tooltipTrigger;

    private void Start()
    {
        itemImage = GetComponentInChildren<EquipmentSlotImage>().GetComponent<Image>();
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        tooltipTrigger = GetComponent<TooltipTrigger>();
        inventoryManager = GameObject.FindGameObjectWithTag("InventoryPanel").GetComponent<InventoryManager>();
        tooltipTrigger.enabled = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        DragDropItem droppedItem = eventData.pointerDrag.GetComponent<DragDropItem>();
        if (droppedItem != null && droppedItem.RepresentedItem.type == type)
        {
            // remove the dropped item from the inventory 
            playerInventory.RemoveItem(droppedItem.RepresentedItem);

            // If there is already an item in this slot
            if (StoredItem != null)
            {
                // move the StoredItem back to the inventory
                playerInventory.AddItem(StoredItem);
                // remove the previously equipped item
                playerInventory.RemoveEquippedItem(StoredItem);
            }

            itemImage.sprite = droppedItem.RepresentedItem.itemImage;
            itemImage.color = Color.white;

            // Store the dropped item in this slot
            StoredItem = droppedItem.RepresentedItem;
            // add item to equipped items list
            playerInventory.AddEquippedItem(StoredItem);

            tooltipTrigger.SetText("BodyText", StoredItem.description);


            string boostText = inventoryManager.CalculateBoostText(StoredItem);
            tooltipTrigger.SetText("BoostText", boostText);
            tooltipTrigger.enabled = true;

        }
    }
}
