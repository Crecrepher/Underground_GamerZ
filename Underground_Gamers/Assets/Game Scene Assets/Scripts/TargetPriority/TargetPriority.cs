using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetPriority.Asset", menuName = "TargetPriority/TargetPriority")]
public class TargetPriority : ScriptableObject
{
    // Ž���� ���, �ݰݿ� ���, �þ� ������ ��� X
    public virtual bool SetTargetByPriority(AIController ai, AIController target)
    {
        float targetDistance = Vector3.Distance(ai.transform.position, target.transform.position);
        if (ai.DistanceToTarget > targetDistance)
        {
            ai.target = target.transform;
            return true;
        }
        return false;
    }
}
