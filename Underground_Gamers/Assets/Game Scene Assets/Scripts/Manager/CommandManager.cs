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
    public Button commandButtonPrefab;

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
    public bool isCheckInfo = false;

    public GameManager gameManager;


    private void Awake()
    {
    }

    private void Start()
    {
        CreateCommands();
        CreateCommandUI();
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

    public void SelectNewAI(AIController newAI, DragAndDrop dragObj)
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
        //Time.timeScale = 1f;
        selectPanel.SetActive(false);
        gameManager.cameraManager.StartZoomOut();
    }

    private void CreateCommandUI()
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
            info.aiController = ai;

            //var text = info.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            //text.text = $"{info.aiType}{info.aiNum}";

            // �ʻ�ȭ ����
            var portrait = info.portrait.GetComponent<Image>();

            // �׽�Ʈ �ڵ�, 0~3, Lobby���� ������ ��� ����
            portrait.sprite = DataTableManager.instance.Get<PlayerTable>(DataType.Player).GetPlayerSprite(ai.code);

            // �ؽ�Ʈ ������
            info.name = $"{info.name}{pcNum}";
            info.aiType = "PC";
            info.aiNum = pcNum++;
            commandInfos.Add(info);

            // ��ư ��� �ο�, ĳ���� ����
            var infoButton = info.GetComponent<Button>();
            DragAndDrop dragObj = infoButton.GetComponent<DragAndDrop>();
            infoButton.onClick.AddListener(() => SelectNewAI(info.aiController, dragObj));
            //infoButton.onClick.AddListener(info.aiController.SelectAI);



            // ��� �Է�
            //var infoButotn = info.GetComponent<Button>();
            //infoButotn.onClick.AddListener(() => OnCommadns(info));

            // Ŀ��� �ֱ�
            //Transform commandParent = info.transform.GetChild(0);

            //for (int i = 0; i < commands.Count; ++i)
            //{
            //    int index = i;
            //    Button commandButton = Instantiate(commandButtonPrefab, commandParent);
            //    var commandID = commandButton.GetComponentInChildren<TextMeshProUGUI>();
            //    commandID.text = $"{(CommandType)i}";

            //    // ��� �Է�
            //    commandButton.onClick.AddListener(() => commands[index].ExecuteCommand(info.aiController, wayPoint));
            //    commandButton.gameObject.SetActive(false);
            //    info.commandButtons.Add(commandButton);
            //}
        }
    }

    //public void ExecuteSwitchLine(int aiIndex)
    //{
    //    commands[(int)CommandType.SwitchLine].ExecuteCommand(aiManager.pc[aiIndex], wayPoint);
    //}
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

    public void OnCommadns(CommandInfo commandInfo)
    {
        //DragAndDrop dragAndDrop = commandInfo.GetComponent<DragAndDrop>();

        //OffAllCommands();
        //if (dragAndDrop != null)
        //{
        //    if (currentCommandInfo == commandInfo && !dragAndDrop.isDragging)
        //        isCheckInfo = true;
        //    else if(currentCommandInfo != commandInfo && !dragAndDrop.isDragging)
        //        isCheckInfo = false;
        //}
        //else
        //{
        //    if (currentCommandInfo == commandInfo)
        //        isCheckInfo = true;
        //    else
        //        isCheckInfo = false;
        //}


        //currentCommandInfo = commandInfo;
        //foreach (var button in currentCommandInfo.commandButtons)
        //{
        //    button.gameObject.SetActive(true);
        //}

        //if (isCheckInfo)
        //{
        //    OffCurrentCommads();
        //}

    }

    public void OffAllCommands()
    {
        //foreach(var info in commandInfos)
        //{
        //    foreach(var button in info.commandButtons)
        //    {
        //        button.gameObject.SetActive(false);
        //    }
        //}
    }

    public void OffCurrentCommads()
    {
        //foreach(var info in currentCommandInfo.commandButtons)
        //{
        //    info.gameObject.SetActive(false);
        //}
        //currentCommandInfo = null;
    }
}
