using UnityEngine;

public class AllyCombat : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    float lastAttackTime;

    public void Attack(Transform target)
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;

        Debug.Log("Ally Attack!");

        // เรียก Damage ที่ enemy
        var damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.TakeDamage(10);
    }
}