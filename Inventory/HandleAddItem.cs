using System.Collections;
using ModelShark;
using UnityEngine;

public class HandleAddItem : MonoBehaviour
{
    public InventoryItem item;

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.GetComponent<PlayerInventory>())
        {
            col.gameObject.GetComponent<PlayerInventory>().AddItem(item);
            StartCoroutine(DestroyItem());
        }
    }

    private IEnumerator DestroyItem()
    {
        yield return new WaitForSeconds(.25f);
        Destroy(gameObject);
    }
}