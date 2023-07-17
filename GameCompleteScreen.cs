using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCompleteScreen : MonoBehaviour
{
    public static GameCompleteScreen Instance { get; private set; }

    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Ensure the UI starts out invisible
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    public void HandleFade()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float newAlpha = elapsedTime / fadeDuration;
            canvasGroup.alpha = newAlpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(3f);

        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float newAlpha = 1f - (elapsedTime / fadeDuration);
            canvasGroup.alpha = newAlpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
