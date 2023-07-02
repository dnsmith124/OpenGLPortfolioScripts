using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellCasting : MonoBehaviour
{
    public Rigidbody spellPrefab;
    public float spellSpeed = 20f;
    [Tooltip("Determines the y coordinate for an instantiated spell prefab. An offset to the player's y coordinate;")]
    public float spellInstantiateHeightOffset = 1.0f;

    public void CastSpell(Vector3 targetDirection)
    {
        // Create a new spell instance
        Rigidbody spellInstance;

        float relativeYCoordinate = transform.position.y + spellInstantiateHeightOffset;
        // Instantiate the spell at player's position and rotation
        Vector3 adjustedTransformPosition = new Vector3(transform.position.x, relativeYCoordinate, transform.position.z);
        spellInstance = Instantiate(spellPrefab, adjustedTransformPosition, transform.rotation);

        targetDirection.Normalize();

        // Shoot the spell in the direction of the hit point
        spellInstance.velocity = targetDirection * spellSpeed;
    }
}
