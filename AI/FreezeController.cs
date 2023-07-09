using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeController : MonoBehaviour
{

    public Material frozenMaterial;
    private Material originalMaterial;
    private SkinnedMeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        originalMaterial = meshRenderer.material;
    }

    public void AssignFrozenMaterial()
    {
        meshRenderer.material = frozenMaterial;
    }

    public void RevertMaterial()
    {
        meshRenderer.material = originalMaterial;
    }
}
