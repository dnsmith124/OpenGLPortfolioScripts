using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellCasting : MonoBehaviour
{
    public Rigidbody projectilePrefab;
    public ParticleSystem aoePrefab;
    public ParticleSystem blizzardPrefab;
    [Tooltip("Determines the y coordinate for an instantiated spell prefab. An offset to the player's y coordinate;")]
    public float spellInstantiateHeightOffset = 1.0f;

    public float aoeSpellRadius = 5f;
    public int aoeSpellDamageMult = 10;
    public int aoeSpellCost = 30;

    public float blizzardSpellRadius = 5f;
    public int blizzardSpellDamageMult = 1;
    public int blizzardSpellCost = 30;
    public float blizzardSpellDuration = 5f;

    public int projSpellDamageMult = 3;
    public int projectileSpellCost = 10;
    public int baseSpellDamage = 10;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    public void CastProjectileSpell(Vector3 targetDirection)
    {
        // Create a new spell instance
        Rigidbody spellInstance;

        float relativeYCoordinate = transform.position.y + spellInstantiateHeightOffset;
        // Instantiate the spell at player's position and rotation
        Vector3 adjustedTransformPosition = new Vector3(transform.position.x, relativeYCoordinate, transform.position.z);
        spellInstance = Instantiate(projectilePrefab, adjustedTransformPosition, transform.rotation);

        targetDirection.Normalize();

        int currentSpellPower = playerStats.GetSpellPower();
        int damage = CalculateSpellDamage(currentSpellPower, projSpellDamageMult, baseSpellDamage);

        spellInstance.GetComponent<ProjectileSpellBehavior>().Setup(targetDirection, damage);

        playerStats.adjustMana(-projectileSpellCost);
    }

    public void CastAoeSpell (Vector3 center)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, aoeSpellRadius);
        Instantiate(aoePrefab, transform.position, transform.rotation);
        foreach (var hitCollider in hitColliders)
        {
            
            if (hitCollider.gameObject.GetComponent<ReceivingHitbox>())
            {
                hitCollider.GetComponentInParent<EnemyAI>().Freeze(3f);
            }
        }
        playerStats.adjustMana(-aoeSpellCost);
    }

    public void CastBlizzardSpell(Vector3 mousePos)
    {
        ParticleSystem blizzard = Instantiate(blizzardPrefab, mousePos, transform.rotation);
        BlizzardSpellBehavior blizz = blizzard.GetComponent<BlizzardSpellBehavior>();
        blizz.radius = blizzardSpellRadius;
        blizz.lifetime = blizzardSpellDuration;

        int currentSpellPower = playerStats.GetSpellPower();
        int damage = CalculateSpellDamage(currentSpellPower, blizzardSpellDamageMult, baseSpellDamage);

        playerStats.adjustMana(-blizzardSpellCost);

        StartCoroutine(BlizzardDamageCoroutine(mousePos, damage, blizzardSpellDuration));
    }

    private IEnumerator BlizzardDamageCoroutine(Vector3 mousePos, int damage, float duration)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            Debug.Log("Tick");
            Collider[] hitColliders = Physics.OverlapSphere(mousePos, blizzardSpellRadius);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.GetComponent<ReceivingHitbox>())
                {
                    hitCollider.GetComponentInParent<EnemyAI>().TakeDamage(damage);
                }
            }

            elapsed += 1.0f;
            yield return new WaitForSeconds(1.0f); 
        }
    }

    private int CalculateSpellDamage(int spellPower, int spellDamageMult, int baseDamage)
    {
        return ((baseDamage * spellDamageMult) + (spellPower / 2));
    }
}
