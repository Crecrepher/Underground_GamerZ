using EPOOutline;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    Retreat,
    Patrol
}

public enum SkillMode
{
    Base,                    // �⺻����
    Original,                // ������ų
    General,                // ���뽺ų
    Count
}

public enum SkillType
{
    Attack,
    Buff,
    Heal
}

public enum SkillActionTypes
{
    Active,
    Passive,
}

public class AIController : MonoBehaviour
{
    public Player playerInfo;
    public int code;

    public NavMeshAgent agent;
    public CharacterStatus status;

    private AIManager aiManager;
    public GameManager gameManager;
    public BuildingManager buildingManager;
    public TeamIdentifier teamIdentity;

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

    [Header("AI ����")]
    public GameObject selectEffect;
    public int selectSortOrder = 5;
    public int originSortOrder = 0;

    public Vector3 hitInfoPos;
    public Vector3 kitingPos;
    public bool isKiting = false;


    [Header("���� ����")]
    public bool isBattle = false;

    [Header("���� & ī���� ����")]
    public AttackDefinition[] attackInfos = new AttackDefinition[(int)SkillMode.Count];
    public KitingData kitingInfo;
    public KitingData reloadingKitingInfo;

    // ���� ������ ĳ������ �þ� ����
    [Header("���� ����")]
    public float supportTime = 0.5f;
    public float lastSupportTime;

    // �̰��� ���� ������ ���°� ����
    [Header("��� ����")]
    public bool isAttack = true;
    public bool isDefend = false;
    public bool isMission = false;
    public bool isRetreat = false;

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

    public bool isPrior = false;

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
    public LayerMask teamLayer;
    public LayerMask enemyLayer;
    public LayerMask obstacleLayer;
    public LayerMask nodeLayer;

    [Header("UI")]
    public string statusName;
    public DebugAIStatusInfo debugAIStatusInfo;
    public CommandInfo aiCommandInfo;
    public Color commandInfoOutlineColor;
    public TextMeshProUGUI aiType;
    public int aiIndex;
    public AICanvas canvas;
    public Color selectOutlineColor;
    public Color unselectOutlineColor;

    [Header("������ �׷���")]
    public DamageGraph damageGraph;

    public Outlinable outlinable;
    public List<AIController> currentLinerInfo;

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

            // dot = 1 �� ��, Acos �� �ѱ�� NoN�� �ߴ� ������, 0���� ��ȯ
            float angleInRadians = Mathf.Acos(dot);
            if (dot >= 1)
            {
                angleInRadians = 0;
            }
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
            Collider col = missionTarget.GetComponent<Collider>();
            //if (col != null)
            //{
            //    var colDir = transform.position - targetPos;
            //    colDir.Normalize();
            //    var colDis = colDir * col.bounds.extents.x;
            //    targetPos += colDis;
            //}
            float colDis = 0;
            if (col != null)
            {
                if (col.bounds.extents.x >= col.bounds.extents.z)
                    colDis = col.bounds.extents.x;
                else
                    colDis = col.bounds.extents.z;
            }
            return Vector3.Distance(transform.position, targetPos) - colDis;
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

            Collider col = battleTarget.GetComponent<Collider>();
            //if (col != null)
            //{
            //    var colDir = transform.position - targetPos;
            //    colDir.Normalize();
            //    var colDis = colDir * col.bounds.extents.x;
            //    targetPos += colDis;
            //}

