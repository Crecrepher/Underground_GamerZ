using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedBuilding : MonoBehaviour, IDestroyable
{
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    public void DestoryObject(GameObject attacker)
    {
        Building building = GetComponent<Building>();
        TeamIdentifier identity = GetComponent<TeamIdentifier>();

        // publish
        // ����List ����
        BuildingEventBus.Publish(transform);
        // �ش� ���� AI ����
        building.PublishMissionTargetEvent();
        // ���� EventBus ����
        gameManager.buildingManager.UnsubscribeDestroyEvent(transform);
        gameManager.gameRuleManager.ReleaseBuilding(identity);
    }
}
