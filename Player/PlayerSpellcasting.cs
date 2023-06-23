using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellCasting : MonoBehaviour
{
    public Rigidbody spellPrefab;
    public float spellSpeed = 20f;
    [Tooltip("Determines the y coordinate for an instantiated spell prefab. An offset to the player's y coordinate;")]
    public float spellInstantiateHeightOffset = 1.0f;

    public void CastSpell()
    {
        // Create a new spell instance
        Rigidbody spellInstance;

        float relativeYCoordinate = transform.position.y + spellInstantiateHeightOffset;
        // Instantiate the spell at player's position and rotation
        Vector3 adjustedTransformPosition = new Vector3(transform.position.x, relativeYCoordinate, transform.position.z);
        spellInstance = Instantiate(spellPrefab, adjustedTransformPosition, transform.rotation);

        // Get the direction from the player to the mouse cursor
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f)) 
        {
            // Calculate the direction from the player to the hit point
            Vector3 direction = hit.point - transform.position;
            direction.Normalize();

            // Shoot the spell in the direction of the hit point
            spellInstance.velocity = direction * spellSpeed;
        }
    }
}
