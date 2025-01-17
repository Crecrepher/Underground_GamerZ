using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBuff : Buff
{
    public float increasedDamageRate;
    private float prevDamage;

    public override void UpdateBuff(AIController ai)
    {
        base.UpdateBuff(ai);
    }

    public override void ApplyBuff(AIController ai)
    {
        timer = Time.time;
        prevDamage = ai.status.damage;
        ai.status.damage += (prevDamage * increasedDamageRate);
        ai.appliedBuffs.Add(this);
    }

    public override void RemoveBuff(AIController ai)
    {
        ai.status.damage -= (prevDamage * increasedDamageRate);
        ai.removedBuffs.Add(this);
    }
}
