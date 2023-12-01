using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    public int maxHp = 50;
    public float speed;
    public float sight;                 // 시야
    public float sightAngle;        // 시야각
    public float detectionRange;    // 탐지범위

    public float range;         // 공격 사정거리

    public float evasionRate;       // 회피율
    public float reactionSpeed;     // 반응속도

    public float damage;
    public float cooldown;
    public float critical;
    public int chargeCount;
    public float reloadCooldown;
    public float accuracyRate;

    public int armor;

    public int deathCount;
    public float respawnTime;
    public float respawnTimeIncreaseRate;
    public bool IsLive { get; set; } = true;
    public int Hp { get; set; }

    private void Awake()
    {
        Hp = maxHp;
    }

    public void Respawn()
    {
        AIController aIController = GetComponent<AIController>();
        if (aIController != null)
        {
            aIController.isOnCoolBaseAttack = true;
            aIController.isOnCoolOriginalSkill = true;
            aIController.isOnCoolGeneralSkill = true;

            //aIController.isKiting = false;
        }
        IsLive = true;
        Hp = maxHp;
    }
}
