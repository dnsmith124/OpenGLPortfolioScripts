using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlizzardSpellBehavior : MonoBehaviour
{
    public float radius = 1f;
    public float lifetime = 5f;

    private void Update()
    {
        Destroy(gameObject, lifetime);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
