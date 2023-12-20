using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CommandType
{
    SwitchLine,
    Defend,
    Attack,
    Count
}

public class CommandManager : MonoBehaviour
{
    [Header("ĳ��")]
    public AIManager aiManager;
    public WayPoint wayPoint;

    [Header("Ŀ��� UI")]
    public CommandInfo commandInfoPrefab;
    public Transform commandInfoTopParent;
    public Transform commandInfoBottomParent;

    [Header("AI ����")]
    public AIController prevAI;
    public AIController currentAI;
    public float selectTime;

    public GameObject selectPanel;

    [Header("���� / ���� ����")]
    public CommandButton attackButton;
    public CommandButton defendButton;

    private Queue<(Command, AIController)> records = new Queue<(Command, AIController)>();
    private List<Command> commands = new List<Command>();

    private List<CommandInfo> commandInfos = new List<CommandInfo>();
    private CommandInfo currentCommandInfo = null;
    public bool isInfoOn = true;

    public GameManager gameManager;

    public Button InfoIcon;

    public SkillModeButton skillModeButton;


    private void Awake()
    {
    }

    private void Start()
    {
        CreateCommands();
    }

    private void CreateCommands()
    {
        commands.Add(new SwitchLineCommand());
        commands.Add(new DefendCommand());
        commands.Add(new AttackCommand());
    }

    public void SetActiveCommandButton(AIController ai)
    {
        attackButton.SetActiveButton(ai.isAttack);
        defendButton.SetActiveButton(ai.isDefend);
    }

    public void ActiveAllCommandButton()
    {
        attackButton.SetActiveButton(false);
        defendButton.SetActiveButton(false);
    }

    public void SelectNewAI(AIController newAI, DragInfoSlot dragObj)
    {
        if (dragObj.isDragging)
            return;
        prevAI = currentAI;

        // ���� �� ����
        if (prevAI == newAI && newAI != null)
        {
            UnSelect();
            return;
        }

        if (prevAI != null)
        {
            prevAI.UnSelectAI();
        }

        currentAI = newAI;
        currentAI.SelectAI();
        skillModeButton.RefreshUsedSkillCoolTime();
        skillModeButton.SetActiveSkillModeButton(true);
        skillModeButton.SetAI(currentAI);
        skillModeButton.SetPriorSkill(currentAI.isPrior);
        if(skillModeButton.IsAutoMode)
        {
            if (currentAI.isOnCoolOriginalSkill)
            {
                skillModeButton.SetActiveCoolTimeText(false);
                skillModeButton.SetActiveCoolTimeFillImage(false);
            }
            else
            {
                skillModeButton.SetActiveCoolTimeText(false);
                skillModeButton.SetActiveCoolTimeFillImage(true);
            }
        }
        else
        {
            if(currentAI.isOnCoolOriginalSkill)
            {
                skillModeButton.SetActiveCoolTimeFillImage(false);
                skillModeButton.SetActiveCoolTimeText(false);
            }
            else
            {
                skillModeButton.SetActiveCoolTimeFillImage(true);
                skillModeButton.SetActiveCoolTimeText(true);
            }
        }

        // ���� 0 ��ġ�� ���� �г�
        selectPanel.SetActive(true);

        // ī�޶� ����
        if (newAI.status.IsLive)
            gameManager.cameraManager.StartZoomIn();

        //Time.timeScale = selectTime;
    }

    public void UnSelect()
    {
        currentAI.UnSelectAI();
        currentAI = null;
        skillModeButton.SetAI(null);
        skillModeButton.SetPriorSkill(false);
        skillModeButton.SetActiveSkillModeButton(false);

        //Time.timeScale = 1f;
        selectPanel.SetActive(false);
        gameManager.cameraManager.StartZoomOut();
    }

    public void CreateCommandUI()
    {
        int pcNum = 1;

        foreach (var ai in aiManager.pc)
        {
            Transform parent = ai.currentLine switch
            {
                Line.Bottom => commandInfoBottomParent,
                Line.Top => commandInfoTopParent,
                _ => commandInfoBottomParent
            };
            CommandInfo info = Instantiate(commandInfoPrefab, parent);
            ai.aiCommandInfo = info;
            ai.aiCommandInfo.aiName.text = ai.status.AIName;
            info.aiController = ai;
            info.SetClassIcon(ai.status.aiClass);

            // �ʻ�ȭ ����
            var portrait = info.portrait;
            var conditionIcon = info.conditionIcon;
            var conditions = info.conditions;

            // �׽�Ʈ �ڵ�, 0~3, Lobby���� ������ ��� ���� / ����� �Է�
            portrait.sprite = DataTableManager.instance.Get<PlayerTable>(DataType.Player).GetPlayerSprite(ai.code);
            conditionIcon.sprite = conditions[ai.status.condition];

            // �ؽ�Ʈ ������
            info.name = $"{info.name}{pcNum}";
            info.aiType = "PC";
            info.aiNum = pcNum++;

            info.ResetKillCount();
            commandInfos.Add(info);


            // ��ư ��� �ο�, ĳ���� ����
            var infoButton = info.GetComponent<Button>();
            DragInfoSlot dragObj = infoButton.GetComponent<DragInfoSlot>();
            infoButton.onClick.AddListener(() => SelectNewAI(info.aiController, dragObj));
        }
    }

    public void ExecuteSwitchLine(AIController ai)
    {
        switch (ai.teamIdentity.teamType)
        {
            case TeamType.PC:
                commands[(int)CommandType.SwitchLine].ExecuteCommand(aiManager.pc[ai.aiIndex], wayPoint);
                break;
            case TeamType.NPC:
                commands[(int)CommandType.SwitchLine].ExecuteCommand(aiManager.npc[ai.aiIndex], wayPoint);
                break;
        }
    }
    public void ExecuteCommand(CommandType type, AIController ai)
    {
        commands[(int)type].ExecuteCommand(ai, wayPoint);
    }

    public void ClickInfoButton()
    {
        isInfoOn = !isInfoOn;
        SetActiveStatusInfo(isInfoOn);
    }

    public void SetActiveStatusInfo(bool isActive)
    {
        foreach (var info in commandInfos)
        {
            info.statusInfo.SetActive(isActive);
        }
    }
}
