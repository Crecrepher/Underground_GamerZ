using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum BuffType
{
    Self,
    Other
}

[CreateAssetMenu(fileName = "BuffSkill.Asset", menuName = "BuffSkill/BuffSkill")]
public class BuffSkill : AttackDefinition
{
    [Header("����")]
    public float coolTime;
    public float duration;
    public BuffType type;
    public GameObject effectPrefab;
    public TextMeshPro scrollingBuffText;
    public float offsetText;

    public override void ExecuteAttack(GameObject attacker, GameObject defender)
    {

    }
}
