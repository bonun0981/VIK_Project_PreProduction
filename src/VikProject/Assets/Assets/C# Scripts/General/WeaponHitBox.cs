using UnityEngine;

public class WeaponHitBox : MonoBehaviour
{
     enum TargetType
    {
        Enemy,
        Player,
        Both
    }
    [SerializeField]TargetType targetType;
    [SerializeField] WeaponStateSO weaponState;
    private AttackDataSO currentAttackData;
    [SerializeField]Collider hitBoxCollider;
    private bool hasTriggeredHitStop;

    private void OnTriggerEnter(Collider other)
    {
        if (!CanHitTarget(other)) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        IKnockbackable knockbackable = other.GetComponent<IKnockbackable>();

        if (damageable != null)
        {
            DealDamage(damageable);

            if (!hasTriggeredHitStop)
            {
                HitStopManager.Instance.DoHitStop(0.08f);
                hasTriggeredHitStop = true;
            }
        }

        if (knockbackable != null)
        {
            KnockBack(knockbackable);
        }

    }

    public void DealDamage(IDamageable target)
    {
        if (currentAttackData == null) return;

        float finalDamage =
            weaponState.baseDamage * currentAttackData.damageMultiplier;
        Debug.Log($"Dealing {finalDamage} damage to {target}");
        target.TakeDamage(finalDamage);
    }
    public void KnockBack(IKnockbackable target)
    {
        if (currentAttackData == null) return;

        float finalKnockback =
            weaponState.baseKnockback * currentAttackData.knockbackMultiplier;
        Debug.Log($"Applying {finalKnockback} knockback to {target}");
        target.Knockback(finalKnockback);
    }
    bool CanHitTarget(Collider other)
    {
        switch (targetType)
        {
            case TargetType.Enemy:
                return other.CompareTag("Enemy");

            case TargetType.Player:
                return other.CompareTag("Player");

            case TargetType.Both:
                return other.CompareTag("Enemy") || other.CompareTag("Player");

            default:
                return false;
        }
    }
    public void SetAttackData(AttackDataSO attackData)
    {
        currentAttackData = attackData;
    }
    public void EnableHitbox()
    {
        hitBoxCollider.enabled = true;
        hasTriggeredHitStop = false; // รีเซ็ตทุกครั้งที่เริ่มโจมตี
    }
    public void DisableHitbox()
    {
        hitBoxCollider.enabled = false;
    }
}
