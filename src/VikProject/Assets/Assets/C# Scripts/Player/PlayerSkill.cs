using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [SerializeField] private string name;
    [SerializeField] private SkillDataSO abilityData;

    [Header("References")]
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private Transform customSpawnPoint;

    public void Activate()
    {
        if (abilityData == null || abilityData.skillPrefab == null)
            return;

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

        // Initialize damage data
        AOESkillPrefab aoe = skillObj.GetComponent<AOESkillPrefab>();
        if (aoe != null)
        {
            aoe.Initialize(abilityData);
        }

        // Initialize projectile movement
        ProjectileSkill projectile = skillObj.GetComponent<ProjectileSkill>();
        if (projectile != null)
        {
            projectile.Initialize(abilityData);
        }
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
}
