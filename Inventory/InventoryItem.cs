using UnityEngine;

public enum ItemType
{
    Helm,
    Weapon,
    Armor,
    Ring,
    Potion,
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public int value;
    public ItemType type; 
    public Sprite itemImage; 
    public string description;

    public int manaBoost;
    public int healthBoost;
    public int charmBoost;
    public int spellPowerBoost;

    public InventoryItem(string itemName, int value, ItemType type, Sprite itemImage, string description)
    {
        this.itemName = itemName;
        this.value = value;
        this.type = type;
        this.itemImage = itemImage;
        this.description = description;
    }
}
