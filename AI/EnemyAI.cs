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
        Dying,
        Frozen
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
    public float randomDropOffset = 2.0f;

    private int currentHealth;
    public int maxHealth = 100;
    public Slider healthBar;
    private GameObject healthBarObject;

    private NavMeshAgent agent;
    protected Animator animator;
    private Camera mainCamera;
    private CapsuleCollider attackCollider;
    private AttackingHitbox attackingHitbox;
    private CapsuleCollider receivingCollider;
    private ReceivingHitbox receivingHitbox;
    private float initialColliderRadius;

    private float frozenTime;
    private FreezeController freezeController;
    private State preFrozenState;

    private DropManager dropManager;

    protected bool isAttacking;
    protected bool isFrozen;
    private bool isDying;

    private void Start()
    {
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

        attackingHitbox = gameObject.GetComponentInChildren<AttackingHitbox>();
        attackingHitbox.initAttackingHitbox(attackDamage, playerStats, target, attackDamageDelay, attackRange);
        attackCollider = attackingHitbox.GetComponent<CapsuleCollider>();
        attackCollider.enabled = false;
        initialColliderRadius = attackCollider.radius;

        receivingHitbox = gameObject.GetComponentInChildren<ReceivingHitbox>();
        receivingCollider = receivingHitbox.GetComponent<CapsuleCollider>();

        dropManager = GetComponent<DropManager>();

        isAttacking = false;
        isFrozen = false;
        isDying = false;

        freezeController = GetComponentInChildren<FreezeController>();
    }

    private void Update()
    {
        if(isDying)
        {
            HandleDying();
            return;
        }

        if (GameController.Instance.isAIEnabled)
        {
            float distanceToTarget = Vector3.Distance(target.position, transform.position);
            //Debug.Log(state);
            switch (state)
            {
                case State.Idling:
                    HandleIdling(distanceToTarget);
                    break;

                case State.Walking:
                    if (!isFrozen)
                        HandleWalking(distanceToTarget);
                    break;

                case State.TakingDamage:
                    if (!isAttacking && !isFrozen)
                        HandleTakingDamage();
                    break;

                case State.Frozen:
                    if(!isFrozen)
                        HandleFrozen(frozenTime);
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

    protected void ChangeState(State newState)
    {
        if (newState == state)
        {
            return;
        }

        Debug.Log(newState);

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
                if(!isFrozen)
                    animator.SetTrigger("Damaging");
                break;
            case State.Frozen:
                //animator.SetTrigger("Freezing");
                break;
            case State.Attacking:
                //animator trigger handled in HandleAttacking()=>TriggerAttack()
                break;
            case State.Dying:
                animator.SetTrigger("Dying");
                break;
        }
    }

    private void HandleDying()
    {
        agent.ResetPath();
        isDying = true;
        StartCoroutine(TriggerDyingAnimAndCleanup());
    }

    private void HandleAttacking()
    {
        agent.ResetPath();
        StartCoroutine(TriggerAttack("Attacking"));
    }

    protected virtual IEnumerator TriggerAttack(string animString)
    {
        isAttacking = true;
        // resize the hitbox
        attackCollider.radius = attackRange;
        // Enable the attack hitbox
        attackCollider.enabled = true;

        // Trigger the animation
        animator.SetTrigger(animString);

        // get the length of the triggered animation
        float animationLength = AnimationUtils.GetAnimationClipLength(animator, animString);

        // Wait for the animation to finish
        yield return new WaitForSeconds(animationLength);

        // Disable the attack hitbox
        attackCollider.enabled = false;

        // reset the hitbox size
        attackCollider.radius = initialColliderRadius;

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

        //Debug.Log("setting destination");

        agent.SetDestination(target.position);
    }

    private void HandleTakingDamage()
    {
        agent.ResetPath();
        StartCoroutine(TriggerDamage());
    }

    private IEnumerator TriggerDamage()
    {
        // animation triggered in ChangeState above

        // get the length of the triggered animation
        float animationLength = AnimationUtils.GetAnimationClipLength(animator, "Damaging");

        // Wait for the animation to finish
        yield return new WaitForSeconds(animationLength);

        // Check distance to player
        float distanceToTarget = Vector3.Distance(target.position, transform.position);

        chaseRange *= 10.0f;

        State stateToRevertTo = State.Idling;
        if (distanceToTarget <= chaseRange && distanceToTarget > attackRange)
        {
            // If player is within chase range but outside attack range, start walking
            stateToRevertTo = State.Walking;
        }
        if (distanceToTarget <= attackRange)
        {
            // If player is within chase range but outside attack range, start walking
            stateToRevertTo = State.Attacking;
        }

        // Set state
        ChangeState(stateToRevertTo);

    }

    public void TakeDamage(int damage)
    {
        agent.ResetPath();
        attackingHitbox.KillAttackingCoroutine(false);

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
            ChangeState(State.TakingDamage);
        }
    }

    public void Freeze(float time)
    {
        Debug.Log("Freeze");
        attackingHitbox.KillAttackingCoroutine(false);
        frozenTime = time;
        preFrozenState = state;
        ChangeState(State.Frozen);
        chaseRange *= 10.0f;
    }

    private void HandleFrozen(float time)
    {
        agent.ResetPath();
        attackingHitbox.KillAttackingCoroutine(false);
        StartCoroutine(TriggerFrozen(time));
    }

    private IEnumerator TriggerFrozen(float time)
    {
        isFrozen = true;

        freezeController.AssignFrozenMaterial();
        float initialSpeed = animator.speed;
        animator.speed = 0;
        yield return new WaitForSeconds(time);
        freezeController.RevertMaterial();
        ChangeState(preFrozenState);
        Debug.Log($"Revert to state {preFrozenState}");
        isFrozen = false;
        animator.speed = initialSpeed;
    }

    private IEnumerator TriggerDyingAnimAndCleanup()
    {
        attackCollider.enabled = false;
        receivingCollider.enabled = false;
        healthBarObject.SetActive(false);
        attackingHitbox.KillAttackingCoroutine(true);

        // get the length of the triggered animation
        float animationLength = AnimationUtils.GetAnimationClipLength(animator, "Death");
        animator.SetTrigger("Dying");

        // Wait for the animation to finish
        yield return new WaitForSeconds(animationLength);

        // Insert some sort of fade or something here

        // Let the body lie there for a moment
        yield return new WaitForSeconds(0.5f);

        // Insert Drop code here (gold, xp gain etc)
        dropManager.GenerateDrops();

        Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
