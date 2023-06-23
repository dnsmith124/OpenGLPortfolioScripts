using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasGroupController : MonoBehaviour, IPointerClickHandler
{
    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(eventData);
        if (canvasGroup.interactable)
        {
            // do something here when you click the canvas
        }
        else
        {
            // do something here when you click the canvas but it's not interactable
        }
    }
}