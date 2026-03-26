using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [SerializeField] private string skillName;
    [SerializeField] private SkillDataSO abilityData;

    [Header("Skill Type")]
    [SerializeField] private bool isUltimateSkill;

    [Header("References")]
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private Transform customSpawnPoint;
    [SerializeField] private PlayerResource playerResource;

    [SerializeField]private float cooldownTimer;
   
    private void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    public void Activate()
    {
        if (abilityData == null || abilityData.skillPrefab == null)
            return;

        // ❌ ไม่เช็ค cooldown แล้ว
        // ❌ ไม่เช็ค resource แล้ว

        // 🔥 ถ้าเป็น ultimate → consume อย่างเดียว
        if (isUltimateSkill)
        {
            playerResource.ConsumeAll();
        }

        Transform spawnTransform = GetSpawnTransform();

        GameObject skillObj = Instantiate(
            abilityData.skillPrefab,
            spawnTransform.position,
            spawnTransform.rotation
        );

        if (abilityData.spawnType == SkillSpawnType.AtWeapon)
        {
            skillObj.transform.SetParent(weaponTransform);
        }

        if (skillObj.TryGetComponent(out AOESkillPrefab aoe))
            aoe.Initialize(abilityData);

        if (skillObj.TryGetComponent(out ProjectileSkill projectile))
            projectile.Initialize(abilityData);

        // 🔥 cooldown reset ยังอยู่
        cooldownTimer = abilityData.cooldown;
    }

    private Transform GetSpawnTransform()
    {
        switch (abilityData.spawnType)
        {
            case SkillSpawnType.AtWeapon:
                return weaponTransform;

            case SkillSpawnType.CustomPoint:
                return customSpawnPoint;

            default:
                return transform;
        }
    }
    public bool CanActivate()
    {
        if (abilityData == null)
            return false;

        // 🔥 cooldown ต้องหมดก่อนเสมอ
        if (cooldownTimer > 0)
            return false;

        // 🔥 ถ้าเป็น Ultimate → ต้อง resource เต็ม
        if (isUltimateSkill)
        {
            if (!playerResource.IsFull())
                return false;
        }

        return true;
    }
    public float CooldownRemaining()
    {
        return cooldownTimer;
    }

    public float CooldownMax()
    {
        return abilityData.cooldown;
    }

    public bool IsUltimate()
    {
        return isUltimateSkill;
    }
}