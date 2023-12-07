using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum OccupationType
{
    None,
    Normal,     // �Ϲ���
    Assault,    // ������
    Sniper,     // ������
    Support,     // ������
    Count
}

public enum DistancePriorityType
{
    None,
    Closer,     // �����
    Far,        // ��
    Count
}


public enum States
{
    Idle,
    MissionExecution,
    Trace,
    AimSearch,
    Attack,
    Kiting,
    Reloading,
}

public enum SkillTypes
{
    Base,                    // �⺻����
    Original,                // ������ų
    General,                // ���뽺ų
    Count
}

public enum SkillActionTypes
{
    Active,
    Passive,
}
public class AIController : MonoBehaviour
{
    public NavMeshAgent agent;
    public CharacterStatus status;

    private StateManager stateManager;
    private List<BaseState> states = new List<BaseState>();

    public Transform point;
    public Transform missionTarget;
    public Transform battleTarget;
    public Transform firePos;
    public Line currentLine = Line.Bottom;


    public LayerMask layer;
    public SPUM_Prefabs spum;

    public Transform rightHand;
    public Transform leftHand;
    public AIManager manager;

    public Vector3 hitInfoPos;
    public Vector3 kitingPos;
    public bool isKiting = false;


    [Header("���� ����")]
    public bool isBattle = false;

    [Header("���� & ī���� ����")]
    public AttackDefinition[] attackInfos = new AttackDefinition[(int)SkillTypes.Count];
    public KitingData kitingInfo;
    public KitingData reloadingKitingInfo;

    // ���� ������ ĳ������ �þ� ����
    [Header("���� ����")]
    public float supportTime = 0.5f;
    public float lastSupportTime;

    // �̰��� ���� ������ ���°� ����
    [Header("��� ����")]
    public bool isAttack = true;
    public bool isDefend;

    [Header("�켱 ���� ����")]
    //public OccupationType occupationType = OccupationType.None;
    //public DistancePriorityType distancePriorityType = DistancePriorityType.None;
    public int occupationIndex = int.MaxValue;
    //public List<CharacterStatus> filterdPriorityList = new List<CharacterStatus>();
    public TargetPriority priorityByDistance;
    public List<TargetPriority> priorityByOccupation = new List<TargetPriority>();

    [Tooltip("Ž�� ������ �ð�")]
    public float detectTime = 0.1f;

    [Header("����, ��ų ����")]
    [Tooltip("������ ���� ����")]
    public float lastBaseAttackTime;
    [Tooltip("���� ������ Ÿ��")]
    public float baseAttackCoolTime;
    public bool isOnCoolBaseAttack;

    [Tooltip("������ ������ų ��� ����")]
    public float lastOriginalSkillTime;
    [Tooltip("������ų ������ Ÿ��")]
    public float originalSkillCoolTime;
    public bool isOnCoolOriginalSkill;

    [Tooltip("������ ���뽺ų ��� ����")]
    public float lastGeneralSkillTime;
    [Tooltip("���뽺ų ������ Ÿ��")]
    public float generalSkillCoolTime;
    public bool isOnCoolGeneralSkill = false;

    [Tooltip("������ ���� ����")]
    public float lastReloadTime;
    [Tooltip("���� Ÿ��")]
    public float reloadCoolTime;
    public bool isReloading;

    public int maxAmmo;
    public int currentAmmo;

    [Header("����")]
    public List<Buff> appliedBuffs = new List<Buff>();
    public List<Buff> removedBuffs = new List<Buff>();
    public bool isInvalid = false;

    [Header("���̾�")]
    public int teamLayer;
    public int enemyLayer;
    public int obstacleLayer;

    [Header("UI")]
    public string statusName;
    public DebugAIStatusInfo debugAIStatusInfo;
    public CommandInfo aiCommandInfo;
    public TextMeshProUGUI aiType;
    public int colorIndex;

    public AICanvas canvas;

    public Transform[] tops;
    public Transform[] bottoms;

