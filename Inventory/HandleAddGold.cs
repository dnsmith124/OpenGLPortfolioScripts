using System.Collections;
using UnityEngine;

public class HandleAddGold : MonoBehaviour
{
    public int goldAmount;

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<PlayerInventory>())
        {
            // play gold pickup sound
            col.gameObject.GetComponent<PlayerInventory>().adjustGoldCount(goldAmount);
            StartCoroutine(DestroyGold());
        }    
    }

    private IEnumerator DestroyGold()
    {
        yield return new WaitForSeconds(.25f);
        Destroy(gameObject);
    }
}
