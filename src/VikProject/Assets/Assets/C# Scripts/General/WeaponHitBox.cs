using Unity.VisualScripting;
using UnityEngine;

public class WeaponHitBox : MonoBehaviour
{
    enum TargetType
    {
        EnemySide,
        PlayerSide,
    }

    [SerializeField] TargetType targetType;
    [SerializeField] WeaponStateSO weaponState;
    [SerializeField] private AttackDataSO currentAttackData;
    [SerializeField] Collider hitBoxCollider;

    private bool hasTriggeredHitStop;

    [Header("Camera Shake")]
    [SerializeField] private bool isPlayerWeapon; // เช็คว่าเป็นอาวุธ player หรือไม่
    [SerializeField] private CameraShakeManager cameraShakeManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!CanHitTarget(other)) return;

        IDamageable damageable = other.GetComponent<IDamageable>();

        Debug.Log("hit " + other.name);

        if (damageable != null)
        {
            DealDamage(damageable);

            Instantiate(currentAttackData.hitEffect, other.bounds.center, Quaternion.identity);

            if (!hasTriggeredHitStop)
            {
                HitStopManager.Instance.DoHitStop(currentAttackData.hitstopTime);
                hasTriggeredHitStop = true;
            }

            // Camera Shake เฉพาะ Player Weapon
            if (isPlayerWeapon && cameraShakeManager != null)
            {
                cameraShakeManager.TakeDamageShake();
            }
        }

        switch (currentAttackData.statusEffect)
        {
            case AttackStatusEffect.None:
                break;

            case AttackStatusEffect.Stun:
                IStunnable stunnable = other.GetComponent<IStunnable>();
                if (stunnable != null)
                {
                    Stun(stunnable, currentAttackData.statusPower);
                }
                break;

            case AttackStatusEffect.Knockback:
                IKnockbackable knockbackTarget = other.GetComponent<IKnockbackable>();
                if (knockbackTarget != null)
                {
                    KnockBack(knockbackTarget, currentAttackData.statusPower);
                }
                break;

            case AttackStatusEffect.KnockUp:
                IKnockUpable knockUpTarget = other.GetComponent<IKnockUpable>();
                if (knockUpTarget != null)
                {
                    KnockUp(knockUpTarget, currentAttackData.statusPower);
                }
                break;
        }
    }

    public void DealDamage(IDamageable target)
    {
        if (currentAttackData == null) return;

        float finalDamage = weaponState.baseDamage * currentAttackData.damageMultiplier;
        Debug.Log($"Dealing {finalDamage} damage to {target}");

        target.TakeDamage(finalDamage);
    }

    public void KnockBack(IKnockbackable target, float multiply)
    {
        if (currentAttackData == null) return;

        float finalKnockback = weaponState.baseKnockback * multiply;
        Debug.Log($"Applying {finalKnockback} knockback to {target}");

        target.Knockback(finalKnockback);
    }

    public void Stun(IStunnable target, float duration)
    {
    }

    public void KnockUp(IKnockUpable target, float duration)
    {
    }

    bool CanHitTarget(Collider other)
    {
        switch (targetType)
        {
            case TargetType.EnemySide:
                return other.CompareTag("Enemy");

            case TargetType.PlayerSide:
                return other.CompareTag("Player") || other.CompareTag("Ally");

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
        hasTriggeredHitStop = false;
    }

    public void DisableHitbox()
    {
        hitBoxCollider.enabled = false;
    }
}