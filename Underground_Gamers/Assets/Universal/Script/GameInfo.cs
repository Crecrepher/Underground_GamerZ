using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

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
    public static int currentStage = 0;
    public float RandomSpawnRange = 1f;

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
    public void RegistPlayers()
    {
        var stateDefines = DataTableManager.instance.stateDef;
        List<Player> usePlayer = GamePlayerInfo.instance.usingPlayers;
        for (int i = 0; i < 5; i++)
        {
            var player = usePlayer[i];
            PlayerInfo playerInfo = pt.GetPlayerInfo(player.code);
            var madePlayer = Instantiate(playerObj);
            madePlayer.AddComponent<DontDestroy>();
            var madePlayerCharactor = Instantiate(Resources.Load<GameObject>(Path.Combine("SPUM", $"{player.code}")), madePlayer.transform);
            madePlayerCharactor.AddComponent<LookCameraRect>();

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

                //임시코드 나중에 바꿔야함!!
                ai.firePos = ai.rightHand;
            }
            AttackDefinition atkDef = stateDefines.attackDefs.Find(a => a.code == playerInfo.atkType).value;
            ai.attackInfos[0] = atkDef;
            ai.kitingInfo = stateDefines.kitingDatas.Find(a => a.code == playerInfo.kitingType).value;

            var stat = madePlayer.GetComponent<CharacterStatus>();
            stat.Hp = playerInfo.hp;
            stat.maxHp = playerInfo.hp;
            stat.speed = playerInfo.moveSpeed;
            stat.sight = playerInfo.sight;
            stat.range = playerInfo.range/2f;
            stat.reactionSpeed = playerInfo.reactionSpeed * 15;
            stat.damage = playerInfo.atk;
            stat.cooldown = playerInfo.atkRate;
            stat.critical = playerInfo.criticalChance;
            stat.chargeCount = playerInfo.magazine;
            stat.reloadCooldown = playerInfo.reloadingSpeed;
            stat.accuracyRate = playerInfo.Accuracy;

            madePlayer.SetActive(false);
            players.Add(madePlayer);
        }
    }

    public void MakePlayers()
    {
        var startPos = GameObject.FindGameObjectsWithTag("PlayerStartPos");
        var endPos = GameObject.FindGameObjectsWithTag("EnemyStartPos");
        var playerDestinations = GameObject.FindGameObjectsWithTag("NPCTower");

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

            var ai = player.GetComponent<AIController>();
            if (playerDestinations != null)
                ai.point = playerDestinations[Random.Range(0, playerDestinations.Length - 1)].transform;
            ai.SetDestination(ai.point.position);

            player.GetComponent<RespawnableObject>().respawner = GameObject.FindGameObjectWithTag("Respawner").GetComponent<Respawner>();
        }

        if (players.Count > 0)
        {
            foreach (var player in players)
            {
                GameObject.FindGameObjectWithTag("AIManager").GetComponent<AIManager>().pc.Add(player.GetComponent<AIController>());
            }
        }
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
}