    public bool RaycastToTarget
    {
        get
        {
            if (this.battleTarget == null)
                return false;
            var origin = transform.position;
            var target = this.battleTarget.position;

            var sightOrigin = transform.position;
            var sightTarget = this.battleTarget.position;
            sightTarget.y = sightOrigin.y;

            var sightDirection = sightTarget - sightOrigin;
            sightDirection.Normalize();

            var direction = target - origin;
            direction.Normalize();

            float sightDot = Vector3.Dot(sightDirection, transform.forward);

            float dot = Vector3.Dot(direction, transform.forward);
            float angleInRadians = Mathf.Acos(dot);
            float angleInDegrees = angleInRadians * Mathf.Rad2Deg;
            float cosineValue = Mathf.Abs(Mathf.Cos(angleInDegrees));
            float attackRange = status.range / cosineValue;

            float sightAngle = 1f - (status.sightAngle / 2) * 0.01f;

            bool isCol = false;
            if (sightDot > sightAngle)
            {
                isCol = Physics.Raycast(origin, direction, out RaycastHit hitInfo, attackRange, enemyLayer);
                if (isCol)
                {
                    hitInfoPos = hitInfo.point;
                }
            }

            return isCol;
        }
    }
    public float DistanceToMissionTarget
    {
        get
        {
            if (missionTarget == null)
            {
                return 0f;
            }
            Vector3 targetPos = missionTarget.transform.position;
            targetPos.y = transform.position.y;
            return Vector3.Distance(transform.position, targetPos);
        }
    }

    public float DistanceToBattleTarget
    {
        get
        {
            if (battleTarget == null)
            {
                return 0f;
            }
            Vector3 targetPos = battleTarget.transform.position;
            targetPos.y = transform.position.y;
            return Vector3.Distance(transform.position, targetPos);
        }
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // ������ ���̺� ���� ������ ������ �� ����, ���� ������ ���̺� ������
        status = GetComponent<CharacterStatus>();
        missionTarget = point;

        SetInitialization();

        teamLayer = layer;

        if (teamLayer == LayerMask.GetMask("PC"))
            enemyLayer = LayerMask.GetMask("NPC");
        else
            enemyLayer = LayerMask.GetMask("PC");

        obstacleLayer = LayerMask.GetMask("Obstacle");

        firePos = rightHand;
    }

    private void Start()
    {
        stateManager = new StateManager();
        states.Add(new IdleState(this));
        states.Add(new MissionExecutionState(this));
        states.Add(new TraceState(this));
        states.Add(new AimSearchState(this));
        states.Add(new AttackState(this));
        states.Add(new KitingState(this));
        states.Add(new ReloadingState(this));

        agent.speed = status.speed;
        //agent.SetDestination(point.position);

        SetState(States.Idle);
    }
    private void Update()
    {
        if (lastOriginalSkillTime + originalSkillCoolTime < Time.time
            && attackInfos[(int)SkillTypes.Original] != null)
        {
            lastOriginalSkillTime = Time.time;
            isOnCoolOriginalSkill = true;
        }

        if (lastBaseAttackTime + baseAttackCoolTime < Time.time)
        {
            lastBaseAttackTime = Time.time;
            isOnCoolBaseAttack = true;
        }

        if (isReloading)
        {
            float time = (1 - (Time.time - lastReloadTime) / reloadCoolTime);
            GetReloadTime(time);
        }

        if (lastReloadTime + reloadCoolTime < Time.time && isReloading)
        {
            isReloading = false;
            Reload();
        }

        if (!isBattle && lastSupportTime + supportTime < Time.time)
        {
            lastSupportTime = Time.time;
            SupportTeam();
        }

        UpdateBuff();



        spum._anim.SetFloat("RunState", Mathf.Min(agent.velocity.magnitude, 0.5f));
        //�ִ� �ӵ��϶� 0.5f�� �Ǿ�� ������ 2�γ���
    }

    public void SetBattleTarget(Transform target)
    {
        Transform prevTarget = this.battleTarget;
        this.battleTarget = target;
        CharacterStatus status = target.GetComponent<CharacterStatus>();
        if (status != null)
        {
            if (prevTarget != null)
            {
                CharacterStatus prevTargetStatus = prevTarget.GetComponent<CharacterStatus>();
                BattleTargetEventBus.Unsubscribe(prevTargetStatus, ReleaseTarget);
            }
            BattleTargetEventBus.Subscribe(status, ReleaseTarget);
        }
        SetDestination(this.battleTarget.position);
    }

    public void SetMissionTarget(Transform target)
    {
        //Transform prevTarget = this.missionTarget;
        //this.missionTarget = target;

        //CharacterStatus status = target.GetComponent<CharacterStatus>();
        //if (status != null)
        //{
        //    if(prevTarget != null)
        //    {
        //        // �� �κ� �����ؾ���
        //        CharacterStatus prevTargetStatus = prevTarget.GetComponent<CharacterStatus>();
        //        BattleTargetEventBus.Unsubscribe(prevTargetStatus, ReleaseTarget);
        //    }
        //    BattleTargetEventBus.Subscribe(status, ReleaseTarget);
        //}
        SetDestination(this.missionTarget.position);
    }

    public void ReleaseTarget()
    {
        occupationIndex = int.MaxValue;
        battleTarget = null;
    }

    public void SetDestination(Vector3 vector3)
    {
        agent.SetDestination(vector3);
    }

