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
        // Ÿ�ٰ��� �߰� �������� ������ ī���� ��ġ ����
        var coltarget = target.GetComponent<Collider>();
        var origin = ctrl.transform.position;
        Vector3 kitingRandomPoint = Vector3.zero;


        // Ÿ�ٰ��� Y��ǥ �����Ͽ�, XZ���󿡼��� ���
        Vector3 targetPos = target.position;
        targetPos.y = ctrl.transform.position.y;

        float colDis = 0;
        if (coltarget != null)
        {
            if (coltarget.bounds.extents.x >= coltarget.bounds.extents.z)
                colDis = coltarget.bounds.extents.x;
            else
                colDis = coltarget.bounds.extents.z;
        }


        // Ÿ�ٰ��� �߰����� ���
        float dis = Vector3.Distance(targetPos, origin) - colDis;
        Vector3 dirToTarget = targetPos - origin;
        dirToTarget.Normalize();
        Vector3 midPoint = (dirToTarget * (dis * 0.5f)) + origin;

        // �ݶ��̴� �ݰ�, ���� �ؾߵɱ�?
        //if (coltarget != null)
        //{
        //    var targetpos = target.position;
        //    targetpos.y = origin.y;
        //    var coldir = origin - target.position;
        //    coldir.Normalize();
        //    var coldis = coldir * coltarget.bounds.extents.x;
        //    midPoint += coldis;
        //}

        Vector3 pointInNavMesh = Vector3.zero;

        if (Utils.RandomPointInNav(midPoint, trackingKitingRange, attemp, out pointInNavMesh))
        {
            ctrl.SetDestination(pointInNavMesh); // ���� �ʿ�
            ctrl.kitingPos = pointInNavMesh;
            //GameObject debugPoint = Instantiate(point, pointInNavMesh, Quaternion.identity);
            //Destroy(debugPoint, 2f);
        }
        else
        {
            ctrl.SetDestination(pointInNavMesh); // ���� �ʿ�
            ctrl.kitingPos = pointInNavMesh;
            //GameObject debugPoint = Instantiate(point, pointInNavMesh, Quaternion.identity);
            //Destroy(debugPoint, 2f);
        }
    }
}
