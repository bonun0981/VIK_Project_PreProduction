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
        IKnockbackable knockbackable = other.GetComponent<IKnockbackable>();

        if (damageable != null && !hitTargets.Contains(damageable))
        {
            damageable.TakeDamage(data.damage);
            hitTargets.Add(damageable);

            if (knockbackable != null)
                knockbackable.Knockback(data.knockback);

            //HitStopManager.Instance.DoHitStop(data.hitStopDuration);
        }
    }
}