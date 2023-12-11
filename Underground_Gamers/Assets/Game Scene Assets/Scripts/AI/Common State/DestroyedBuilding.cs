using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedBuilding : MonoBehaviour, IDestroyable
{
    private BuildingManager buildingManager;
    private void Awake()
    {
        buildingManager = GameObject.FindGameObjectWithTag("BuildingManager").GetComponent<BuildingManager>();
    }
    public void DestoryObject(GameObject attacker)
    {
        Building building = GetComponent<Building>();

        // publish
        // ����List ����
        BuildingEventBus.Publish(transform);
        // �ش� ���� AI ����
        building.PublishMissionTargetEvent();
        // ���� EventBus ����
        buildingManager.UnsubscribeDestroyEvent(transform);
    }
}
