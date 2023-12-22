using EPOOutline;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum GameType
{
    Story,
    Official,
    Scrimmage,
}
public class GameInfo : MonoBehaviour
{
    public static GameInfo instance
    {
        get
        {
            if (gameInfo == null)
            {
                gameInfo = FindObjectOfType<GameInfo>();
            }
            return gameInfo;
        }
    }

    private static GameInfo gameInfo;

    public GameObject playerObj;
    public GameObject enemyObj;
    public int currentStage = 0;
    public float RandomSpawnRange = 1f;
    public GameType gameType = GameType.Story;
    List<Player> entryPlayer = new List<Player>();

    [HideInInspector]
    public int screammageLevel = 0;
    private List<GameObject> players;
    private List<GameObject> enemys;
    private PlayerTable pt;
    public void Awake()
    {
        players = new List<GameObject>();
        enemys = new List<GameObject>();
    }
    private void Start()
    {
        pt = DataTableManager.instance.Get<PlayerTable>(DataType.Player);
    }

    public void SetEntryPlayer(int[] entryIndex)
    {
        entryPlayer.Clear();
        entryPlayer = new List<Player>()
        {
            GamePlayerInfo.instance.GetOfficialPlayer(entryIndex[0]),
            GamePlayerInfo.instance.GetOfficialPlayer(entryIndex[1]),
            GamePlayerInfo.instance.GetOfficialPlayer(entryIndex[2]),
            GamePlayerInfo.instance.GetOfficialPlayer(entryIndex[3]),
            GamePlayerInfo.instance.GetOfficialPlayer(entryIndex[4])
        };
        GamePlayerInfo.instance.SaveFile();
    }

    public void StartGame()
    {
        GamePlayerInfo.instance.SaveFile();
        switch (gameType)
        {
            case GameType.Story:
                entryPlayer = GamePlayerInfo.instance.usingPlayers;
                break;
            case GameType.Official:
                {
                    //SetEntryPlayer();
                }
                break;
            case GameType.Scrimmage:
                entryPlayer = GamePlayerInfo.instance.usingPlayers;
                break;
        }
    }

