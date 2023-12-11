using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionExecutionState : AIState
{
    private float reloadTime;
    private float reloadCoolTime = 3f;

    public MissionExecutionState(AIController aiController) : base(aiController)
    {

    }

    public override void Enter()
    {
        if (!aiStatus.IsLive)
        {
            return;
        }
        aiController.RefreshDebugAIStatus(this.ToString());

        aiController.isBattle = false;

        aiController.SetMissionTarget(aiController.missionTarget);

        lastDetectTime = Time.time - aiController.detectTime;
        reloadTime = Time.time;
        agent.isStopped = false;
        agent.speed = aiStatus.speed;
        agent.angularSpeed = aiStatus.reactionSpeed;

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

        if (aiController.missionTarget == null)
        {
            aiController.SetState(States.Idle);
            return;
        }

        if(aiController.battleTarget != null)
        {
            //aiController.SetBattleTarget(aiController.battleTarget);
            aiController.SetState(States.Trace);
            return;
        }

        // ���� ���� �ƴ�, ���� ���� �� �Ѿ��� ���ڸ��ٸ� ����
        if(reloadTime + reloadCoolTime < Time.time && aiController.currentAmmo < aiController.maxAmmo)
        {
            reloadTime = Time.time;
            aiController.Reload();
        }

        if (lastDetectTime + aiController.detectTime < Time.time)
        {
            lastDetectTime = Time.time;

            // Ž�� �� Ÿ�� ����
            SearchTargetInDetectionRange();
            SearchTargetInSector();
        }
    }
}