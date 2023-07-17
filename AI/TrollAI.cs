using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrollAI : EnemyAI
{
    private const string attackOneTriggerName = "Attacking";
    private const string attackTwoTriggerName = "AttackingTwo";
    private const string attackThreeTriggerName = "AttackingThree";
    private string prevAnimUsed = "AttackingTwo";
    private int numAttacks = 0;
    private bool hasShouted = false;
    private bool coroutinePlaying = true;
    private GameController gameController;
    public GameObject healthBarParent;
    public GameObject cage;
    public GameCompleteScreen gameCompleteScreen;

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

    protected override IEnumerator TriggerAttack(string animString)
    {
        string upcomingAnim = attackOneTriggerName;
        switch (prevAnimUsed) { 
        
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
        return base.TriggerAttack(upcomingAnim);
    }

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
