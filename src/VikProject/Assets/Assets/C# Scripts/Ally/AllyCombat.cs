using UnityEngine;

public class AllyCombat : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public AllyAnimationController anim;
    float lastAttackTime;
    void Awake()
    {
        anim = GetComponent<AllyAnimationController>();
    }
    public void Attack(Transform target)
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;

        anim.PlayAttack(); // เล่น animation

        Debug.Log("Ally Attack!");

        //var damageable = target.GetComponent<IDamageable>();
        //if (damageable != null)
        //    damageable.TakeDamage(10);
    }
}