using UnityEngine;

public class UniversalDropsData : MonoBehaviour
{
    public static UniversalDropsData Instance;
    public Drop[] universalDrops; 

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogError("Multiple instances of UniversalDropsData. Only one instance is allowed");
        }
    }

    public Drop[] GetUniversalDrops()
    {
        return universalDrops;
    }

}
