using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrollAI : EnemyAI
{
    public GameObject healthBarParent;
    public GameObject cage;
    public GameCompleteScreen gameCompleteScreen;
    public float withstandStaggerChance;

    private const string attackOneTriggerName = "Attacking";
    private const string attackTwoTriggerName = "AttackingTwo";
    private const string attackThreeTriggerName = "AttackingThree";
    private string prevAnimUsed = "AttackingTwo";
    private int numAttacks = 0;
    private bool hasShouted = false;
    private bool coroutinePlaying = true;
    private GameController gameController;

    private void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    protected override void HandleIdling(float distanceToTarget)
    {
        if (distanceToTarget <= chaseRange && distanceToTarget > attackRange)
        {
            if(!hasShouted)
            {
                //StartCoroutine(ShoutAndWait());
            }
            if(!coroutinePlaying)
            {
                ChangeState(State.Walking);
            }
            return;
        }
        if (distanceToTarget <= attackRange)
        {
            ChangeState(State.Attacking);
            return;
        }
    }

    protected override void HandleTakingDamage()
    {
        agent.ResetPath();

        if (!hasShouted)
            StartCoroutine(ShoutAndWait());

        if (!coroutinePlaying)
            StartCoroutine(TriggerDamage());
    }

    protected override void ChangeState(State newState)
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
                float takeHit = Random.Range(0f, 1f);
                if (!hasShouted || (!isFrozen && takeHit > withstandStaggerChance))
                    animator.SetTrigger("Damaging");
                else
                    ChangeState(State.Idling);
                if (!isAggroed)
                    HandleUpdateAggroState();
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

    protected override void HandleAttacking()
    {
        agent.ResetPath();
        string upcomingAnim = attackOneTriggerName;
        switch (prevAnimUsed)
        {

            case attackOneTriggerName:
                upcomingAnim = attackTwoTriggerName;
                break;
            case attackTwoTriggerName:
                upcomingAnim = attackOneTriggerName;
                break;
            case attackThreeTriggerName:
                upcomingAnim = attackOneTriggerName;
                break;
            default:
                break;
        }
        if (numAttacks > 1 && Random.value <= 0.30f)
            upcomingAnim = attackThreeTriggerName;

        prevAnimUsed = upcomingAnim;
        numAttacks++;
        StartCoroutine(TriggerAttack(upcomingAnim));
    }

    //protected override IEnumerator TriggerAttack(string animString)
    //{
    //    string upcomingAnim = attackOneTriggerName;
    //    switch (prevAnimUsed) { 
        
    //        case attackOneTriggerName:
    //            upcomingAnim = attackTwoTriggerName;
    //            break;
    //        case attackTwoTriggerName:
    //            upcomingAnim = attackOneTriggerName;
    //            break;
    //        case attackThreeTriggerName:
    //            upcomingAnim = attackOneTriggerName;
    //            break;
    //        default:
    //            break;
    //    }
    //    if (numAttacks > 1 && Random.value <= 0.30f)
    //        upcomingAnim = attackThreeTriggerName;

    //    prevAnimUsed = upcomingAnim;
    //    numAttacks++;
    //    Debug.Log("Trigger Attack");
    //    return base.TriggerAttack(upcomingAnim);
    //}

    protected override void LateUpdate ()
    {

    }

    private IEnumerator ShoutAndWait()
    {
        hasShouted = true;

        float animationLength = AnimationUtils.GetAnimationClipLength(animator, "Shouting");
        animator.SetTrigger("Shouting");

        healthBarObject.SetActive(true);
        healthBarParent.GetComponent<Animator>().SetTrigger("reveal");

        yield return new WaitForSeconds(animationLength);

        chaseRange += 20.0f;

        coroutinePlaying = false;
    }

    protected override IEnumerator TriggerDyingAnimAndCleanup()
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

        healthBarParent.SetActive(false);
        cage.SetActive(false);
        GameCompleteScreen.Instance.HandleFade();
        gameController.UpdateCondition("beatBoss", true);
        gameController.UpdateCondition("bossLives", false);

        Die();
    }
}
