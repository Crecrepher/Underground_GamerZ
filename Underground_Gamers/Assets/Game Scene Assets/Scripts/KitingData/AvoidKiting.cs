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

        Vector3 targetPos = target.position;
        targetPos.y = target.position.y;
        Collider col = target.GetComponent<Collider>();



        Vector3 enemyLook = ctrl.transform.position - targetPos;
        enemyLook.Normalize();
        float distanceToTarget = Vector3.Distance(ctrl.transform.position, targetPos);
        if (col != null)
        {
            var colDir = ctrl.transform.position - targetPos;
            colDir.Normalize();
            var colDis = colDir * col.bounds.extents.x;
            targetPos += colDis;
        }
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

                // �̵� ������ ��ΰ� �ִ� ���
                kitingPos = hit.position;
                ctrl.SetDestination(kitingPos);
                ctrl.kitingPos = kitingPos;
                GameObject debugPoint = Instantiate(point, kitingPos, Quaternion.identity);
                Destroy(debugPoint, 2f);
                return;
            }

            attempt++;
        }
        kitingPos = ( enemyLook * (ctrl.status.range - distanceToTarget)) + ctrl.transform.position;
        ctrl.SetDestination(kitingPos);
        ctrl.kitingPos = kitingPos;
        GameObject _debugPoint = Instantiate(point, kitingPos, Quaternion.identity);
        Destroy(_debugPoint, 2f);
    }
}
