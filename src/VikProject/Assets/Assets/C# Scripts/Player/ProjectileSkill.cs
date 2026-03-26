using UnityEngine;
using System.Collections.Generic;

public class ProjectileSkill : MonoBehaviour
{
    [SerializeField] private float speed = 15f;

    private SkillDataSO data;

    private float timer;
    private bool isMoving = true;

    private HashSet<IDamageable> hitTargets =
        new HashSet<IDamageable>();

    public void Initialize(SkillDataSO abilityData)
    {
        data = abilityData;
    }

    void Update()
    {
        if (data == null) return;

        timer += Time.deltaTime;

        // Phase 1 : Moving
        if (isMoving)
        {
            transform.position +=
                transform.forward * speed * Time.deltaTime;

            if (timer >= data.moveDuration)
            {
                isMoving = false;
                timer = 0f;
            }
        }
        // Phase 2 : Stay
        else
        {
            if (timer >= data.stayDuration)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (data == null) return;

        if (((1 << other.gameObject.layer) & data.targetLayer) == 0)
            return;

        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null && !hitTargets.Contains(damageable))
        {
            hitTargets.Add(damageable);

            // ✅ Activate enemy
            EnemyNormal enemy = other.GetComponent<EnemyNormal>();
            if (enemy != null && !enemy.aiActive)
            {
                enemy.ActivateAI();
            }

            // ✅ Damage
            damageable.TakeDamage(data.damage);

            // ✅ Hurt reaction
            if (enemy != null)
            {
                enemy.OnHurt();
            }

            // ✅ Knockback (unified)
            if (data.knockback > 0)
            {
                KnockbackReceiver knockback = other.GetComponent<KnockbackReceiver>();

                if (knockback != null)
                {
                    Vector3 dir = other.transform.position - transform.position;
                    dir.y = 0f;
                    dir.Normalize();

                    knockback.Knockback(dir, data.knockback);
                }
            }
        }
    }
}