using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PatrolState : AIState
{
    private float patrolRadius = 4f;
    private float patrolTimer;
    private float patrolTime;

    private float detectTimer;
    private float detectTime;

    public PatrolState(AIController aiController) : base(aiController)
    {
    }

    public override void Enter()
    {
        aiController.RefreshDebugAIStatus(this.ToString());

        agent.isStopped = false;
        agent.speed = aiController.status.speed;
        aiController.battleTarget = null;
        patrolTime = 2f;
        patrolTimer = Time.time - patrolTime;

        detectTime = 1f;
        detectTimer = Time.time - detectTimer;
    }

    public override void Exit()
    {

    }

    public override void Update()
    {
        if (aiController.isAttack)
        {
            aiController.SetMissionTarget(aiController.buildingManager.GetAttackPoint(aiController.currentLine, aiController.teamIdentity.teamType));
            aiController.SetState(States.MissionExecution);
            return;
        }

        if(patrolTime + patrolTimer < Time.time)
        {
            patrolTimer = Time.time;
            RandomPatrol();
        }

        if(detectTime + detectTimer < Time.time)
        {
            detectTimer = Time.time;
            SearchTargetInPatrol();
        }
    }

    private void RandomPatrol()
    {
        // Ÿ�ٰ��� �߰� �������� ������ ī���� ��ġ ����
        var coltarget = aiController.missionTarget.GetComponent<Collider>();
        var origin = aiController.transform.position;
        Vector3 kitingRandomPoint = Vector3.zero;


        // Ÿ�ٰ��� Y��ǥ �����Ͽ�, XZ���󿡼��� ���
        Vector3 targetPos = aiController.missionTarget.position;
        targetPos.y = aiController.transform.position.y;

        // Ÿ�ٰ��� �߰����� ���
        float dis = Vector3.Distance(targetPos, origin);
        Vector3 dirToTarget = targetPos - origin;
        dirToTarget.Normalize();
        Vector3 midPoint = (dirToTarget * (dis * 0.5f)) + origin;

        // �ݶ��̴� �ݰ�, ���� �ؾߵɱ�?
        if (coltarget != null)
        {
            var targetpos = aiController.missionTarget.position;
            targetpos.y = origin.y;
            var coldir = origin - aiController.missionTarget.position;
            coldir.Normalize();
            var coldis = coldir * coltarget.bounds.extents.x;
            midPoint += coldis;
        }

        Vector3 pointInNavMesh = Vector3.zero;

        if (Utils.RandomPointInNav(midPoint, patrolRadius, 30, out pointInNavMesh))
        {
            agent.SetDestination(pointInNavMesh);
        }
    }
}
