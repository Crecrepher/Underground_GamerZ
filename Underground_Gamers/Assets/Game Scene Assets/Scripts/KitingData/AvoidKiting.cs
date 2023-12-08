using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "AvoidKiting.Asset", menuName = "KitingData/AvoidKiting")]
public class AvoidKiting : KitingData
{
    public GameObject point;
    public override void UpdateKiting(Transform target, AIController ctrl)
    {
        int attempt = 0;

        // ���� ���� ���� �þ�
        Vector3 enemyLook = ctrl.transform.position - target.position;
        enemyLook.Normalize();

        float distanceToTarget = Vector3.Distance(ctrl.transform.position, target.transform.position);

        Vector3 kitingPos = Vector3.zero;

        while (attempt < 30)
        {
            float randomAngle = Random.Range(-30f, 30f);
            Quaternion randomRotation = Quaternion.Euler(0f, randomAngle, 0f);
            Vector3 randomDirection = randomRotation * enemyLook;
            kitingPos = (randomDirection * (ctrl.status.range - distanceToTarget)) + ctrl.transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(kitingPos, out hit, 1.0f, NavMesh.AllAreas)) // ������ ��ġ�� �׺�޽ÿ� �ִ��� Ȯ��
            {
                // SamplePosition���� �׺���̼� �Ž� �� ��ġ�� Ȯ�ε�
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(ctrl.transform.position, kitingPos, NavMesh.AllAreas, path))
                {
                    // CalculatePath�� ��θ� ����Ͽ� �̵� ���� ���θ� Ȯ��
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        // �̵� ������ ��ΰ� �ִ� ���
                        kitingPos = hit.position;
                        ctrl.SetDestination(kitingPos);
                        ctrl.kitingPos = kitingPos;
                        GameObject debugPoint = Instantiate(point, kitingPos, Quaternion.identity);
                        Destroy(debugPoint, 2f);
                        return;
                    }
                }
            }

            attempt++;
        }

        ctrl.SetState(States.Kiting);
    }
}
