using UnityEngine;

public class DropManager : MonoBehaviour
{

    public Drop[] possibleDrops; // Array of possible drops, set in the inspector
    public float dropRadius = 1.0f; // Radius within which the items will drop

    // Call this method when you want to generate drops
    public void GenerateDrops()
    {
        foreach (Drop uniDrop in UniversalDropsData.Instance.universalDrops)
        {
            if (Random.value < uniDrop.dropRate)
            {
                // If the item is chosen to drop, instantiate it at a random position within the drop radius
                Vector3 dropPosition = transform.position + Random.insideUnitSphere * dropRadius;
                dropPosition.y = transform.position.y; // keep the drop at the same height as the original object
                Instantiate(uniDrop.item, dropPosition, Quaternion.identity);
            }
        }
        foreach (Drop drop in possibleDrops)
        {
            if (Random.value < drop.dropRate)
            {
                // If the item is chosen to drop, instantiate it at a random position within the drop radius
                Vector3 dropPosition = transform.position + Random.insideUnitSphere * dropRadius;
                dropPosition.y = transform.position.y; // keep the drop at the same height as the original object
                Instantiate(drop.item, dropPosition, Quaternion.identity);
            }
        }
    }
}
