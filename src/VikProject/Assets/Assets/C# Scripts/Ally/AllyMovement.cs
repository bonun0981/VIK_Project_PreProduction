using UnityEngine;
using UnityEngine.AI;

public class AllyMovement : MonoBehaviour
{
    public float combatAvoidanceDistance = 3f;
    bool movementLocked;
    public NavMeshAgent agent;
    public AllyFormationController formation;
    public float followDistance = 2.5f;
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
    void UpdateAvoidance()
    {
        if (agent == null) return;

        if (currentEnemy == null)
        {
            agent.obstacleAvoidanceType =
                ObstacleAvoidanceType.LowQualityObstacleAvoidance;
            return;
        }

        float dist =
            (transform.position - currentEnemy.position).sqrMagnitude;

        if (dist < combatAvoidanceDistance * combatAvoidanceDistance)
        {
            // เข้า melee → ปิด avoidance
            agent.obstacleAvoidanceType =
                ObstacleAvoidanceType.NoObstacleAvoidance;
        }
        else
        {
            // เดินทาง → ใช้ avoidance
            agent.obstacleAvoidanceType =
                ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        }
    }
    void Update()
    {
        if (movementLocked)
            return;

        TryActivateEnemy();

        UpdateAvoidance(); // ⭐ เพิ่มตรงนี้

        timer += Time.deltaTime;

        if (timer > repathInterval)
        {
            timer = 0;
            agent.SetDestination(currentTarget);
        }
    }
    public void FollowPlayer(Transform player)
    {
        if (player == null) return;

        agent.isStopped = false;

        Vector3 dir = transform.position - player.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.01f)
            dir = Random.insideUnitSphere;

        dir = dir.normalized;

        Vector3 target = player.position + dir * followDistance;

        currentTarget = target;
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

    public void OnHurt()
    {
        movementLocked = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }
    public void RecoverFromHurt()
    {
        movementLocked = false;

        if (agent != null)
        {
            agent.isStopped = false;
        }
    }

}