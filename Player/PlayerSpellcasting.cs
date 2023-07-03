using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellCasting : MonoBehaviour
{
    public Rigidbody projectilePrefab;
    public ParticleSystem aoePrefab;
    [Tooltip("Determines the y coordinate for an instantiated spell prefab. An offset to the player's y coordinate;")]
    public float spellInstantiateHeightOffset = 1.0f;
    public float aoeSpellRadius = 5f;
    public int aoeSpellDamage = 10;
    public int projectileSpellDamage = 10;


    public void CastProjectileSpell(Vector3 targetDirection)
    {
        // Create a new spell instance
        Rigidbody spellInstance;

        float relativeYCoordinate = transform.position.y + spellInstantiateHeightOffset;
        // Instantiate the spell at player's position and rotation
        Vector3 adjustedTransformPosition = new Vector3(transform.position.x, relativeYCoordinate, transform.position.z);
        spellInstance = Instantiate(projectilePrefab, adjustedTransformPosition, transform.rotation);

        targetDirection.Normalize();

        spellInstance.GetComponent<ProjectileSpellBehavior>().Setup(targetDirection, projectileSpellDamage);

    }

    public void CastAoeSpell (Vector3 center)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, aoeSpellRadius);
        Instantiate(aoePrefab, transform);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.GetComponent<EnemyAI>())
            {
                // This assumes your enemy script has a function called TakeDamage
                hitCollider.gameObject.GetComponent<EnemyAI>().TakeDamage(aoeSpellDamage);
            }
        }
    }
}
