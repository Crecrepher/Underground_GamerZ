using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrackingKiting.Asset", menuName = "KitingData/TrackingKiting")]
public class TrackingKiting : KitingData
{
    [Tooltip("���� ī���� ����")]
    public float trackingKitingRange;
    public override void UpdateKiting(Transform target, AIController ctrl)
    {
        Vector3 kitingRandomPoint = Utils.RandomPointInCircle(trackingKitingRange, ctrl.target);
        ctrl.SetDestination(kitingRandomPoint);
    }
}
