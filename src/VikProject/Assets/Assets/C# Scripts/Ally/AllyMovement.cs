using UnityEngine;
using UnityEngine.AI;

public class AllyMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public AllyFormationController formation;

    public float repathInterval = 0.3f;
    float timer;

    Vector3 currentTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        formation.Register(this);
        //agent.avoidancePriority = Random.Range(20, 60);
        agent.acceleration = 40;
        agent.angularSpeed = 720;
        //agent.stoppingDistance = 1.5f;
        //agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > repathInterval)
        {
            timer = 0;
            agent.SetDestination(currentTarget);
        }
    }

    public void FollowPlayer(Transform player)
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    public void MoveTo(Vector3 position)
    {
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    public bool IsInRange(Vector3 target, float range)
    {
        return Vector3.Distance(transform.position, target) <= range;
    }
}