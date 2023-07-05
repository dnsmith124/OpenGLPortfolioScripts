using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public int value;
    public ItemType type; // Now using the ItemType enum
    public Sprite itemImage; // The sprite for the item's image
    public string description; // Description of the item

    public enum ItemType
    {
        Weapon,
        Potion,
        Armor,
        Ring,
    }

    public InventoryItem(string itemName, int value, ItemType type, Sprite itemImage, string description)
    {
        this.itemName = itemName;
        this.value = value;
        this.type = type;
        this.itemImage = itemImage;
        this.description = description;
    }
}
