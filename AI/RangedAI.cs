using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAI : EnemyAI
{
    public Rigidbody projectilePrefab;
    public float projectileInstantiateHeightOffset;

    protected override IEnumerator TriggerAttack(string animString)
    {
        // Find the vector pointing from the GameObject to the hit point
        Vector3 targetDirection = target.position - transform.position;
        targetDirection.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // adjust this to smooth turn later
        transform.rotation = targetRotation;

        isAttacking = true;

        // Trigger the animation
        animator.SetTrigger(animString);

        // get the length of the triggered animation
        float animationLength = AnimationUtils.GetAnimationClipLength(animator, animString);

        yield return new WaitForSeconds(attackDamageDelay);

        if(!isFrozen)
            ShootProjectile(targetDirection);

        // Wait for the animation to finish
        yield return new WaitForSeconds(animationLength - attackDamageDelay);

        // Check distance to player
        float distanceToTarget = Vector3.Distance(target.position, transform.position);

        State stateToRevertTo = State.Idling;
        if (distanceToTarget <= chaseRange && distanceToTarget > attackRange)
        {
            // If player is within chase range but outside attack range, start walking
            stateToRevertTo = State.Walking;
        }

        // Set state
        ChangeState(stateToRevertTo);
        isAttacking = false;
    }

    public void ShootProjectile(Vector3 targetDirection)
    {
        // Create a new instance
        Rigidbody projectileInstance;

        float relativeYCoordinate = transform.position.y + projectileInstantiateHeightOffset;

        // Instantiate the spell at enemy's position and rotation
        Vector3 adjustedTransformPosition = new Vector3(transform.position.x, relativeYCoordinate, transform.position.z);
        projectileInstance = Instantiate(projectilePrefab, adjustedTransformPosition, transform.rotation);

        targetDirection.Normalize();

        projectileInstance.GetComponent<ArrowProjectileBehavior>().Setup(targetDirection, attackDamage);

    }

}
