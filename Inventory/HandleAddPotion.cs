using System.Collections;
using UnityEngine;

public class HandleAddPotion : MonoBehaviour
{
    public enum Type
    {
        Health,
        Mana
    }
    public Type potionType;

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<PlayerInventory>())
        {
            // play pickup sound
            switch (potionType)
            {
                case Type.Health:
                    col.gameObject.GetComponent<PlayerInventory>().adjustHealthPotionCount(1);
                    break;
                case Type.Mana:
                    col.gameObject.GetComponent<PlayerInventory>().adjustManaPotionCount(1);
                    break;
                default:
                    break;
            }
            StartCoroutine(DestroyPotion());
        }    
    }

    private IEnumerator DestroyPotion()
    {
        yield return new WaitForSeconds(.25f);
        Destroy(gameObject);
    }
}
