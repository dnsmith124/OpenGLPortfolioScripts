using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public enum State
    {
        Idling,
        Walking,
        Attacking,
        AttackOne
    }

    public State state;
    public float speed = 3.5f;
    [Tooltip("Determines how fast the player can rotate while walking.")]
    public float rotationSpeed = 10f;
    [Tooltip("Determines how fast the player rotates to the target of a spell cast when pressed.")]
    public float spellCastRotationSpeed = 10f;
    [Tooltip("How long to wait during the casting animation before the projectile is initialized. Seconds.")]
    public float spellCastAnimOffsetTime = 0.4f;

    private bool canMove = true;
    private bool spellCooldown = false;
    private NavMeshAgent agent;
    private Camera _mainCamera;
    private Vector3 targetPoint;
    private Animator animator;
    private PlayerSpellCasting playerSpellCasting;
    private RaycastHit lastSpellTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.angularSpeed = 0; // Disable automatic rotation
        _mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        playerSpellCasting = GetComponent<PlayerSpellCasting>();
    }

    void Update()
    {
        if (!canMove)
        {
            agent.SetDestination(gameObject.transform.position);
            return;
        }

        if(Input.GetButtonDown("Fire2"))
        {
            if (spellCooldown)
            {
                return;
            }

            RaycastHit newSpellTarget = GetMousePositionFromRayCast();
            if (newSpellTarget.collider.gameObject.tag == "NPC")
                return;

            lastSpellTarget = newSpellTarget;
            agent.ResetPath();
            state = State.AttackOne;
            return;
        }


        if (Input.GetButton("Fire1"))
        {
            RaycastHit hit = GetMousePositionFromRayCast();
            agent.destination = hit.point;
            targetPoint = hit.point;
        }

        switch (state)
        {
            case State.Idling:
                HandleIdling();
                break;

            case State.Walking:
                HandleWalking();
                break;

            case State.AttackOne:
                if (spellCooldown)
                    return;
                HandleAttackOne();
                break;

            case State.Attacking:
                HandleAttacking();
                break;
        }


    }

    private RaycastHit GetMousePositionFromRayCast()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit;
        }

        return new RaycastHit();
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    private void HandleIdling()
    {
        animator.SetTrigger("Idling");
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            state = State.Walking;
        }
    }

    private void HandleWalking()
    {
        animator.SetTrigger("Walking");
        // If the agent is moving
            if (agent.remainingDistance > agent.stoppingDistance)
        {
            // Calculate the direction to the target point
            Vector3 directionToTarget = targetPoint - transform.position;

            // Ignore Y axis difference
            directionToTarget.y = 0;

            // If we have a direction (might be zero if the target is exactly at our position)
            if (directionToTarget.sqrMagnitude > 0)
            {
                // Create the target rotation
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

                // Smoothly interpolate towards the target rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            agent.ResetPath();
            state = State.Idling;
        }
    }

    private void HandleAttackOne()
    {
        spellCooldown = true;
        StartCoroutine(RotateAndCastSpell(lastSpellTarget, spellCastRotationSpeed, "AttackOne", spellCastAnimOffsetTime));
    }

    private void HandleAttacking()
    {

    }

    private IEnumerator RotateAndCastSpell(RaycastHit targetHit, float rotationSpeed, string triggerName, float animOffsetTime)
    {
        // Find the vector pointing from the GameObject to the hit point
        Vector3 targetDirection = targetHit.point - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection - transform.position);

        // rotate until we're within 3 degrees of the target
        while (Quaternion.Angle(transform.rotation, targetRotation) > 3f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }

        // Trigger the animation
        animator.SetTrigger(triggerName);

        // get the length of the triggered animation
        float animationLength = AnimationUtils.GetAnimationClipLength(animator, triggerName);

        // Wait for the specified time in the animation to cast the spell.
        yield return new WaitForSeconds(animOffsetTime);
        playerSpellCasting.CastSpell(targetDirection);

        // Wait for the animation to finish
        yield return new WaitForSeconds(animationLength - animOffsetTime);

        // Set state to Idling and cooldown to false
        state = State.Idling;
        spellCooldown = false;
    }
}