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

        // cooldown check
        if (cooldownTimer > 0)
            return;

        // ultimate resource check
        if (isUltimateSkill)
        {
            if (!playerResource.IsFull())
                return;

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

        AOESkillPrefab aoe = skillObj.GetComponent<AOESkillPrefab>();
        if (aoe != null)
            aoe.Initialize(abilityData);

        ProjectileSkill projectile = skillObj.GetComponent<ProjectileSkill>();
        if (projectile != null)
            projectile.Initialize(abilityData);

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