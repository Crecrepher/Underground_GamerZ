using DG.Tweening.Core.Easing;
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

    public GameObject selectOutline;

    public bool isEntry = false;
    public bool isSelected = false;

    private GameManager gameManager;

    public void SetInfo(GameManager gameManager, int index, Sprite illustration, string name, int playerHp, int playerAttack, Sprite grade, Sprite type, int level, Sprite codition, int skillLevel)
    {
        this.gameManager = gameManager;
        this.Index = index;
        playerNameText.text = $"{name}";
        illustrationIcon.sprite = illustration;
        playerHpText.text = $"{playerHp}";
        playerAttackText.text = $"{playerAttack}";
        gradeIcon.sprite = grade;
        classIcon.sprite = type;
        levelText.text = $"{level}";
        conditionIcon.sprite = codition;
        skillLevelText.text = $"{skillLevel}";
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
