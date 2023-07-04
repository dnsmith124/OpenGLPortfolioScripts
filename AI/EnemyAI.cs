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
    private PlayerStats playerStats;
    public float movementSpeed = 10.0f;
    public float chaseRange = 10.0f;
    public float attackRange = 2.2f;
    public int attackDamage = 25;
    [Tooltip("Time before damage takes place in an animation.")]
    public float attackDamageDelay = 1.0f;

    private int currentHealth;
    public int maxHealth = 100;
    public Slider healthBar;
    private GameObject healthBarObject;

    private NavMeshAgent agent;
    //private float attackTimer;
    private Animator animator;
    private Camera mainCamera;
    private CapsuleCollider attackCollider;
    private float initialColliderRadius;

    private bool isAttacking;
    private Coroutine attackCoroutine;

    private void Start()
    {
        attackCollider = GetComponent<CapsuleCollider>();
        attackCollider.enabled = false;
        initialColliderRadius = attackCollider.radius;

        isAttacking = false;

        target = GameObject.FindGameObjectWithTag("Player").transform;
        playerStats = target.GetComponent<PlayerStats>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;

        animator = GetComponent<Animator>();

        healthBarObject = healthBar.gameObject;
        mainCamera = Camera.main;
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        healthBarObject.SetActive(false);

        ChangeState(State.Idling);

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
                    if (!isAttacking)
                        HandleAttacking();
                    break;

                case State.Dying:
                    HandleDying();
                    break;
            }
        }
        else
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

    private void ChangeState(State newState)
    {
        if (newState == state)
        {
            return;
        }

        state = newState;

        switch (state)
        {
            case State.Idling:
                animator.SetTrigger("Idling");
                break;
            case State.Walking:
                animator.SetTrigger("Walking");
                break;
            case State.TakingDamage:
                animator.SetTrigger("Damaging");
                break;
            case State.Attacking:
                animator.SetTrigger("Attacking");
                break;
            case State.Dying:
                animator.SetTrigger("Dying");
                break;
        }
    }

    private void HandleDying()
    {
        StartCoroutine(TriggerDyingAnimAndCleanup());
    }

    private void HandleAttacking()
    {
        agent.ResetPath();
        StartCoroutine(TriggerAnim("Attacking", State.Idling));
    }

    private IEnumerator TriggerAnim(string triggerName, State stateToRevertTo)
    {
        isAttacking = true;
        // resize the hitbox
        attackCollider.radius = attackRange;
        // Enable the attack hitbox
        attackCollider.enabled = true;

        // Trigger the animation
        animator.SetTrigger(triggerName);

        // get the length of the triggered animation
        float animationLength = AnimationUtils.GetAnimationClipLength(animator, triggerName);

        // Wait for the animation to finish
        yield return new WaitForSeconds(animationLength);

        // Disable the attack hitbox
        attackCollider.enabled = false;

        // reset the hitbox size
        attackCollider.radius = initialColliderRadius;

        // Set state to Idling
        ChangeState(stateToRevertTo);
        isAttacking = false;

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && playerStats)
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


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
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

        // If the player is still within range, apply damage
        if (Vector3.Distance(target.position, transform.position) <= attackRange)
        {
            Debug.Log("Hit");
            playerStats.adjustHealth(-attackDamage);
        }
    }

    private void HandleIdling(float distanceToTarget)
    {
        if (distanceToTarget <= chaseRange && distanceToTarget > attackRange)
        {
            ChangeState(State.Walking);
            return;
        }
        if (distanceToTarget <= attackRange)
        {
            ChangeState(State.Attacking);
            return;
        }
    }

    private void HandleWalking(float distanceToTarget)
    {
        if (distanceToTarget <= attackRange)
        {
            ChangeState(State.Attacking);
            return;
        }

        if (distanceToTarget >= chaseRange)
        {
            agent.ResetPath();
            ChangeState(State.Idling);
            return;
        }

        agent.SetDestination(target.position);
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
            ChangeState(State.Dying);
        }
        else
        {
            animator.SetTrigger("Damaging");
            ChangeState(State.Idling);
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
