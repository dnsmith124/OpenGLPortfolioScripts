using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeSpellBehavior : MonoBehaviour
{
    public float lifeTime = 5f;
    void Start()
    {
        
    }

    private void Update()
    {
        Destroy(gameObject, lifeTime);
    }
}
