using UnityEngine;

[CreateAssetMenu(fileName = "UniversalDropsData", menuName = "DropManager/UniversalDropsData", order = 1)]
public class UniversalDropsData : ScriptableObject
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

}
