using UnityEngine;
using UnityEngine.UI;

public class SmoothColorFade : MonoBehaviour
{

    public float colorChangeSpeed = 1.0f;
    public Color colorOne = Color.red;
    public Color colorTwo = Color.blue;


    private Color newColor;
    private Image imageComponent;

    private void Start()
    {
        imageComponent = GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogError("No Image component attached to this GameObject");
        }
    }

    void Update()
    {
        newColor = Color.Lerp(colorOne, colorTwo, Mathf.PingPong(Time.time, colorChangeSpeed));
        imageComponent.color = newColor;
    }
   
}