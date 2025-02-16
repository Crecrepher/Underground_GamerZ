using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragBattleLayoutSlot : DragSlot
{
    public DragBattleLayoutGhostImage ghostImagePrefab;
    private DragBattleLayoutGhostImage ghostImage;

    public Image illustration;

    private Transform uiCanvas;
    private GameManager gameManager;
    private BattleLayoutForge battleLayoutForge;

    private List<AIController> currentEntry;
    public Color[] outlineColors = new Color[3];
    public Image outline;
    public Image classIcon;

    public AIController AI { get; private set; }

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        battleLayoutForge = GameObject.FindGameObjectWithTag("BattleLayoutForge").GetComponent<BattleLayoutForge>();
        uiCanvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<Transform>();
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        foreach (DragBattleLayoutSlot slot in battleLayoutForge.Slots)
        {
            slot.SetActiveAllRayCast(false);
        }
        SetActiveAllRayCast(false);
        ghostImage.SetActiveGhostImage(true);
        ghostImage.transform.position = eventData.position;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        ghostImage.transform.position = eventData.position;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        foreach (DragBattleLayoutSlot slot in battleLayoutForge.Slots)
        {
            slot.SetActiveAllRayCast(true);
        }
        ghostImage.SetActiveGhostImage(false);
    }


    public void SetActiveAllRayCast(bool isActive)
    {
        var images = GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            image.raycastTarget = isActive;
        }
    }

    public void MatchAI(AIController ai, GameManager gameManager, int index)
    {
        this.AI = ai;
        this.gameManager = gameManager;
        int grade = 0;
        if (this.gameManager.gameRuleManager.gameType == GameType.Official)
            grade = GamePlayerInfo.instance.GetOfficialPlayer(index).grade - 3;
        else
            grade = GamePlayerInfo.instance.usingPlayers[index].grade - 3;

        outline.color = outlineColors[grade];
        classIcon.sprite = AI.status.aiClass;
        AI.commandInfoOutlineColor = outlineColors[grade];
        this.battleLayoutForge = gameManager.battleLayoutForge;
        this.gameManager.uiCanvas = gameManager.uiCanvas;
        ghostImage = Instantiate(ghostImagePrefab, this.gameManager.uiCanvas);
        ghostImage.SetGhostImage(AI.status.illustration, classIcon.sprite, outlineColors[grade]);
        ghostImage.SetActiveGhostImage(false);
        RegistEntry(this.gameManager.entryManager.NoneEntryAI);
    }

    public void MatchPortrait()
    {
        illustration.sprite = AI.status.illustration;
    }

    public void SetParent(Transform parent)
    {
        gameObject.transform.SetParent(parent);
    }

    public List<AIController> GetCurrentEntry()
    {
        return currentEntry;
    }

    public void ReleaseEntry()
    {
        currentEntry.Remove(AI);
    }

    public void RegistEntry(List<AIController> entry)
    {
        currentEntry = entry;
        currentEntry.Add(AI);
    }
}