    public void SetState(States newState)
    {
        stateManager.ChangeState(states[(int)newState]);
    }

    public void UpdateState()
    {
        stateManager.Update();
    }

    public void UpdateKiting()
    {
        if (battleTarget != null)
            kitingInfo.UpdateKiting(battleTarget, this);
    }

    public void UpdateReloadKiting()
    {
        if (battleTarget != null)
            reloadingKitingInfo.UpdateKiting(battleTarget, this);
    }

    public void GetReloadTime(float time)
    {
        canvas.reloadBar.value = time;
    }

    public void TryReloading()
    {
        canvas.reloadBar.gameObject.SetActive(true);
    }

    public void Reload()
    {
        currentAmmo = maxAmmo;
        canvas.reloadBar.gameObject.SetActive(false);
    }

    private void SupportTeam()
    {
        Transform target = null;
        var cols = Physics.OverlapSphere(transform.position, status.sight, teamLayer);
        float distance = float.MaxValue;
        foreach (var col in cols)
        {
            if (col.gameObject == this.gameObject)
                continue;
            AIController controller = col.GetComponent<AIController>();

            if (isAttack)
            {
                // �������� �Ʊ� �˻�
                if (controller != null && controller.isBattle)
                {
                    float colDistance = Vector3.Distance(transform.position, col.transform.position);
                    if (colDistance < distance)
                    {
                        target = controller.battleTarget;
                        distance = colDistance;
                    }
                }
            }
            else if(isDefend)
            {
                TeamIdentifier colIdentity = col.GetComponent<TeamIdentifier>();
                // �������� �������� ��찡 ������?
                // ������ �˻�
                if (colIdentity != null && colIdentity.isBuilding)
                {
                    if(colIdentity.buildingTarget)
                    {
                        target = colIdentity.buildingTarget;
                        break;
                    }
                }

                // �������� �Ʊ� �˻�
                if (controller != null && controller.isBattle)
                {
                    float colDistance = Vector3.Distance(transform.position, col.transform.position);
                    if (colDistance < distance)
                    {
                        target = controller.battleTarget;
                        distance = colDistance;
                    }
                }

            }
        }

        if (target != null)
        {
            SetBattleTarget(target);
            SetState(States.Trace);
        }
    }

    public void RefreshDebugAIStatus(string debug)
    {
        statusName = $"{debugAIStatusInfo.aiType}{debugAIStatusInfo.aiNum} : {debug}";
        debugAIStatusInfo.GetComponentInChildren<TextMeshProUGUI>().text = statusName;
    }

    private void UpdateBuff()
    {
        foreach (var buff in appliedBuffs)
        {
            buff.UpdateBuff(this);
        }

        foreach (var buff in removedBuffs)
        {
            appliedBuffs.Remove(buff);
        }

        if (removedBuffs.Count > 0)
        {
            removedBuffs.Clear();
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        float viewAngle = 60f;
        float viewRadius = 2f;
        float dectionRange = 1f;
        if (status != null)
        {
            viewRadius = status.sight;
            viewAngle = status.sightAngle;
            dectionRange = status.detectionRange;
        }
        Gizmos.DrawWireSphere(transform.position, dectionRange);

        Vector3 viewAngleA = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 viewAngleB = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;
        Gizmos.color = Color.blue;
        Vector3 newV = transform.position;
        newV.y += 0.5f;
        Gizmos.DrawLine(newV, newV + viewAngleA * viewRadius);
        Gizmos.DrawLine(newV, newV + viewAngleB * viewRadius);

        if (firePos != null && battleTarget != null)
        {
            if (RaycastToTarget)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(firePos.position, hitInfoPos);
            }
        }
    }

    public void SetInitialization()
    {
        if (attackInfos[(int)SkillTypes.Base] != null)
        {
            baseAttackCoolTime = attackInfos[(int)SkillTypes.Base].cooldown;
            reloadCoolTime = attackInfos[(int)SkillTypes.Base].reloadCooldown;
            maxAmmo = attackInfos[(int)SkillTypes.Base].chargeCount;
        }
        lastReloadTime = Time.time - reloadCoolTime;
        lastBaseAttackTime = Time.time - baseAttackCoolTime;

        if (attackInfos[(int)SkillTypes.Original] != null)
            originalSkillCoolTime = attackInfos[(int)SkillTypes.Original].cooldown;
        lastOriginalSkillTime = Time.time - originalSkillCoolTime;

        if (attackInfos[(int)SkillTypes.General] != null)
            generalSkillCoolTime = attackInfos[(int)SkillTypes.General].cooldown;
        lastGeneralSkillTime = Time.time - generalSkillCoolTime;

        lastSupportTime = Time.time - supportTime;

        Reload();
    }
}
