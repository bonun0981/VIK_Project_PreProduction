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

        if (damageable != null)
        {
            DealDamage(damageable);

            Instantiate(currentAttackData.hitEffect, other.bounds.center, Quaternion.identity);

            if (!hasTriggeredHitStop)
            {
                HitStopManager.Instance.DoHitStop(currentAttackData.hitstopTime);
                hasTriggeredHitStop = true;
            }

            if (isPlayerWeapon && cameraShakeManager != null)
            {
                cameraShakeManager.TakeDamageShake();
            }

            // ⭐ Knockback จาก AttackDataSO
            if (currentAttackData.statusEffect == AttackStatusEffect.Knockback)
            {
                KnockbackReceiver knockback = other.GetComponent<KnockbackReceiver>();

                if (knockback != null)
                {
                    Vector3 dir =
                        other.transform.position - transform.root.position;

                    knockback.Knockback(dir, currentAttackData.statusPower);
                    Debug.Log($"Applying knockback to {other.name} with direction {dir} and power {currentAttackData.statusPower}");
                }
            }
        }
    }

    public void DealDamage(IDamageable target)
    {
        if (currentAttackData == null) return;

        float finalDamage = weaponState.baseDamage * currentAttackData.damageMultiplier;
        Debug.Log($"Dealing {finalDamage} damage to {target}");

        target.TakeDamage(finalDamage);
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