    public void MakePlayers()
    {
        GamePlayerInfo.instance.isOnSchedule = false;
        var stateDefines = DataTableManager.instance.stateDef;

        for (int i = 0; i < 5; i++)
        {
            var player = entryPlayer[i];
            // �׽�Ʈ�� ���� �ڵ�
            if (pt == null)
                return;
            PlayerInfo playerInfo = pt.GetPlayerInfo(player.code);
            foreach (var item in player.training)
            {
                var ti = pt.GetTrainingInfo(item);
                ti.AddStats(playerInfo);
            }
            var madePlayer = Instantiate(playerObj);
            //madePlayer.AddComponent<DontDestroy>();

            var madePlayerCharactor = Instantiate(Resources.Load<GameObject>(Path.Combine("SPUM", $"{player.code}")), madePlayer.transform);
            madePlayerCharactor.AddComponent<LookCameraRect>();
            var outLine = madePlayerCharactor.AddComponent<Outlinable>();

            float charactorScale = madePlayer.transform.localScale.x;

            var ai = madePlayer.GetComponent<AIController>();
            ai.spum = madePlayerCharactor.GetComponent<SPUM_Prefabs>();
            var childs = madePlayerCharactor.GetComponentsInChildren<Transform>();
            foreach (var child in childs)
            {
                if (child.name == "ArmL")
                {
                    ai.leftHand = child;
                }
                else if (child.name == "ArmR")
                {
                    ai.rightHand = child;
                }

                //�ӽ��ڵ� ���߿� �ٲ����!!
                ai.firePos = ai.rightHand;
            }
            AttackDefinition atkDef = stateDefines.attackDefs.Find(a => a.code == playerInfo.atkType).value;
            AttackDefinition skillDef = stateDefines.skillDatas.Find(a => a.code == playerInfo.UniqueSkillCode).value;
            ai.attackInfos[0] = atkDef;
            ai.attackInfos[1] = skillDef;
            ai.kitingInfo = stateDefines.kitingDatas.Find(a => a.code == playerInfo.kitingType).value;
            ai.code = playerInfo.code;
            ai.SetInitialization();
            //ai.aiCommandInfo.SetPortraitInCommandInfo(player.code);
            ai.outlinable = outLine;

            var stat = madePlayer.GetComponent<CharacterStatus>();

            stat.AIName = playerInfo.name;
            stat.Hp = (int)pt.CalculateCurrStats(playerInfo.hp, player.level);
            stat.maxHp = stat.Hp;
            stat.speed = pt.CalculateCurrStats(playerInfo.moveSpeed, player.level);
            stat.sight = pt.CalculateCurrStats(playerInfo.sight, player.level);
            stat.range = pt.CalculateCurrStats(playerInfo.range, player.level);
            stat.reactionSpeed = pt.CalculateCurrStats(playerInfo.reactionSpeed, player.level) * 15;
            stat.damage = pt.CalculateCurrStats(playerInfo.atk, player.level);
            stat.cooldown = pt.CalculateCurrStats(playerInfo.atkRate, player.level);
            stat.critical = pt.CalculateCurrStats(playerInfo.critical, player.level);
            stat.chargeCount = playerInfo.magazine;
            stat.reloadCooldown = playerInfo.reloadingSpeed;
            stat.accuracyRate = pt.CalculateCurrStats(playerInfo.accuracy, player.level);
            stat.detectionRange = pt.CalculateCurrStats(playerInfo.detectionRange, player.level);
            stat.occupationType = (OccupationType)playerInfo.type;
            stat.distancePriorityType = DistancePriorityType.Closer;
            stat.illustration = pt.GetPlayerSprite(playerInfo.code);
            stat.aiClass = pt.playerTypeSprites[playerInfo.type - 1];
            stat.grade = pt.starsSprites[playerInfo.grade - 3];
            stat.lv = player.level;
            stat.maxLv = player.maxLevel;
            stat.xp = player.xp;
            stat.maxXp = player.maxXp;
            stat.condition = player.condition;

            switch (stat.occupationType)
            {
                case OccupationType.Normal:
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 1).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 2).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 3).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 4).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 0).value);
                    break;
                case OccupationType.Assault:
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 3).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 4).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 1).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 2).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 0).value);
                    break;
                case OccupationType.Sniper:
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 4).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 3).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 2).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 1).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 0).value);
                    break;
                case OccupationType.Support:
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 1).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 2).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 3).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 4).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 0).value);
                    break;
                default:
                    break;
            }
            ai.priorityByDistance = stateDefines.distanceTargetPriorityDatas.Find(a => a.code == 0).value;

            madePlayer.SetActive(false);
            players.Add(madePlayer);
        }

        var startPos = GameObject.FindGameObjectsWithTag("PlayerStartPos");
        var endPos = GameObject.FindGameObjectsWithTag("EnemyStartPos");

        var buildingManager = GameObject.FindGameObjectWithTag("BuildingManager").GetComponent<BuildingManager>();
        var aiManager = GameObject.FindGameObjectWithTag("AIManager").GetComponent<AIManager>();


        if (startPos.Length < 1)
        {
            return;
        }
        foreach (var player in players)
        {
            if (startPos == null) return;
            var spawnPos = startPos[Random.Range(0, startPos.Length - 1)].transform.position + new Vector3(Random.Range(-RandomSpawnRange, RandomSpawnRange), 0, Random.Range(-RandomSpawnRange, RandomSpawnRange));
            player.transform.position = spawnPos;
            player.SetActive(true);

            // �̰����� �Ҵ� ���ϵ��� ����
            //var ai = player.GetComponent<AIController>();
            //if (buildingManager != null)
            //    ai.point = buildingManager[Random.Range(0, buildingManager.Length - 1)].transform;
            //ai.SetDestination(ai.point);

            var ai = player.GetComponent<AIController>();
            var portrait = player.GetComponent<Portrait>();
            //if (buildingManager != null)
            //    ai.point = buildingManager.GetAttackPoint(Line.Bottom, TeamType.PC);
            //ai.SetDestination(ai.point);
            ai.spum.gameObject.AddComponent<Outlinable>();
            ai.InitInGameScene();

            portrait.SetPortrait(ai.spum);
            player.GetComponent<LookCameraByScale>().SetPlayer();
            player.GetComponent<RespawnableObject>().respawner = GameObject.FindGameObjectWithTag("Respawner").GetComponent<Respawner>();

        }

        if (players.Count > 0)
        {
            foreach (var player in players)
            {
                aiManager.pc.Add(player.GetComponent<AIController>());
            }
        }

        players.Clear();
        switch (gameType)
        {
            case GameType.Story:
                MakeEnemys();
                break;
            case GameType.Official:
                MakeTemporaryEnemys();
                break;
            case GameType.Scrimmage:
                MakeTemporaryEnemys();
                break;
        }
    }

    public void MakeEnemys()
    {
        var stateDefines = DataTableManager.instance.stateDef;
        var st = DataTableManager.instance.Get<StageTable>(DataType.Stage);
        var enemys = st.GetStageInfo(currentStage).enemys;
        for (int i = 0; i < 5; i++)
        {
            int player = enemys[i];
            EnemyInfo playerInfo = st.GetEnemyInfo(player);

            var madePlayer = Instantiate(enemyObj);
            var madePlayerCharactor = Instantiate(Resources.Load<GameObject>(Path.Combine("EnemySpum", $"{player}")), madePlayer.transform);
            madePlayerCharactor.AddComponent<LookCameraRect>();
            var outLine = madePlayerCharactor.AddComponent<Outlinable>();

            float charactorScale = madePlayer.transform.localScale.x;

            var ai = madePlayer.GetComponent<AIController>();
            ai.spum = madePlayerCharactor.GetComponent<SPUM_Prefabs>();
            var childs = madePlayerCharactor.GetComponentsInChildren<Transform>();
            foreach (var child in childs)
            {
                if (child.name == "ArmL")
                {
                    ai.leftHand = child;
                }
                else if (child.name == "ArmR")
                {
                    ai.rightHand = child;
                }

                //�ӽ��ڵ� ���߿� �ٲ����!!
                ai.firePos = ai.rightHand;
            }
            AttackDefinition atkDef = stateDefines.attackDefs.Find(a => a.code == playerInfo.atkType).value;
            AttackDefinition skillDef = stateDefines.skillDatas.Find(a => a.code == playerInfo.uniqueSkill).value;
            ai.attackInfos[0] = atkDef;
            ai.attackInfos[1] = skillDef;
            ai.kitingInfo = stateDefines.kitingDatas.Find(a => a.code == playerInfo.kitingType).value;
            ai.code = playerInfo.code;
            ai.SetInitialization();
            //ai.aiCommandInfo.SetPortraitInCommandInfo(player.code);
            ai.outlinable = outLine;

            var stat = madePlayer.GetComponent<CharacterStatus>();

            stat.AIName = playerInfo.name;
            stat.Hp = playerInfo.hp;
            stat.maxHp = stat.Hp;

            stat.speed = playerInfo.moveSpeed;
            stat.sight = playerInfo.sight;
            stat.range = playerInfo.range;

            stat.reactionSpeed = playerInfo.reaction * 15;
            stat.damage = playerInfo.atk;
            stat.cooldown = playerInfo.atkRate;
            stat.critical = playerInfo.critical;
            stat.chargeCount = playerInfo.mag;
            stat.reloadCooldown = playerInfo.reload;
            stat.accuracyRate = playerInfo.accuracy;
            stat.detectionRange = playerInfo.detection;
            stat.occupationType = (OccupationType)playerInfo.type;
            stat.distancePriorityType = DistancePriorityType.Closer;

            switch (stat.occupationType)
            {
                case OccupationType.Normal:
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 1).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 2).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 3).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 4).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 0).value);
                    break;
                case OccupationType.Assault:
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 3).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 4).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 1).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 2).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 0).value);
                    break;
                case OccupationType.Sniper:
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 4).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 3).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 2).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 1).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 0).value);
                    break;
                case OccupationType.Support:
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 1).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 2).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 3).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 4).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 0).value);
                    break;
                default:
                    break;
            }
            ai.priorityByDistance = stateDefines.distanceTargetPriorityDatas.Find(a => a.code == 0).value;

            madePlayer.SetActive(false);
            this.enemys.Add(madePlayer);
        }
        var startPos = GameObject.FindGameObjectsWithTag("EnemyStartPos");
        var endPos = GameObject.FindGameObjectsWithTag("PlayerStartPos");

        var buildingManager = GameObject.FindGameObjectWithTag("BuildingManager").GetComponent<BuildingManager>();
        var aiManager = GameObject.FindGameObjectWithTag("AIManager").GetComponent<AIManager>();


        if (startPos.Length < 1)
        {
            return;
        }
        foreach (var player in this.enemys)
        {
            if (startPos == null) return;
            var spawnPos = startPos[Random.Range(0, startPos.Length - 1)].transform.position + new Vector3(Random.Range(-RandomSpawnRange, RandomSpawnRange), 0, Random.Range(-RandomSpawnRange, RandomSpawnRange));
            player.transform.position = spawnPos;
            player.SetActive(true);

            var ai = player.GetComponent<AIController>();
            var portrait = player.GetComponent<Portrait>();
            //if (buildingManager != null)
            //    ai.point = buildingManager.GetAttackPoint(Line.Bottom, TeamType.NPC);
            //ai.SetDestination(ai.point);
            ai.spum.gameObject.AddComponent<Outlinable>();
            ai.InitInGameScene();

            portrait.SetPortrait(ai.spum);
            player.GetComponent<LookCameraByScale>().SetPlayer();
            player.GetComponent<RespawnableObject>().respawner = GameObject.FindGameObjectWithTag("Respawner").GetComponent<Respawner>();

        }

        if (this.enemys.Count > 0)
        {
            foreach (var player in this.enemys)
            {
                aiManager.npc.Add(player.GetComponent<AIController>());
            }
        }

        //aiManager.RegisterMissionTargetEvent();
    }

    public void MakeTemporaryEnemys()
    {
        var stateDefines = DataTableManager.instance.stateDef;
        var st = DataTableManager.instance.Get<StageTable>(DataType.Stage);
        var enemys = st.GetStageInfo(101).enemys;
        for (int i = 0; i < 5; i++)
        {
            int player = enemys[i];
            EnemyInfo playerInfo = st.GetEnemyInfo(player);

            var madePlayer = Instantiate(enemyObj);
            var madePlayerCharactor = Instantiate(Resources.Load<GameObject>(Path.Combine("EnemySpum", $"{player}")), madePlayer.transform);
            madePlayerCharactor.AddComponent<LookCameraRect>();
            var outLine = madePlayerCharactor.AddComponent<Outlinable>();

            float charactorScale = madePlayer.transform.localScale.x;

            var ai = madePlayer.GetComponent<AIController>();
            ai.spum = madePlayerCharactor.GetComponent<SPUM_Prefabs>();
            var childs = madePlayerCharactor.GetComponentsInChildren<Transform>();
            foreach (var child in childs)
            {
                if (child.name == "ArmL")
                {
                    ai.leftHand = child;
                }
                else if (child.name == "ArmR")
                {
                    ai.rightHand = child;
                }

                //�ӽ��ڵ� ���߿� �ٲ����!!
                ai.firePos = ai.rightHand;
            }
            AttackDefinition atkDef = stateDefines.attackDefs.Find(a => a.code == playerInfo.atkType).value;
            AttackDefinition skillDef = stateDefines.skillDatas.Find(a => a.code == playerInfo.uniqueSkill).value;
            ai.attackInfos[0] = atkDef;
            ai.attackInfos[1] = skillDef;
            ai.kitingInfo = stateDefines.kitingDatas.Find(a => a.code == playerInfo.kitingType).value;
            ai.code = playerInfo.code;
            ai.SetInitialization();
            //ai.aiCommandInfo.SetPortraitInCommandInfo(player.code);
            ai.outlinable = outLine;

            var stat = madePlayer.GetComponent<CharacterStatus>();

            stat.AIName = playerInfo.name;
            stat.Hp = playerInfo.hp;
            stat.maxHp = stat.Hp;

            stat.speed = playerInfo.moveSpeed;
            stat.sight = playerInfo.sight;
            stat.range = playerInfo.range;

            stat.reactionSpeed = playerInfo.reaction * 15;
            stat.damage = playerInfo.atk;
            stat.cooldown = playerInfo.atkRate;
            stat.critical = playerInfo.critical;
            stat.chargeCount = playerInfo.mag;
            stat.reloadCooldown = playerInfo.reload;
            stat.accuracyRate = playerInfo.accuracy;
            stat.detectionRange = playerInfo.detection;
            stat.occupationType = (OccupationType)playerInfo.type;
            stat.distancePriorityType = DistancePriorityType.Closer;

            switch (stat.occupationType)
            {
                case OccupationType.Normal:
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 1).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 2).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 3).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 4).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 0).value);
                    break;
                case OccupationType.Assault:
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 3).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 4).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 1).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 2).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 0).value);
                    break;
                case OccupationType.Sniper:
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 4).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 3).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 2).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 1).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 0).value);
                    break;
                case OccupationType.Support:
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 1).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 2).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 3).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 4).value);
                    ai.priorityByOccupation.Add(stateDefines.occupationTargetPriorityDatas.Find(a => a.code == 0).value);
                    break;
                default:
                    break;
            }
            ai.priorityByDistance = stateDefines.distanceTargetPriorityDatas.Find(a => a.code == 0).value;

            madePlayer.SetActive(false);
            this.enemys.Add(madePlayer);
        }
        var startPos = GameObject.FindGameObjectsWithTag("EnemyStartPos");
        var endPos = GameObject.FindGameObjectsWithTag("PlayerStartPos");

        var buildingManager = GameObject.FindGameObjectWithTag("BuildingManager").GetComponent<BuildingManager>();
        var aiManager = GameObject.FindGameObjectWithTag("AIManager").GetComponent<AIManager>();


        if (startPos.Length < 1)
        {
            return;
        }
        foreach (var player in this.enemys)
        {
            if (startPos == null) return;
            var spawnPos = startPos[Random.Range(0, startPos.Length - 1)].transform.position + new Vector3(Random.Range(-RandomSpawnRange, RandomSpawnRange), 0, Random.Range(-RandomSpawnRange, RandomSpawnRange));
            player.transform.position = spawnPos;
            player.SetActive(true);

            var ai = player.GetComponent<AIController>();
            var portrait = player.GetComponent<Portrait>();
            if (buildingManager != null)
                ai.point = buildingManager.GetAttackPoint(Line.Bottom, TeamType.NPC);
            ai.SetDestination(ai.point);
            ai.spum.gameObject.AddComponent<Outlinable>();
            ai.InitInGameScene();

            portrait.SetPortrait(ai.spum);
            player.GetComponent<LookCameraByScale>().SetPlayer();
            player.GetComponent<RespawnableObject>().respawner = GameObject.FindGameObjectWithTag("Respawner").GetComponent<Respawner>();

        }

        if (this.enemys.Count > 0)
        {
            foreach (var player in this.enemys)
            {
                aiManager.npc.Add(player.GetComponent<AIController>());
            }
        }

        this.enemys.Clear();
    }

    public void DeletePlayers()
    {
        foreach (var player in players)
        {
            Destroy(player);
        }
        foreach (var player in enemys)
        {
            Destroy(player);
        }
        players.Clear();
        enemys.Clear();
    }

    public void WinReward()
    {
        if (GamePlayerInfo.instance.cleardStage < currentStage)
        {
            GamePlayerInfo.instance.cleardStage = currentStage;
        }
        GamePlayerInfo.instance.AddMoney(currentStage * 30, currentStage * 10, 0);
        GamePlayerInfo.instance.GetXpItems(10, 5, 1, 0);
    }


}
