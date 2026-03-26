using UnityEngine;
using UnityEngine.AI;

public class AllyAnimationController : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        float speed = agent.velocity.magnitude;

        // ส่งค่า speed เข้า Animator
        animator.SetFloat("Speed", speed);
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }
}