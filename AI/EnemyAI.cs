using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Idling,
        Walking,
        Attacking
    }

    public State state;
    public Transform target;
    public float chaseRange = 10.0f;
    public float attackRange = 2.2f;
    public float attackDelay = 1.0f;

    private NavMeshAgent agent;
    private float attackTimer;
    private Animator animator;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        state = State.Idling;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float distanceToTarget = Vector3.Distance(target.position, transform.position);
        switch (state)
        {
            case State.Idling:
                HandleIdling(distanceToTarget);
                break;

            case State.Walking:
                HandleWalking(distanceToTarget);
                break;

            case State.Attacking:
                HandleAttacking(distanceToTarget);
                break;
        }
    }

    private void HandleAttacking(float distanceToTarget)
    {
        agent.ResetPath();
        animator.SetTrigger("Attacking");
        if (attackTimer >= attackDelay)
        {
            // Insert Attack code here.
            // Make sure to reset the attack timer after attacking.
            attackTimer = 0;
        }
        attackTimer += Time.deltaTime;
        if (distanceToTarget > attackRange)
        {
            state = State.Walking;
        }
    }

    private void HandleIdling(float distanceToTarget)
    {
        animator.SetTrigger("Idling");
        if (distanceToTarget <= chaseRange)
        {
            state = State.Walking;
        }
    }

    private void HandleWalking(float distanceToTarget)
    {
        agent.SetDestination(target.position);
        animator.SetTrigger("Walking");
        if (distanceToTarget <= attackRange)
        {
            state = State.Attacking;
        }

        if (distanceToTarget >= chaseRange)
        {
            agent.ResetPath();
            state = State.Idling;
        }
    }
}
