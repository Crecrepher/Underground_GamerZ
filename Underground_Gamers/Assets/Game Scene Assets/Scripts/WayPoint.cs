using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Line
{
    Top,
    Bottom,
    Count
}

public class WayPoint : MonoBehaviour
{
    public GameObject bottomWayPoint;
    public GameObject topWayPoint;

    [Tooltip("0�� �ε��� ����")]
    public Transform[] bottomWayPoints;
    [Tooltip("0�� �ε��� ����")]
    public Transform[] topWayPoints;

    private void Awake()
    {
        bottomWayPoint = transform.GetChild(1).gameObject;
        topWayPoint = transform.GetChild(0).gameObject;

        bottomWayPoints = bottomWayPoint.GetComponentsInChildren<Transform>();
        topWayPoints = topWayPoint.GetComponentsInChildren<Transform>();
    }
}
