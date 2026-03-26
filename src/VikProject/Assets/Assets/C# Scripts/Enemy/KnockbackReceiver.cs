using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class KnockbackReceiver : MonoBehaviour
{
    [SerializeField] float knockbackDuration = 0.2f;

    Rigidbody rb;
    NavMeshAgent agent;
    EnemyNormal enemy;

    bool isKnockedBack;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<EnemyNormal>();   // ⭐ reference enemy
    }

    public void Knockback(Vector3 direction, float force)
    {
        if (isKnockedBack) return;

        StartCoroutine(KnockbackRoutine(direction, force));
    }

    IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        isKnockedBack = true;

        // ⭐ แจ้ง enemy ว่าเริ่ม knockback
        if (enemy != null)
            enemy.StartKnockback();

        direction.y = 0f;
        direction.Normalize();

        Vector3 finalForce = direction * force;

        if (agent != null)
            agent.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(finalForce, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(knockbackDuration);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (agent != null)
            agent.enabled = true;

        // ⭐ แจ้ง enemy ว่า knockback จบ
        if (enemy != null)
            enemy.EndKnockback();

        isKnockedBack = false;
    }
}