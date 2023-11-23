using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum States
{
    Idle,
    Trace,
    Attack,
}
public class AIController : MonoBehaviour
{
    private NavMeshAgent agent;
    public CharacterStatus status;
    private StateManager stateManager;
    private List<BaseState> states = new List<BaseState>();

    private LookCamera lookCamera;
    public Transform point;
    public LayerMask layer;

    [Tooltip("Ž�� ������ �ð�")]
    public float detectTime = 0.1f;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // ������ ���̺� ���� ������ ������ �� ����, ���� ������ ���̺� ������
        status = GetComponent<CharacterStatus>();
        lookCamera = GetComponentInChildren<LookCamera>();
    }

    private void Start()
    {
        stateManager = new StateManager();
        states.Add(new IdleState(this));
        states.Add(new TraceState(this));

        agent.speed = status.speed;
        agent.SetDestination(point.position);

        SetState(States.Idle);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            agent.SetDestination(point.position);

        }
        lookCamera.LookAtCamera();
    }
    public void SetState(States newState)
    {
        stateManager.ChangeState(states[(int)newState]);
    }    


    public void UpdateState()
    {
        stateManager.Update();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        float viewAngle = 60f;
        float viewRadius = 2f;
        float dectionRange = 1f;
        if (status != null)
        {
            viewRadius = status.sight;
            viewAngle = status.sightAngle;
            dectionRange = status.detectionRange;
        }
        Gizmos.DrawWireSphere(transform.position, dectionRange);

        Vector3 viewAngleA = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 viewAngleB = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;
        Gizmos.color = Color.blue;
        Vector3 newV = transform.position;
        newV.y += 0.5f;
        Gizmos.DrawLine(newV, newV + viewAngleA * viewRadius);
        Gizmos.DrawLine(newV, newV + viewAngleB * viewRadius);

    }
}
