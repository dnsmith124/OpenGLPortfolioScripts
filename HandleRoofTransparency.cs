using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleRoofTransparency : MonoBehaviour
{
    private Roof roofChildObject;
    private MeshRenderer[] roofMeshRenderers;

    private void Start()
    {
        roofChildObject = GetComponentInChildren<Roof>();
        roofMeshRenderers = roofChildObject.GetComponentsInChildren<MeshRenderer>();
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            foreach (var meshRenderer in roofMeshRenderers)
            {
                if(meshRenderer.enabled)
                    meshRenderer.enabled = false;
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            foreach (var meshRenderer in roofMeshRenderers)
            {
                meshRenderer.enabled = true;
            }
        }
    }
}
