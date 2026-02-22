using UnityEngine;
using System.Collections.Generic;

public class AOESkillPrefab : MonoBehaviour
{
    private SkillDataSO data;
    private HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();

    public void Initialize(SkillDataSO abilityData)
    {
        SphereCollider col = GetComponent<SphereCollider>();
        if (col != null)
        {
            col.radius = data.radius;
        }
        data = abilityData;
        Destroy(gameObject, data.duration);
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

            HitStopManager.Instance.DoHitStop(data.hitStopDuration);
        }
    }
}