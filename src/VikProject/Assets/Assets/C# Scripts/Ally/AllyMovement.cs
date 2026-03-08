using UnityEngine;
using UnityEngine.AI;

public class AllyMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public AllyFormationController formation;

    public float repathInterval = 0.3f;
    float timer;

    Vector3 currentTarget;
    Transform currentEnemy;
    bool sentActivate;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        formation.Register(this);
       
        agent.acceleration = 40;
        agent.angularSpeed = 720;
        //agent.autoBraking = false;
        agent.avoidancePriority = Random.Range(30, 70);
       
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
    }

    void Update()
    {
        TryActivateEnemy();
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
    public void MoveToEnemy(Transform enemy)
    {
        currentEnemy = enemy;
        currentTarget = enemy.position;

        agent.isStopped = false;
        agent.SetDestination(enemy.position);
    }
    public bool IsInRange(Vector3 target, float range)
    {
        return Vector3.Distance(transform.position, target) <= range;
    }
    void TryActivateEnemy()
    {
        if (currentEnemy == null) return;
        if (sentActivate) return;

        float dist =
            (transform.position - currentEnemy.position).sqrMagnitude;

        if (dist < 36f) // 6 meters
        {
            EnemyNormal enemy = currentEnemy.GetComponent<EnemyNormal>();

            if (enemy != null)
            {
                enemy.ActivateAI();
                sentActivate = true;
            }
        }
    }
}