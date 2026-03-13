using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimationController : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;

    float smoothForward;
    float smoothRight;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        Vector3 velocity = agent.velocity;

        Vector3 local = transform.InverseTransformDirection(velocity);

        float forward = local.z;
        float right = local.x;

        smoothForward = Mathf.Lerp(smoothForward, forward, Time.deltaTime * 8f);
        smoothRight = Mathf.Lerp(smoothRight, right, Time.deltaTime * 8f);

        animator.SetFloat("Forward", smoothForward);
        animator.SetFloat("Right", smoothRight);
    }
    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }
}