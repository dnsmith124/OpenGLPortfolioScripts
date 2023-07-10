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
}
