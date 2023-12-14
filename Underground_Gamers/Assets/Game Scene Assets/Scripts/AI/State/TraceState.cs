using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceState : AIState
{
    public TraceState(AIController aiController) : base(aiController)
    {

    }

    private float traceTimer;
    private float traceTime;

    public override void Enter()
    {
        aiController.RefreshDebugAIStatus(this.ToString());
        if(aiController.battleTarget != null)
        {
            if (!aiController.battleTarget.GetComponent<TeamIdentifier>().isBuilding)
                aiController.isBattle = true;
            else
                aiController.isBattle = false;
        }

        lastDetectTime = Time.time - aiController.detectTime;
        traceTime = Time.time - traceTimer;

        agent.isStopped = false;
        agent.angularSpeed = aiStatus.reactionSpeed;
        agent.speed = aiController.kitingInfo.kitingSpeed;
    }

    public override void Exit()
    {

    }

    public override void Update()
    {
        if (!aiStatus.IsLive)
        {
            return;
        }

        RotateToTarget();

        if (aiController.battleTarget == null)
        {
            aiController.SetState(States.MissionExecution);
            return;
        }

        if(aiController.DistanceToBattleTarget < aiStatus.range)
        {
            aiController.SetState(States.AimSearch);
            return;
        }

        // ���� ����
        if(aiController.DistanceToBattleTarget > aiStatus.range && traceTime + traceTimer < Time.time)
        {
            traceTimer = Time.time;
            aiController.SetDestination(aiController.battleTarget);
        }

        if (lastDetectTime + aiController.detectTime < Time.time && aiController.battleTarget != null)
        {
            lastDetectTime = Time.time;

            // Ž�� �� Ÿ�� ����
            SearchTargetInDetectionRange();
            SearchTargetInSector();

            // �����ؾ��ұ�?
            //aiController.SetDestination(aiController.battleTarget.position);
        }
    }
}
