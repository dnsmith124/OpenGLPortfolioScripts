using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingHitbox : MonoBehaviour
{

    private int attackDamage;
    private PlayerStats playerStats;
    private Transform target;
    private Coroutine attackCoroutine;
    private float attackDamageDelay;
    private float attackRange;

    public void KillAttackingCoroutine(bool death)
    {
        if(attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        if (death)
            target = null;
    }

    public void initAttackingHitbox(int damage, PlayerStats playerStats, Transform playerTransform, float attackDamageDelay, float attackRange)
    {
        attackDamage = damage;
        this.playerStats = playerStats;
        target = playerTransform;
        this.attackDamageDelay = attackDamageDelay;
        this.attackRange = attackRange;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (target != null && col.gameObject.CompareTag("Player") && playerStats)
        {
            // If a previous attack coroutine is still running, stop it
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }

            // Start a new attack coroutine
            attackCoroutine = StartCoroutine(DealDamageAfterDelay(attackDamageDelay));
        }
    }


    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // If an attack coroutine is still running, stop it
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }


    private IEnumerator DealDamageAfterDelay(float delay)
    {

        yield return new WaitForSeconds(delay);

        // If the player is still within range (plus a little leeway, apply damage)
        if (Vector3.Distance(target.position, transform.position) <= attackRange + 0.25f)
        {
            playerStats.adjustHealth(-attackDamage);
        }
    }
}
