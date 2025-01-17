using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerRecruitRecruiting : ManagerRecruit
{
    [SerializeField]
    private List<Toggle> toggleList;

    [Space(5f)]
    [Header("Recruit")]
    public Image recruitImage;
    public TMP_Text recruitInfo;
    [SerializeField]
    private int defaultRecruitCode = 0;
    private int currCode = 0;
    private int currCount = 0;
    private int maxCount = 1;
    private List<int> outPut;
    [SerializeField]
    private TMP_Text MoneyText1;
    [SerializeField]
    private TMP_Text MoneyText10;
    [SerializeField]
    private GameObject recruitCheckWindow;
    [SerializeField]
    private TMP_Text recruitCheckWindowMoney;
    [SerializeField]
    private TMP_Text recruitCheckWindowMoneyCurr;
    [SerializeField]
    private GameObject spaceLackWarning;
    [SerializeField]
    private GameObject moneyWarning;
    [SerializeField]
    private TMP_Text moneyWarningWindowMoney;
    [SerializeField]
    private TMP_Text moneyWarningWindowMoneyCurr;

    [Space(5f)]
    [Header("RecruitEffect")]
    [SerializeField]
    private GameObject recruitCardBoard;
    [SerializeField]
    private GameObject recruitEffect;
    public GameObject recruitEffrctPrefabNomal;
    public GameObject recruitEffrctPrefabRare;
    public GameObject recruitEffrctPrefabNormalPass;
    [SerializeField]
    private Image recruitEffrctCharImage;
    [SerializeField]
    private Image recruitEffrctTypeImage;
    [SerializeField]
    private Transform recruitEffrctPos;
    [SerializeField]
    public GameObject recruitEffrctWindow;
    [SerializeField]
    private Image recruitEffrctStars;
    [SerializeField]
    private TMP_Text recruitEffrctName;
    [SerializeField]
    private Transform recruitCardPos;
    public GameObject recruitCardPrefab;

    private List<GameObject> oldRecruitCards = new List<GameObject>();
    [SerializeField]
    private GameObject recruitCardEffetRare;
    [SerializeField]
    private GameObject recruitCardEffetUnique;

    private int recruitCount = 0;
    private RecruitTable rt;
    private PlayerTable pt;
    private StringTable st;


    [SerializeField]
    private PopupRecruitChanceInfo popupRecruitChanceInfo;

    public override void OnEnter()
    {
        currCode = defaultRecruitCode;
        gameObject.SetActive(true);
        toggleList[0].isOn = true;
        ResetIndex();
        UpdateMoneyInfo();
    }

    public override void OnExit()
    {
        gameObject.SetActive(false);
        LobbyUIManager.instance.UpdateMoneyInfo();
    }

    public void ResetIndex()
    {
        ShowIndex(currCode);
        UpdateMoneyInfo();
    }

    public void ShowIndex(int code)
    {
        currCode = code;
        if (rt == null || pt == null || st == null)
        {
            rt = DataTableManager.instance.Get<RecruitTable>(DataType.Recruit);
            pt = DataTableManager.instance.Get<PlayerTable>(DataType.Player);
            st = DataTableManager.instance.Get<StringTable>(DataType.String);
        }
        RecruitInfo info = rt.GetRecruitInfo(currCode.ToString());

        recruitImage.sprite = Resources.Load<Sprite>(Path.Combine("RecruitSprite", currCode.ToString()));
        recruitInfo.text = st.Get($"recruit_info_{currCode}");
        MoneyText1.text = $"<sprite=1> : {info.crystal}";
        MoneyText10.text = $"<sprite=1>  : {info.crystal * 10}";
    }

    public void TryRecruit(int count)
    {
        recruitCount = count;
        RecruitInfo info = rt.GetRecruitInfo(currCode.ToString());
        int needMoney = info.money * recruitCount;
        int needCrystal = info.crystal * recruitCount;
        int needTicket = info.contractTicket * recruitCount;

        int usingList = 0;
        List<Player> used = GamePlayerInfo.instance.usingPlayers;
        foreach (var item in used)
        {
            if (item.code >= 0)
            {
                usingList++;
            }
        }

        if (!GamePlayerInfo.instance.CheckMoney(needMoney, needCrystal, needTicket))
        {
            string messege = "";
            string submessege = st.Get("recruitMoneyLackMessegeCurr");
            if (needMoney - GamePlayerInfo.instance.money > 0)
            {
                messege += $"<sprite=0> {needMoney - GamePlayerInfo.instance.money}{st.Get("count")}";
                submessege += $"<sprite=0> {GamePlayerInfo.instance.money}{st.Get("count")} ";
            }
            if (needCrystal - GamePlayerInfo.instance.crystal > 0)
            {
                messege += $"<sprite=1> {needCrystal - GamePlayerInfo.instance.crystal}{st.Get("count")}";
                submessege += $"<sprite=1> {GamePlayerInfo.instance.crystal}{st.Get("count")}"; ;
            }
            if (needTicket - GamePlayerInfo.instance.contractTicket > 0)
            {
                messege += $"<sprite=2> {needTicket - GamePlayerInfo.instance.contractTicket}{st.Get("count")}";
                submessege += $"<sprite=2> {GamePlayerInfo.instance.contractTicket}{st.Get("count")}";
            }
            messege += st.Get("recruitMoneyLackMessege");
            moneyWarningWindowMoney.text = messege;
            moneyWarningWindowMoneyCurr.text = submessege;
            moneyWarning.SetActive(true);
            return;
        }
        else if (GamePlayerInfo.instance.havePlayers.Count + count + usingList > 200)
        {
            spaceLackWarning.SetActive(true);
            return;
        }
        else
        {
            string messege = "";
            string submessege = st.Get("recruitCheckCurrMessege");
            if (needMoney > 0)
            {
                messege += $"<sprite=0> {needMoney}{st.Get("count")}";
                submessege += $"<sprite=0> {GamePlayerInfo.instance.money}{st.Get("count")} ";
            }
            if (needCrystal > 0)
            {
                messege += $"<sprite=1> {needCrystal}{st.Get("count")}";
                submessege += $"<sprite=1> {GamePlayerInfo.instance.crystal}{st.Get("count")}"; ;
            }
            if (needTicket > 0)
            {
                messege += $"<sprite=2> {needTicket}{st.Get("count")}";
                submessege += $"<sprite=2> {GamePlayerInfo.instance.contractTicket}{st.Get("count")}";
            }
            messege += st.Get("recruitCheckMessege");
            recruitCheckWindowMoney.text = messege;
            recruitCheckWindowMoneyCurr.text = submessege;
            recruitCheckWindow.SetActive(true);
            return;
        }
    }

    public void StartRecruit()
    {
        SoundPlayer.instance.PauseMusic();
        RecruitInfo info = rt.GetRecruitInfo(currCode.ToString());
        if (!GamePlayerInfo.instance.UseMoney(info.money * recruitCount, info.crystal * recruitCount, info.contractTicket * recruitCount))
        {
            return;
        }
        foreach (var card in oldRecruitCards)
        {
            Destroy(card.gameObject);
        }
        oldRecruitCards.Clear();



        outPut = rt.RecruitRandom(currCode, recruitCount);
        currCount = recruitCount;
        bool isHaveUnique = false;
        foreach (int i in outPut)
        {
            GamePlayerInfo.instance.AddPlayer(i);
            GamePlayerInfo.instance.AddMoney(0, 0, 1);

            var card = Instantiate(recruitCardPrefab, recruitCardPos);
            var rc = card.GetComponent<RecruitCards>();
            rc.image.sprite = pt.GetPlayerSprite(i);
            PlayerInfo playerInfo = pt.playerDatabase[i];
            rc.stars.sprite = playerInfo.grade switch
            {
                3 => pt.starsSprites[0],
                4 => pt.starsSprites[1],
                5 => pt.starsSprites[2],
                _ => pt.starsSprites[0],
            };
            int grade = pt.GetPlayerInfo(i).grade;
            if (grade >= 5)
            {
                var effect = Instantiate(recruitCardEffetUnique, card.transform);
                effect.transform.SetSiblingIndex(1);
                isHaveUnique = true;
            }
            else if (grade >= 4)
            {
                var effect = Instantiate(recruitCardEffetRare, card.transform);
                effect.transform.SetSiblingIndex(1);
            }
            oldRecruitCards.Add(card);
        }

        if (isHaveUnique)
        {
            Instantiate(recruitEffrctPrefabRare, recruitEffrctPos);
        }
        else
        {
            Instantiate(recruitEffrctPrefabNomal, recruitEffrctPos);
        }

        RecruitEffrctNextPlayer();

        recruitCardBoard.SetActive(true);
        recruitEffect.SetActive(true);
        UpdateMoneyInfo();
        LobbyUIManager.instance.UpdateMoneyInfo();
    }

    public void RecruitEffrctNextPlayer()
    {
        Instantiate(recruitEffrctPrefabNormalPass, recruitEffrctPos);
        if (currCount == 0)
        {
            recruitEffrctWindow.SetActive(false);
            return;
        }
        currCount--;
        int currMakeCode = outPut[currCount];
        PlayerInfo pi = pt.GetPlayerInfo(currMakeCode);
        recruitEffrctCharImage.sprite = pt.GetPlayerFullSprite(currMakeCode);
        recruitEffrctTypeImage.sprite = Resources.Load<Sprite>(Path.Combine("PlayerType", pi.type.ToString()));
        recruitEffrctName.text = st.Get($"playerName{pi.code}");
        recruitEffrctStars.sprite = pi.grade switch
        {
            3 => pt.starsSprites[0],
            4 => pt.starsSprites[1],
            5 => pt.starsSprites[2],
            _ => pt.starsSprites[0],
        };
        Canvas.ForceUpdateCanvases();
    }

    public void UpdateMoneyInfo()
    {
        RecruitUIManager.instance.lobbyTopMenu.UpdateMoney();
        LobbyUIManager.instance.UpdateMoneyInfo();
    }

    public void RestartBGM()
    {
        SoundPlayer.instance.ResumeMusic();
    }


    public void OpenChanceInfo()
    {
        int infoCode = currCode switch
        {
            10000 => 0,
            11000 => 1,
            12000 => 2,
            _ => 3,
        };
        popupRecruitChanceInfo.OpenChanceInfo(infoCode);
    }
}
