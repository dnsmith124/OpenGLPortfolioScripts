using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Idling,
        Walking,
        Attacking,
        TakingDamage,
        Dying
    }

    public State state;
    public Transform target;
    public float chaseRange = 10.0f;
    public float attackRange = 2.2f;
    public float attackDelay = 1.0f;

    private int currentHealth;
    public int maxHealth = 100;
    public Slider healthBar;
    private GameObject healthBarObject;

    private NavMeshAgent agent;
    private float attackTimer;
    private Animator animator;
    private Camera mainCamera;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        state = State.Idling;
        animator = GetComponent<Animator>();

        healthBarObject = healthBar.gameObject;
        mainCamera = Camera.main;
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        healthBarObject.SetActive(false);
    }

    private void Update()
    {
        if (GameController.Instance.isAIEnabled)
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

                case State.Dying:
                    HandleDying();
                    break;
            }
        } else
        {
            switch (state)
            {
                case State.Idling:
                    HandleIdling(chaseRange + 1.0f);
                    break;

                case State.Dying:
                    HandleDying();
                    break;
            }
        }
    }

    private void LateUpdate()
    {
        // Make the health bar face the camera
        healthBarObject.transform.LookAt(healthBarObject.transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }

    private void HandleDying()
    {
        StartCoroutine(TriggerDyingAnimAndCleanup());
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

    public void TakeDamage(int damage)
    {
        agent.ResetPath();
        currentHealth -= damage;
        healthBar.value = currentHealth;

        if (currentHealth < maxHealth)
        {
            healthBarObject.SetActive(true);
        }

        if (currentHealth <= 0)
        {
            state = State.Dying;
        }
        else
        {
            animator.SetTrigger("Damaging");
            state = State.Idling;
        }
    }

    private IEnumerator TriggerDyingAnimAndCleanup()
    {
        // get the length of the triggered animation
        float animationLength = AnimationUtils.GetAnimationClipLength(animator, "Death");
        animator.SetTrigger("Dying");

        // Wait for the animation to finish
        yield return new WaitForSeconds(animationLength);

        // Insert some sort of fade or something here

        // Let the body lie there for the duration of the animation
        yield return new WaitForSeconds(animationLength);

        // Insert Drop code here (gold, xp gain etc)

        Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
