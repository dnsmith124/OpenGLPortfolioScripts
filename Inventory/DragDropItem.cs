using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventoryItem RepresentedItem { get; set; }

    public CanvasGroup canvasGroup;
    private GameObject placeholder; 
    private Vector3 originalPosition;
    private Transform originalParent;
    private GameObject inventoryContainer;
    private InventoryManager inventoryManager;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        inventoryContainer = GameObject.FindGameObjectWithTag("InventoryContainer");
        inventoryManager = GameObject.FindGameObjectWithTag("InventoryPanel").GetComponent<InventoryManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;

        // Create a new placeholder
        placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(transform.parent);
        placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());

        // Give the placeholder the same layout settings as the item
        LayoutElement layoutElement = placeholder.AddComponent<LayoutElement>();
        LayoutElement thisLayoutElement = GetComponent<LayoutElement>();
        layoutElement.preferredWidth = thisLayoutElement.preferredWidth;
        layoutElement.preferredHeight = thisLayoutElement.preferredHeight;
        layoutElement.flexibleWidth = 0;
        layoutElement.flexibleHeight = 0;

        originalPosition = transform.position;
        originalParent = transform.parent;

        // Make the dragged item a child of the InventoryContainer's transform
        transform.SetParent(inventoryContainer.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the dragged item with the cursor
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        Destroy(placeholder);

        // Check if the item was dropped onto an equipment slot
        EquipmentSlot slot = eventData.pointerCurrentRaycast.gameObject?.GetComponent<EquipmentSlot>();
        if (slot != null && RepresentedItem.type == slot.type)
        {
            // if it was, destroy the dropped game object
            Destroy(gameObject);
        }
        else
        {
            // if the item was not equipped, so return it to its original slot
            transform.position = originalPosition;
            transform.SetParent(originalParent);
        }

        inventoryManager.UpdateUI();
    }

}
