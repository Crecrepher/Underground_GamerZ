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
    public float castDuration = 1f;
    public float duration;
    public BuffType type;
    public CastEffect castEffectPrefab;
    public GameObject durationEffectPrefab;
    public TextMeshPro scrollingBuffText;

    [Header("��ġ ����")]
    public float offsetText;
    public float offsetDurationEffct;
    public float offsetCastEffct;    
    
    [Header("ũ�� ����")]
    public float scaleText = 1f;
    public float scaleDurationEffct = 1f;
    public float scaleCastEffct = 1f;

    public override void ExecuteAttack(GameObject attacker, GameObject defender)
    {

    }
}
