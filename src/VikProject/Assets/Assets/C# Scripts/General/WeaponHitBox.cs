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
    [SerializeField] AttackDataSO attackData;

    private void OnTriggerEnter(Collider other)
    {
        if (!CanHitTarget(other)) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        IKnockbackable knockbackable = other.GetComponent<IKnockbackable>();
        if (damageable != null)
        {
            DealDamage(damageable);
            
        }
        if(knockbackable != null)
        {
            KnockBack(knockbackable);
        }

    }

    public void DealDamage(IDamageable target)
    {
        float finalDamage =
       weaponState.baseDamage * attackData.damageMultiplier;

        float finalKnockback =
            weaponState.baseKnockback * attackData.knockbackMultiplier;

        target.TakeDamage(finalDamage);
    }
    public void KnockBack(IKnockbackable target)
    {
        float finalKnockback =
            weaponState.baseKnockback * attackData.knockbackMultiplier;
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
}
