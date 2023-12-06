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

        if(aiController.point != null && aiController.target == null)
            aiController.SetTarget(aiController.point);

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

        if (aiController.target == null)
        {
            aiController.SetState(States.Idle);
            return;
        }

        // ���� ���� �ƴ�, ���� ���� �� �Ѿ��� ���ڸ��ٸ� ����
        if(reloadTime + reloadCoolTime < Time.time && aiController.currentAmmo < aiController.maxAmmo)
        {
            reloadTime = Time.time;
            aiController.Reload();
        }

        // ���� �ʿ�, ����Ʈ ������ �ʿ� / �ؼ���, Ÿ�� ����
        if(Vector3.Distance(aiTr.position, aiController.target.position) < 2f)
        {
            // currentPoint��, ���� ����Ʈ ����. List<Transform>�� �̿��ϰ�, EventBus�� current���� ����
            // ���� ����, ž���� ���Ҷ��� ����
            aiController.SetTarget(aiController.point);
        }

        if (lastDetectTime + aiController.detectTime < Time.time)
        {
            lastDetectTime = Time.time;

            // Ž�� �� Ÿ�� ����
            SearchTargetInDetectionRange();
            SearchTargetInSector();

            agent.SetDestination(aiController.target.position);
        }
    }
}