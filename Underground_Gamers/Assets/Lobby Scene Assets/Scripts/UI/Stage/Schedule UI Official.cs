using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScheduleUIOfficial : ScheduleUISubscriber
{
    [SerializeField]
    private GameObject UI_OfficialSelect;
    [SerializeField]
    private Button[] levelButtons = new Button[4];
    [SerializeField]
    private int[] lastLevels = new int[4];
    [SerializeField]
    private TMP_Text popupStartOfficialText;

    [SerializeField]
    private GameObject UI_OfficialMain;
    [SerializeField]
    private OfficialTeamTable[] officialTeamTables = new OfficialTeamTable[8];
    [SerializeField]
    private OfficialPlayerTable[] officialPlayerTables = new OfficialPlayerTable[8];
    private int officialLevel = -1;
    [SerializeField]
    private TMP_Text[] nextLeagueInfo = new TMP_Text[8];
    [SerializeField]
    private Image[] nextLeagueInfoBack = new Image[8];

    private StringTable st;

    public SceneLoader sceneLoader;
    public override void OnEnter()
    {
        base.OnEnter();
        if (GamePlayerInfo.instance.isOnOfficial)
        {
            UpdateOfficialMain();
            UI_OfficialSelect.SetActive(false);
            UI_OfficialMain.SetActive(true);
        }
        else
        {
            int currlevel = GamePlayerInfo.instance.cleardStage;
            for (int i = 0; i < 4; i++)
            {
                levelButtons[i].interactable = currlevel >= lastLevels[i];
            }
            UI_OfficialSelect.SetActive(true);
            UI_OfficialMain.SetActive(false);
        }
        lobbyTopMenu.AddFunction(OnBack);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
    private void OnBack()
    {
        scheduleUIManager.OpenWindow(1);
    }

    public void SaveOfficialLevel(int level)
    {
        if (st == null)
        {
            st = DataTableManager.instance.Get<StringTable>(DataType.String);
        }
        officialLevel = level;
        popupStartOfficialText.text = level switch
        {
            1 => st.Get("2nd"),
            2 => st.Get("1st"),
            3 => st.Get("champions"),
            _ => st.Get("3rd"),
        };
        popupStartOfficialText.text += " "+st.Get("official_entry_warning1") + "\n\n<color=\"yellow\">" + st.Get("official_entry_warning2") + "</color>";


    }

    public void StartOfficial()
    {
        GamePlayerInfo.instance.OfficialMakeEnemyTeams(officialLevel);
        UpdateOfficialMain();
        lobbyTopMenu.ExecuteFunction();
        UI_OfficialSelect.SetActive(false);
        UI_OfficialMain.SetActive(true);
    }

    public void UpdateOfficialMain()
    {
        OfficialTeamData[] sortedArray = GamePlayerInfo.instance.OfficialTeamRankSort();
        for (int i = 0; i < officialTeamTables.Length; i++)
        {
            officialTeamTables[i].SetInfos(sortedArray[i], i);
            officialPlayerTables[i].SetInfos(GamePlayerInfo.instance.officialPlayerDatas[i], i);
        }

        if (GamePlayerInfo.instance.officialWeekNum < 7)
        {
            for (int i = 0; i < nextLeagueInfo.Length; i++)
            {
                nextLeagueInfo[i].gameObject.SetActive(true);
                OfficialTeamData currentData = GamePlayerInfo.instance.officialTeamDatas[GamePlayerInfo.instance.officialMatchInfo[GamePlayerInfo.instance.officialWeekNum, i]];
                nextLeagueInfo[i].text = currentData.name;
                if (currentData.isPlayer)
                {
                    nextLeagueInfoBack[i].color = Color.yellow;
                }
                else
                {
                    nextLeagueInfoBack[i].color = Color.white;
                }
            }
        }
        else
        {
            for (int i = 2; i < nextLeagueInfo.Length; i++)
            {
                nextLeagueInfo[i].gameObject.SetActive(false);
            }
            int finalIndex = GamePlayerInfo.instance.officialWeekNum switch
            {
                7 => 3,
                8 => 2,
                9 => 1,
                _ => 4
            };
            nextLeagueInfo[0].text = GamePlayerInfo.instance.officialTeamDatas[finalIndex].name;
            nextLeagueInfo[1].text = GamePlayerInfo.instance.officialTeamDatas[finalIndex - 1].name;
        }

    }

    public void StartGame()
    {
        GameInfo.instance.gameType = GameType.Official;
        GamePlayerInfo.instance.endScrimmage = true;
        //SceneManager.LoadScene("Game Scene");
        sceneLoader.SceneLoad("Game Scene");
    }
}
