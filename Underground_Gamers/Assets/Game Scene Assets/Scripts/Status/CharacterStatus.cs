using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatus : MonoBehaviour
{
    [Header("우선 순위 설정")]
    public OccupationType occupationType = OccupationType.None;
    public DistancePriorityType distancePriorityType = DistancePriorityType.None;

    public string AIName;

    [Header("스텟")]
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
    public float accuracyRate;      // 명중률

    public int armor;

    public int condition;

    public int killCount;
    public int deathCount;
    public float respawnTime;
    public float respawnTimeIncreaseRate;

    [Header("리워드 목록")]
    public Sprite illustration;
    public Sprite aiClass;
    public int lv;
    public int maxLv;
    public float xp;
    public float maxXp;
    public Sprite grade;

    [Header("데미지 그래프")]
    public float dealtDamage;
    public float takenDamage;
    public float healAmount;

    public bool IsLive { get; set; } = true;
    public int Hp { get; set; }

    private void Awake()
    {
        Hp = maxHp;
        killCount = 0;
        deathCount = 0;
    }

    public void ResetStatus()
    {
        IsLive = true;
        Hp = maxHp;
        killCount = 0;
        deathCount = 0;
        dealtDamage = 0;
        takenDamage = 0;
        healAmount = 0;
    }

    public void Respawn()
    {
        AIController aiController = GetComponent<AIController>();
        GameManager gameManager = GetComponent<AIController>().gameManager;
        IsLive = true;
        Hp = maxHp;
        GetHp();

    }

    public void GetHp()
    {
        var aiCanvas = GetComponentInChildren<AICanvas>();
        if (aiCanvas != null)
        {
             aiCanvas.hpBar.value = (float)Hp / (float)maxHp;
        }

        var controller = GetComponent<AIController>();
        if (controller != null)
        {
            if (controller.aiCommandInfo != null)
                controller.aiCommandInfo.DisplayHpBar((float)Hp / (float)maxHp);
        }
    }

    public void LevelUp()
    {
        var pt = DataTableManager.instance.Get<PlayerTable>(DataType.Player);
        while (xp >= maxXp)
        {
            lv++;
            if (lv >= maxLv)
            {
                xp = 0;
                if (maxLv >= 50)
                {
                    maxXp = 0;
                    break;
                }
                else
                {
                    maxXp = pt.GetLevelUpXp(lv + 1);
                }
            }
            else
            {
                xp = xp - maxXp;
                maxXp = pt.GetLevelUpXp(lv + 1);
            }
        }
    }

}
