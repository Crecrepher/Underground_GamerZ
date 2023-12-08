using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "TrackingKiting.Asset", menuName = "KitingData/TrackingKiting")]
public class TrackingKiting : KitingData
{
    public GameObject point;

    [Tooltip("���� ī���� ����")]
    public float trackingKitingRange;
    public override void UpdateKiting(Transform target, AIController ctrl)
    {
        int attempt = 0;
        Vector3 kitingRandomPoint;
        kitingRandomPoint = Utils.RandomPointInCircle(trackingKitingRange, ctrl.battleTarget);
        kitingRandomPoint.y = ctrl.transform.position.y;
        var colTarget = target.GetComponent<Collider>();
        var center = ctrl.transform.position;

        if (colTarget != null)
        {
            var targetPos = target.position;
            targetPos.y = center.y;
            var colDir = center - target.position;
            colDir.Normalize();
            var colDis = colDir * colTarget.bounds.extents.x;
            kitingRandomPoint += colDis;
        }
        kitingRandomPoint += center;
        while (attempt < 30)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(kitingRandomPoint, out hit, ctrl.agent.radius, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(center, hit.position, NavMesh.AllAreas, path))
                {
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        kitingRandomPoint.y = center.y;
                        ctrl.SetDestination(kitingRandomPoint); // ���� �ʿ�
                        ctrl.kitingPos = kitingRandomPoint;
                        GameObject debugPoint = Instantiate(point, kitingRandomPoint, Quaternion.identity);
                        Destroy(debugPoint, 2f);
                        return;
                        //return hit.position;
                    }
                }
            }

            attempt++;
        }

        // ���� �� �⺻�� ��ȯ
        Debug.Log("Ž�� ����");
        GameObject debugFailedPoint = Instantiate(point, kitingRandomPoint, Quaternion.identity);



        //int attempt = 0;
        //Vector3 kitingRandomPoint = Vector3.zero;
        //var colTarget = target.GetComponent<Collider>();

        //while (attempt < 30)
        //{
        //    if (colTarget != null)
        //    {
        //        var colDir = ctrl.transform.position - target.position;
        //        colDir.Normalize();
        //        var colDis = colDir * colTarget.bounds.extents.magnitude;
        //        kitingRandomPoint += colDis;
        //    }

        //    kitingRandomPoint = Utils.RandomPointInCircle(trackingKitingRange, ctrl.battleTarget);
        //    kitingRandomPoint.y = ctrl.transform.position.y;

        //    NavMeshHit hit;
        //    if (NavMesh.SamplePosition(kitingRandomPoint, out hit, 1.0f, NavMesh.AllAreas))
        //    {
        //        kitingRandomPoint = hit.position;
        //        ctrl.SetDestination(kitingRandomPoint); // ���� �ʿ�
        //        ctrl.kitingPos = kitingRandomPoint;
        //        GameObject debugPoint = Instantiate(point, kitingRandomPoint, Quaternion.identity);
        //        Destroy(debugPoint, 2f);
        //        return;
        //    }

        //    attempt++;
        //}

        //ctrl.SetState(States.Kiting);
    }
}
