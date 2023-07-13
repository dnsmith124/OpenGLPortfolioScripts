using System.Collections.Generic;
using UnityEngine;

public class EnemyPackInit : MonoBehaviour
{
    public float initializationRadius = 20.0f;
    private List<GameObject> enemies;
    private SphereCollider sphereCollider;

    void Start()
    {
        enemies = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTransform = transform.GetChild(i);
            enemies.Add(childTransform.gameObject);
        }
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = initializationRadius;
    }

    private void OnTriggerEnter(Collider col)
    {

        if(col.GetComponent<PlayerController>())
        {
            Debug.Log("Player entered");
            foreach (GameObject enemy in enemies)
            {
                enemy.SetActive(true); 
            }
        }
    }
}
