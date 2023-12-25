using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndPannel : MonoBehaviour
{
    public GameManager gameManager;

    public TextMeshProUGUI winText;
    public TextMeshProUGUI LoseText;

    public GameObject rewardPanel;
    [Header("�� �׷��� �г�")]
    public GameObject damageGraphPanel;
    public GameObject nextRoundButton;
    public GameObject okButton;
    public GameObject endRoundButton;

    public Transform rewardParent;
    public Transform damageGraphParent;

    public AIReward rewardPrefab;
    public DamageGraph damageGraphPrefab;

    private float maxDealtDamage = 0;
    private float maxTakenDamage = 0;
    private float maxHealAmount = 0;

    public void ResetDamageGraph()
    {
        maxDealtDamage = 0;
        maxTakenDamage = 0;
        maxHealAmount = 0;
    }

    public void ResetTotalKillCount()
    {
        var npcKillCounttext = GameObject.FindGameObjectWithTag("NPC_Score").GetComponent<TMP_Text>();
        npcKillCounttext.text = $"{0}";
        var pcKillCounttext = GameObject.FindGameObjectWithTag("PC_Score").GetComponent<TMP_Text>();
        pcKillCounttext.text = $"{0}";
    }

    public void OnGameEndPanel()
    {
        gameObject.SetActive(true);
        if (gameManager.gameRuleManager.WinningCount == 1)
        {
            OffDamageGraph();
            OnRewardPanel();
        }
        else
        {
            OnDamageGraph();
            OffRewardPanel();
        }
        CreateAIReward();
        CreateDamageGraph();

        // ���� �ϱ���
        //gameManager.aiRewardManager.DisplayRewardResult();

        // ���� ��
        if (gameManager.IsRoundWin)
        {
            GameInfo.instance.WinReward();
            gameManager.aiRewardManager.DisplayFillXpGauage();
        }

        // ���� ��
        // �ִϸ��̼� ���� �� �б� �ʿ�
        //gameManager.aiRewardManager.DisplayRewardResult();
        GamePlayerInfo.instance.ClearLvUpCount();
    }

    public void CreateAIReward()
    {
        foreach (var pc in gameManager.aiManager.pc)
        {
            AIReward aiReward = Instantiate(rewardPrefab, rewardParent);
            //�̸�
            aiReward.aiNameText.text = $"{pc.status.AIName}";
            aiReward.lvText.text = $"Lv. {pc.status.lv}";
            aiReward.illustration.sprite = pc.status.illustration;
            aiReward.aiClass.sprite = pc.status.aiClass;
            aiReward.grade.sprite = pc.status.grade;
            aiReward.currentXP = pc.status.xp;
            aiReward.maxXP = pc.status.maxXp;
            aiReward.ai = pc;
            gameManager.aiRewardManager.rewards.Add(aiReward);
        }
    }

    public void CreateDamageGraph()
    {
        foreach (var pc in gameManager.aiManager.pc)
        {
            DamageGraph damageGraph = Instantiate(damageGraphPrefab, damageGraphParent);
            // ���⼭ �̹��� �ޱ�
            pc.damageGraph = damageGraph;
            damageGraph.illustration.sprite = pc.status.illustration;
            damageGraph.dealtDamage = pc.status.dealtDamage;
            damageGraph.takenDamage = pc.status.takenDamage;
            damageGraph.healAmount = pc.status.healAmount;

            if (maxDealtDamage < damageGraph.dealtDamage)
            {
                maxDealtDamage = damageGraph.dealtDamage;
            }

            if (maxTakenDamage < damageGraph.takenDamage)
            {
                maxTakenDamage = damageGraph.takenDamage;
            }

            if (maxHealAmount < damageGraph.healAmount)
            {
                maxHealAmount = damageGraph.healAmount;
            }
        }

        foreach (var pc in gameManager.aiManager.pc)
        {
            pc.damageGraph.SetMaxDamages(maxDealtDamage, maxTakenDamage, maxHealAmount);
            pc.damageGraph.DisplayDamges();
        }
    }

    public void EnterLobby()
    {
        GameInfo.instance.ClearEntryPlayer();
        GameInfo.instance.ClearMembersIndex();
        gameManager.aiRewardManager.ClearRewards();
        SceneManager.LoadScene("Lobby Scene");
    }

    public void ClearEntry()
    {
        GameInfo.instance.ClearEntryPlayer();
        GameInfo.instance.ClearMembersIndex();
    }

    public void EnterNextRound()
    {
        gameManager.aiManager.ResetAI();
        gameManager.skillCoolTimeManager.ResetSkillCooldown();
        gameManager.respawner.ClearRespawn();
        gameManager.commandManager.ResetCommandInfo();
        gameManager.buildingManager.ResetBuildings();

        // Time.timeScale =1f �Ű澲��
        gameManager.PlayingGame();
        ResetDamageGraph();
        ResetTotalKillCount();

        gameManager.entryPanel.SetActiveEntryPanel(true);
        // �̰����� entryPanel�� EntryMembers�ʱ�ȭ ���־����, ������
        GameInfo.instance.ClearEntryPlayer();
        GameInfo.instance.ClearMembersIndex();

        // NextRound �� �Ź� �Ǿ�� ��
        gameManager.lineManager.ResetAllLines();
        gameManager.commandManager.UnSelect();
        gameManager.aiRewardManager.ClearRewards();
    }

    public void EndRound()
    {
        GameInfo.instance.ClearEntryPlayer();
        GameInfo.instance.ClearMembersIndex();
        gameManager.aiManager.ResetAI();
        gameManager.aiRewardManager.ClearRewards();
        GamePlayerInfo.instance.CalculateOfficialPlayer(gameManager.IsGameWin,
            gameManager.gameRuleManager.PCWinEvidenceCount,
            gameManager.gameRuleManager.NPCWinEvidenceCount);
        SceneManager.LoadScene("Lobby Scene");
    }

    public void OffGameEndPanel()
    {
        gameObject.SetActive(false);
    }

    public void OnDamageGraph()
    {
        damageGraphPanel.SetActive(true);
        SetActiveDamageOkButton(gameManager.gameRuleManager.WinningCount == 1);
        SetActiveDamageNextRoundButton(!gameManager.IsRoundEnd && gameManager.gameRuleManager.WinningCount != 1);
        SetActiveDamageEndRoundButton(gameManager.IsRoundEnd && gameManager.gameRuleManager.WinningCount != 1);
    }

    public void OnRewardPanel()
    {
        rewardPanel.SetActive(true);
    }
    public void OffDamageGraph()
    {
        damageGraphPanel.SetActive(false);
    }

    public void SetActiveDamageOkButton(bool isActive)
    {
        okButton.SetActive(isActive);
    }
    public void SetActiveDamageNextRoundButton(bool isActive)
    {
        nextRoundButton.SetActive(isActive);
    }
    public void SetActiveDamageEndRoundButton(bool isActive)
    {
        endRoundButton.SetActive(isActive);
    }

    public void OffRewardPanel()
    {
        rewardPanel.SetActive(false);
    }
}
