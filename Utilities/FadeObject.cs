using System;
using System.Collections;
using UnityEngine;

public class FadeObject : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void StartFade(float duration, Action callback)
    {
        StartCoroutine(Fade(duration, callback));
    }

    private IEnumerator Fade(float duration, Action callback)
    {
        float startTime = Time.time;
        Color originalColor = meshRenderer.material.color;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(originalColor.a, 0, t));
            meshRenderer.material.color = newColor;
            yield return null; 
        }

        // ensure the alpha of the meshRenderer's color to 0
        meshRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        // Call the callback function when the fade is completed
        callback?.Invoke(); 

    }
}
