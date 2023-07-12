using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public enum State
    {
        Idling,
        Walking,
        Attacking,
        AttackOne,
        AttackTwo
    }

    public State state;
    public float speed = 3.5f;
    [Tooltip("Determines how fast the player can rotate while walking.")]
    public float rotationSpeed = 10f;
    [Tooltip("How long to wait during the casting animation before the projectile is initialized. Seconds.")]
    public float projectileSpellCastAnimOffsetTime = 0.4f;
    [Tooltip("How long to wait during the casting animation before the aoe is initialized. Seconds.")]
    public float aoeSpellCastAnimOffsetTime = 0.25f;
    public int healthPotionHealingAmount = 25;
    public int manaPotionHealingAmount = 25;
    public Image IceBoltButtonImage;
    public Image FrostNovaButtonImage;
    public GameObject clickAnimPrefab;


    private bool canMove = true;
    private bool spellCooldown = false;
    private NavMeshAgent agent;
    private Camera _mainCamera;
    private Vector3 targetPoint;
    private Animator animator;
    private PlayerSpellCasting playerSpellCasting;
    private RaycastHit lastSpellTarget;
    private PlayerStats playerStats;
    private PlayerInventory playerInventory;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.angularSpeed = 0; // Disable automatic rotation
        _mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        playerSpellCasting = GetComponent<PlayerSpellCasting>();
        playerStats = GetComponent<PlayerStats>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        if (canMove)
        {
            if (Input.GetButtonDown("HealthPotion"))
            {
                if (playerInventory.getHealthPotionCount() > 0 && playerStats.currentHealth < playerStats.maxHealth)
                {
                    playerInventory.adjustHealthPotionCount(-1);
                    playerStats.adjustHealth(healthPotionHealingAmount);
                    // play health potion sound
                } else
                {
                    playerInventory.FlashPotion(true);
                }
                return;
            }
            if (Input.GetButtonDown("ManaPotion"))
            {
                if (playerInventory.getManaPotionCount() > 0 && playerStats.currentMana < playerStats.maxMana)
                {
                    playerInventory.adjustManaPotionCount(-1);
                    playerStats.adjustMana(manaPotionHealingAmount);
                    // play health potion sound
                }
                else
                {
                    playerInventory.FlashPotion(false);
                }
                return;
            }
            if (Input.GetButtonDown("Fire2"))
            {
                if (playerStats.currentMana < playerSpellCasting.projectileSpellCost)
                {
                    Debug.Log("Not enough mana");
                    StartCoroutine(FlashCoroutine(1f, IceBoltButtonImage));
                    return;
                }
                if (spellCooldown)
                {
                    Debug.Log("CD");
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

            if (Input.GetButtonDown("Fire3"))
            {
                if (playerStats.currentMana < playerSpellCasting.aoeSpellCost)
                {
                    Debug.Log("Not enough mana");
                    StartCoroutine(FlashCoroutine(1f, FrostNovaButtonImage));
                    return;
                }
                if (spellCooldown)
                {
                    Debug.Log("CD");
                    return;
                }


                agent.ResetPath();
                state = State.AttackTwo;
                return;
            }

            if (Input.GetButtonDown("Fire1"))
            {
                RaycastHit hit = GetMousePositionFromRayCast();
                Instantiate(clickAnimPrefab, hit.point, Quaternion.identity);
            }
            if (Input.GetButton("Fire1"))
            {
                RaycastHit hit = GetMousePositionFromRayCast();
                agent.destination = hit.point;
                targetPoint = hit.point;
            }
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

            case State.AttackTwo:
                if (spellCooldown)
                    return;
                HandleAttackTwo();
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

    public void EnterUIMode()
    {
        Debug.Log("Enter UI Mode");
        canMove = false;
        agent.ResetPath();
        state = State.Idling;
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
        StartCoroutine(RotateAndCastProjectileSpell(lastSpellTarget, "AttackOne", projectileSpellCastAnimOffsetTime));
    }

    private void HandleAttackTwo()
    {
        spellCooldown = true;
        StartCoroutine(castAoeSpell("AttackTwo", projectileSpellCastAnimOffsetTime));
    }

    private void HandleAttacking()
    {

    }

    private IEnumerator castAoeSpell(string triggerName, float animOffsetTime)
    {
        FrostNovaButtonImage.color = Color.gray;

        // Trigger the animation
        animator.SetTrigger(triggerName);

        // get the length of the triggered animation
        float animationLength = AnimationUtils.GetAnimationClipLength(animator, triggerName);

        // Wait for the specified time in the animation to cast the spell.
        yield return new WaitForSeconds(animOffsetTime);
        playerSpellCasting.CastAoeSpell(transform.position);

        // Wait for the animation to finish
        yield return new WaitForSeconds(animationLength - animOffsetTime);

        FrostNovaButtonImage.color = Color.white;

        // Set state to Idling and cooldown to false
        state = State.Idling;
        spellCooldown = false;
    }

    private IEnumerator RotateAndCastProjectileSpell(RaycastHit targetHit, string triggerName, float animOffsetTime)
    {

        IceBoltButtonImage.color = Color.gray;

        // Find the vector pointing from the GameObject to the hit point
        Vector3 targetDirection = targetHit.point - transform.position;
        targetDirection.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        transform.rotation = targetRotation;

        // Trigger the animation
        animator.SetTrigger(triggerName);

        // get the length of the triggered animation
        float animationLength = AnimationUtils.GetAnimationClipLength(animator, triggerName);

        // Wait for the specified time in the animation to cast the spell.
        yield return new WaitForSeconds(animOffsetTime);
        playerSpellCasting.CastProjectileSpell(targetDirection);

        // Wait for the animation to finish
        yield return new WaitForSeconds(animationLength - animOffsetTime);

        IceBoltButtonImage.color = Color.white;

        // Set state to Idling and cooldown to false
        state = State.Idling;
        spellCooldown = false;
    }

    IEnumerator FlashCoroutine(float flashTime, Image image)
    {
        // Calculate how long one flash should take (two flashes = flash on and off)
        float singleFlashTime = flashTime / 4f;

        // Do this twice
        for (int i = 0; i < 2; i++)
        {
            // Lerp color to red
            for (float t = 0; t < 1; t += Time.deltaTime / singleFlashTime)
            {
                image.color = Color.Lerp(Color.white, Color.red, t);
                yield return null;
            }

            // Lerp color back to white
            for (float t = 0; t < 1; t += Time.deltaTime / singleFlashTime)
            {
                image.color = Color.Lerp(Color.red, Color.white, t);
                yield return null;
            }
        }
    }
}