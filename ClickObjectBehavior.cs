using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickObjectBehavior : MonoBehaviour
{
    public float timeTilDestroy = 2f;

    void Update()
    {
        Destroy(gameObject, timeTilDestroy);
    }
}
