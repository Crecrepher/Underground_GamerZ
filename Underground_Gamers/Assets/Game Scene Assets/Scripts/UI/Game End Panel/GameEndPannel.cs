using System.Collections;
using System.Collections.Generic;
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
        SceneManager.LoadScene("Lobby Scene");
    }

    public void EnterNextRound()
    {
        // ���� ����� ��, BuildingManger��� 
        // GameRuleManager�� ���� ���

        // AIManager���� ���� 
        // SkillCoolTimeManager ���� �ð� �ʱ�ȭ, UI �Ű澲��
        // ��Ʈ�� ����
        // ���� ��� �г� �ִ°� ���� 
        // ĳ���� ���� ResetAI

        // ���� �ð� ����
        // ų�α� �г� �����
        // ����Ʈ �� �����
        // ���� �Ŵ��� �����ϱ�, ���ص� �ɵ�

        gameManager.aiManager.ResetAI();
        gameManager.skillCoolTimeManager.ResetSkillCooldown();
        gameManager.respawner.ClearRespawn();
        gameManager.commandManager.ResetCommandInfo();
        gameManager.buildingManager.ResetBuildings();

        // Time.timeScale =1f �Ű澲��
        gameManager.PlayingGame();

        // PlayingGame�ȿ��� ó��
        //gameManager.gameEndPannel.OffGameEndPanel();
        gameManager.battleLayoutForge.SetActiveBattleLayoutForge(true);
        gameManager.entryManager.RefreshSelectLineButton();
    }

    public void EndRound()
    {
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
