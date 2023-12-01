using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "ElevatedSpiritsBuff.Asset", menuName = "BuffSkill/ElevatedSpiritsBuff")]
public class ElevatedSpiritsBuff : BuffSkill
{
    public float increasedSpeedRate;
    public int invalidAttackCount;

    public override void ExecuteAttack(GameObject attacker, GameObject defender)
    {
        AIController buffAi = type switch
        {
            BuffType.Self => attacker.GetComponent<AIController>(),
            BuffType.Other => defender.GetComponent<AIController>(),
            _ => attacker.GetComponent<AIController>()
        };

        Vector3 textPos = attacker.transform.position;
        textPos.y += offsetText;
        TextMeshPro text = Instantiate(scrollingBuffText, textPos, Quaternion.identity);
        text.text = "Spirit UP!";
        text.color = new Color(0.7f, 1, 0);


        SpeedBuff speedBuff = new SpeedBuff();
        speedBuff.duration = duration;
        speedBuff.increasedSpeedRate = increasedSpeedRate;

        InvalidAttackBuff invalidAttackBuff = new InvalidAttackBuff();
        invalidAttackBuff.duration = duration;
        invalidAttackBuff.invalidAttackCount = invalidAttackCount;

        speedBuff.ApplyBuff(buffAi);
        invalidAttackBuff.ApplyBuff(buffAi);

        GameObject buffEffect = Instantiate(effectPrefab, buffAi.transform);
        Destroy(buffEffect, duration);
    }
}