using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EntryPlayer : MonoBehaviour, IPointerClickHandler
{
    public int Index { get; private set; }

    public TextMeshProUGUI playerNameText;
    public Image illustrationIcon;
    public TextMeshProUGUI playerHpText;
    public TextMeshProUGUI playerAttackText;
    public Image gradeIcon;
    public Image classIcon;
    public TextMeshProUGUI levelText;
    public Image conditionIcon;
    public TextMeshProUGUI skillLevelText;
    public Image skillIcon;
    public TextMeshProUGUI skillNameText;

    public GameObject selectOutline;
    public Image bg;

    public bool isEntry = false;
    public bool isSelected = false;

    private GameManager gameManager;
    public Color[] outlineColors = new Color[3];

    public void SetInfo(GameManager gameManager, int index, Sprite illustration, string name, int playerHp,
        int playerAttack, int grade, Sprite type, int level, Sprite codition, int skillLevel, Sprite skillIcon, string skillName)
    {
        this.gameManager = gameManager;
        this.Index = index;
        playerNameText.text = $"{name}";
        illustrationIcon.sprite = illustration;
        playerHpText.text = $"{this.gameManager.str.Get("hp")} {playerHp}";
        playerAttackText.text = $"{this.gameManager.str.Get("atk")} {playerAttack}";
        gradeIcon.sprite = this.gameManager.pt.starsSprites[grade];
        SetBgColor(grade);
        classIcon.sprite = type;
        levelText.text = $"{this.gameManager.str.Get("lv")} {level}";
        conditionIcon.sprite = codition;
        skillLevelText.text = $"{this.gameManager.str.Get("skill lv")} {skillLevel}";
        this.skillIcon.sprite = skillIcon;
        skillNameText.text = skillName;
    }

    public void SetConditionIcon(Sprite conditionIcon)
    {
        this.conditionIcon.sprite = conditionIcon;
        //conditionIcon.sprite = GamePlayerInfo.instance.GetOfficialPlayer(Index).condition;
    }

    public void SetBgColor(int grade)
    {
        bg.color = outlineColors[grade];
    }
    public void SetActiveSelectOutline(bool isActive)
    {
        selectOutline.SetActive(isActive);
    }
    public void ClickEntryPlayer()
    {
        if (isEntry)
        {
            ClickEntryMember();
        }
        else
            ClickBenchMember();

        gameManager.entryPanel.SwapEntryMember();
    }

    public void ClickEntryMember()
    {
        if (gameManager.entryPanel.selectedEntryMember == this) // ���� ����� �ٽ� Ŭ���� ���
        {
            gameManager.entryPanel.selectedEntryMember.SetActiveSelectOutline(false); // ���� ����
            gameManager.entryPanel.selectedEntryMember = null;
        }
        else // �ٸ� ����� Ŭ���� ���
        {
            if (gameManager.entryPanel.selectedEntryMember != null)
            {
                gameManager.entryPanel.selectedEntryMember.SetActiveSelectOutline(false); // ���� ��� ���� ����
            }

            gameManager.entryPanel.selectedEntryMember = this; // ���ο� ��� ����
            gameManager.entryPanel.selectedEntryMember.SetActiveSelectOutline(true); // ������ ��� Ȱ��ȭ
        }
    }

    public void ClickBenchMember()
    {
        if (gameManager.entryPanel.selectedBenchMember == this) // ���� ����� �ٽ� Ŭ���� ���
        {
            gameManager.entryPanel.selectedBenchMember.SetActiveSelectOutline(false); // ���� ����
            gameManager.entryPanel.selectedBenchMember = null;
        }
        else // �ٸ� ����� Ŭ���� ���
        {
            if (gameManager.entryPanel.selectedBenchMember != null)
            {
                gameManager.entryPanel.selectedBenchMember.SetActiveSelectOutline(false); // ���� ��� ���� ����
            }

            gameManager.entryPanel.selectedBenchMember = this; // ���ο� ��� ����
            gameManager.entryPanel.selectedBenchMember.SetActiveSelectOutline(true); // ������ ��� Ȱ��ȭ
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ClickEntryPlayer();
    }
}