            float colDis = 0;
            if (col != null)
            {
                if (col.bounds.extents.x >= col.bounds.extents.z)
                    colDis = col.bounds.extents.x;
                else
                    colDis = col.bounds.extents.z;
            }
            return Vector3.Distance(transform.position, targetPos) - colDis;
        }
    }
    private void Awake()
    {
        if (gameManager == null)
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (aiManager == null)
            aiManager = GameObject.FindGameObjectWithTag("AIManager").GetComponent<AIManager>();
        if (buildingManager == null)
            buildingManager = GameObject.FindGameObjectWithTag("BuildingManager").GetComponent<BuildingManager>();
        //if (outlinable == null)
        //{
        //    // �ƿ����� �ʱ� ����
        //    outlinable = spum.AddComponent<Outlinable>();
        //    outlinable.AddAllChildRenderersToRenderingList();
        //    outlinable.OutlineParameters.Color = unselectOutlineColor;
        //}

        agent = GetComponent<NavMeshAgent>();
        status = GetComponent<CharacterStatus>();
        teamIdentity = GetComponent<TeamIdentifier>();

        //missionTarget = point;
        //SetInitialization();

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
        states.Add(new RetreatState(this));
        states.Add(new PatrolState(this));

        agent.speed = status.speed;

        if (teamIdentity.teamType == TeamType.PC)
            gameManager.lineManager.JoiningLine(this);
        if (teamIdentity.teamType == TeamType.NPC)
            gameManager.npcManager.SelectLineByInit(this);
    }

    private void OnDisable()
    {
        foreach (var buff in appliedBuffs)
        {
            buff.RemoveBuff(this);
        }

        foreach (var buff in removedBuffs)
        {
            appliedBuffs.Remove(buff);
        }
        appliedBuffs.Clear();
        removedBuffs.Clear();
    }

    public void InitInGameScene()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        aiManager = GameObject.FindGameObjectWithTag("AIManager").GetComponent<AIManager>();
        buildingManager = GameObject.FindGameObjectWithTag("BuildingManager").GetComponent<BuildingManager>();
        outlinable.AddAllChildRenderersToRenderingList();
        outlinable.OutlineParameters.Color = unselectOutlineColor;
    }
    private void Update()
    {
        if (lastBaseAttackTime + baseAttackCoolTime < Time.time)
        {
            lastBaseAttackTime = Time.time;
            isOnCoolBaseAttack = true;
        }

        // ���� �Ұ� �����϶��� ������
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

        if (!isBattle && lastSupportTime + supportTime < Time.time && !isReloading)
        {
            lastSupportTime = Time.time;
            SupportTeam();
        }

        UpdateBuff();

        //�ִ� �ӵ��϶� 0.5f�� �Ǿ�� ������ 2�γ���, ī���� �ӵ� ���
        float s = agent.velocity.magnitude / (agent.speed + 3) * 0.5f;
        if (s <= 0.1f)
            s = Mathf.Max(0.15f, s);

        if (agent.velocity.magnitude < 0.1f)
            s = 0f;

        spum._anim.SetFloat("RunState", Mathf.Min(s, 0.5f));
    }

    public void UnSelectAI()
    {
        // �г� �Ʒ��� ������ / ���̾� �缳��
        //SortingGroup sort = spum.GetComponentInChildren<SortingGroup>();
        //Canvas canvas = transform.GetComponentInChildren<Canvas>();
        //var particleRenderer = transform.GetComponentInChildren<ParticleSystem>().GetComponent<Renderer>();

        //canvas.sortingOrder = originSortOrder;
        //sort.sortingOrder = originSortOrder;
        //particleRenderer.sortingOrder = originSortOrder;
        //if (aiCommandInfo != null)

        // �ӽ� ����
        outlinable.OutlineParameters.Color = unselectOutlineColor;

        gameManager.commandManager.ActiveAllCommandButton();
        aiCommandInfo.UnSelectAI();
        selectEffect.SetActive(false);
    }

    public void SelectAI()
    {
        // �г� ���� ����
        //SortingGroup sort = spum.GetComponentInChildren<SortingGroup>();
        //Canvas canvas = transform.GetComponentInChildren<Canvas>();
        //var particleRenderer = transform.GetComponentInChildren<ParticleSystem>(true).GetComponent<Renderer>();

        //canvas.sortingOrder = selectSortOrder;
        //sort.sortingOrder = selectSortOrder;
        //particleRenderer.sortingOrder = selectSortOrder;
        //if (aiCommandInfo != null)
        //CommandManager commandManager = gameManager.commandManager;
        //commandManager.SetActiveCommandButton(commandManager.currentAI);

        // �ӽ� ����
        selectEffect.SetActive(true);
        outlinable.OutlineParameters.Color = selectOutlineColor;
        aiCommandInfo.SelectAI();
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
        if (this.status.IsLive)
            SetDestination(this.battleTarget);
    }

    public void SetMissionTarget(Transform target)
    {
        Building prevBuilding = this.missionTarget.GetComponent<Building>();
        if (prevBuilding != null)
        {
            prevBuilding.ReleaseAIController(this);
        }
        this.missionTarget = target;
        Building building = null;
        if (isAttack)
            building = this.missionTarget.GetComponent<Building>();
        if (isDefend)
            building = this.missionTarget.parent.GetComponent<Building>();

        if (building != null)
            building.AddAIController(this);
        if (status.IsLive)
            SetDestination(this.missionTarget.position);
    }

    public void RefreshBuilding()
    {
        isRetreat = false;
        isMission = false;
        Transform[] wayPoints = currentLine switch
        {
            Line.Bottom => gameManager.wayPoint.bottomWayPoints,
            Line.Top => gameManager.wayPoint.topWayPoints,
            _ => gameManager.wayPoint.bottomWayPoints
        };

        Transform lineWayPoint = Utils.FindNearestPoint(this, wayPoints);
        if (lineWayPoint != null)
        {
            // ���⼭ Ÿ�ٸ� ����ش�, ���� ���� ��� �����ϱ� ����
            missionTarget = lineWayPoint;
            //ai.SetMissionTarget(lineWayPoint);
        }

        //if (isAttack)
        //{
        //    point = buildingManager.GetAttackPoint(currentLine, teamIdentity.teamType);
        //    missionTarget = point;
        //}
        //else
        //{
        //    point = buildingManager.GetDefendPoint(currentLine, teamIdentity.teamType).GetComponent<Building>().defendPoint;
        //    missionTarget = point;
        //}
    }

    public void ReleaseTarget()
    {
        occupationIndex = int.MaxValue;
        battleTarget = null;
    }

    public void SetDestination(Transform target)
    {
        Collider col = target.GetComponent<Collider>();
        Vector3 destination = target.position;
        destination.y = transform.position.y;
        //if (col != null)
        //{
        //    var colDir = transform.position - destination;
        //    colDir.Normalize();
        //    var colDis = colDir * col.bounds.extents.x;
        //    destination += colDis;
        //}
        agent.SetDestination(destination);
    }

    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
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
            else if (isDefend)
            {
                TeamIdentifier colIdentity = col.GetComponent<TeamIdentifier>();
                // �������� �������� ��찡 ������?
                // ������ �˻�
                if (colIdentity != null && colIdentity.isBuilding)
                {
                    if (colIdentity.buildingTarget)
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
        if (debugAIStatusInfo != null)
        {
            statusName = $"{debugAIStatusInfo.aiType}{debugAIStatusInfo.aiNum} : {debug}";
            debugAIStatusInfo.GetComponentInChildren<TextMeshProUGUI>().text = statusName;
        }
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
        if (attackInfos[(int)SkillMode.Base] != null)
        {
            baseAttackCoolTime = attackInfos[(int)SkillMode.Base].cooldown + status.cooldown;
            reloadCoolTime = attackInfos[(int)SkillMode.Base].reloadCooldown;
            maxAmmo = attackInfos[(int)SkillMode.Base].chargeCount;
        }
        lastReloadTime = Time.time - reloadCoolTime;
        lastBaseAttackTime = Time.time - baseAttackCoolTime;
        isOnCoolBaseAttack = true;

        if (attackInfos[(int)SkillMode.Original] != null)
            originalSkillCoolTime = attackInfos[(int)SkillMode.Original].cooldown;
        lastOriginalSkillTime = Time.time - originalSkillCoolTime;
        isOnCoolOriginalSkill = true;

        if (attackInfos[(int)SkillMode.General] != null)
            generalSkillCoolTime = attackInfos[(int)SkillMode.General].cooldown;
        lastGeneralSkillTime = Time.time - generalSkillCoolTime;

        lastSupportTime = Time.time - supportTime;

        Reload();
    }

    public void Respawn()
    {
        lastReloadTime = Time.time - reloadCoolTime;
        lastSupportTime = Time.time - supportTime;
        Reload();

        if (attackInfos[(int)SkillMode.Base] != null)
            isOnCoolBaseAttack = true;
        else
            isOnCoolBaseAttack = false;

        RefreshBuilding();
        // ���� ����
        if (isAttack)
            SetState(States.MissionExecution);
        if (isDefend)
            SetState(States.Retreat);

        if (teamIdentity.teamType == TeamType.NPC)
        {
            gameManager.npcManager.SelectLineByRate(this);
            gameManager.lineManager.JoiningLine(this);
        }

        // ī�޶� ����
        if (gameManager != null)
        {
            if (gameManager.commandManager.currentAI == this)
            {
                gameManager.cameraManager.StartZoomIn();
            }
        }
    }
}
