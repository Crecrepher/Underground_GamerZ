using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamType
{
    PC,
    NPC
}

public class TeamIdentifier : MonoBehaviour
{
    public LayerMask teamLayer;
    public LayerMask enemyLayer;

    public bool isBuilding;
    public Transform buildingTarget;
    private CharacterStatus status;
    [Header("���๰ ���� ����")]
    public TeamType teamType;
    public Line line;

    private float targetReleaseTime = 3f;
    private float lastTargetReleaseTime;

    public float recoveryTime = 2f;
    private float lastRecoveryTime;

    public bool isSelfRecovery;
    public int recoveryValue;

    private void Awake()
    {
        status = GetComponent<CharacterStatus>();
    }

    private void Update()
    {
        // �����ð� ���� Ÿ���� ���žȵǸ� ���� / ������ ���ݴ����� ������
        if (isBuilding && buildingTarget != null && targetReleaseTime + lastTargetReleaseTime < Time.time)
        {
            ReleaseTarget();
        }

        // �ǹ� �ڰ�����
        if (isBuilding && isSelfRecovery && lastRecoveryTime + recoveryTime < Time.time)
        {
            lastRecoveryTime = Time.time;
            if (status != null)
            {
                status.Hp += recoveryValue;
                status.Hp = Mathf.Min(status.Hp, status.maxHp);
                status.GetHp();
                //Debug.Log($"{gameObject.name} : {status.Hp}");
            }
        }
    }

    // �ǹ��� ������ Ÿ�� ����, Ÿ�� ������ ����
    public void SetBuildingTarget(Transform attacker)
    {
        isSelfRecovery = false;
        lastTargetReleaseTime = Time.time;
        Transform prevTarget = this.buildingTarget;
        this.buildingTarget = attacker;
        CharacterStatus status = attacker.GetComponent<CharacterStatus>();

        if (prevTarget != null)
        {
            CharacterStatus prevStatus = prevTarget.GetComponent<CharacterStatus>();
            BattleTargetEventBus.Unsubscribe(prevStatus, ReleaseTarget);
        }

        if (status != null)
        {
            BattleTargetEventBus.Subscribe(status, ReleaseTarget);
        }
    }

    private void ReleaseTarget()
    {
        lastTargetReleaseTime = Time.time;
        buildingTarget = null;

        isSelfRecovery = true;
        lastRecoveryTime = Time.time;
    }
}
