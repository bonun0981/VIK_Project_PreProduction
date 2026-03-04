using UnityEngine;
using UnityEngine.AI;

public class AllyMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public float followRadius = 2.5f;

    public float repathInterval = 0.25f;
    float repathTimer;

    Vector3 slotOffset;      // ล็อกตำแหน่งรอบ player
    Vector3 currentTarget;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // 🔒 สุ่มครั้งเดียว
        float angle = Random.Range(0f, 360f);
        slotOffset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            0,
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * followRadius;

        // ค่าแนะนำสำหรับ Musou
        agent.acceleration = 40f;
        agent.angularSpeed = 720f;
        agent.stoppingDistance = 1.5f;
        agent.autoBraking = true;
    }

    void Update()
    {
        repathTimer += Time.deltaTime;

        if (repathTimer >= repathInterval)
        {
            repathTimer = 0f;
            agent.SetDestination(currentTarget);
        }
    }

    public void FollowAroundPlayer(Transform player)
    {
        Vector3 targetPos = player.position + slotOffset;
        currentTarget = targetPos;

        agent.isStopped = false;
    }

    public void MoveTo(Vector3 position)
    {
        currentTarget = position;
        agent.isStopped = false;
    }

    public bool IsInRange(Vector3 target, float range)
    {
        return Vector3.Distance(transform.position, target) <= range;
    }
}