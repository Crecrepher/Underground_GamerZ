
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackDefinition.Asset", menuName = "AttackData/AttackDefinition")]
public class AttackDefinition : ScriptableObject
{
    [Header("����")]
    [Tooltip("������")]
    public float damage;
    public float maxDamageRate;
    public float minDamageRate;

    public float cooldown;
    [HideInInspector]
    public float fixedCooldown;
    public float reloadCooldown;

    [Tooltip("���� ��")]
    public int chargeCount;
    [Tooltip("���߷�")]
    public float accuracyRate;
    [Tooltip("��ź��")]
    public float spreadRate;


    [Range(0f, 1f)]
    [Tooltip("ũ��Ƽ�� Ȯ�� 0 ~ 1 ���� ��")]
    public float criticalRate;
    public float criticalMultiplier;

    public SkillMode skillMode;
    public SkillType skillType;

    public Attack CreateAttack(CharacterStatus attacker, CharacterStatus defender)
    {
        float damage = this.damage + attacker.damage;
        damage *= Random.Range(minDamageRate, maxDamageRate);
        // ĳ������ ���� + ������ ����(��ų, ��Ÿ)
        bool isCritical = Random.value < attacker.critical;

        if(isCritical)
        {
            damage *= criticalMultiplier;
        }

        if(defender != null)
        {
            damage -= defender.armor;
            damage = Mathf.Max(0, damage);
        }

        return new Attack((int)damage, isCritical, false);
    }

    public virtual void ExecuteAttack(GameObject attacker, GameObject defender)
    {

    }
}
