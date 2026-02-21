using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [SerializeField] private SkillDataSO abilityData;
    [SerializeField] private Transform spawnPoint;

    public void Activate()
    {
        if (abilityData == null || abilityData.skillPrefab == null)
            return;

        GameObject skillObj = Instantiate(
            abilityData.skillPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        AOESkillPrefab skill = skillObj.GetComponent<AOESkillPrefab>();
        if (skill != null)
        {
            skill.Initialize(abilityData);
        }
    }
}
