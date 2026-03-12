using UnityEngine;
using UnityEngine.UI;

public class Player_HUD : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private GameObject player;

    [Header("Health")]
    public Slider hpSlider;
    public Slider easeHpSlider;
    private Health playerHealth;

    [SerializeField] private float lerpSpeed = 2f;
    [SerializeField] private float snapThreshold = 0.005f;

    [Header("Resource")]
    [SerializeField] private Slider resourceSlider;
    private PlayerResource playerResource;

    [Header("Skill UI")]
    [SerializeField] private Image skillCooldownOverlay;
   

    [SerializeField]private PlayerSkill normalSkill;
    [SerializeField]private PlayerSkill ultimateSkill;

    private void Start()
    {
        if (player == null) return;

        playerHealth = player.GetComponent<Health>();
        playerResource = player.GetComponent<PlayerResource>();

        PlayerSkill[] skills = player.GetComponents<PlayerSkill>();

        foreach (var skill in skills)
        {
            if (skill.IsUltimate())
                ultimateSkill = skill;
            else
                normalSkill = skill;
        }
    }

    private void Update()
    {
        UpdateHealth();
        UpdateResource();
        UpdateSkillCooldowns();
    }

    void UpdateHealth()
    {
        if (playerHealth == null) return;

        float targetHp = playerHealth.currentHealth / playerHealth.maxHealth;

        hpSlider.value = targetHp;

        if (easeHpSlider.value > targetHp)
        {
            easeHpSlider.value = Mathf.Lerp(
                easeHpSlider.value,
                targetHp,
                lerpSpeed * Time.deltaTime
            );

            if (Mathf.Abs(easeHpSlider.value - targetHp) < snapThreshold)
                easeHpSlider.value = targetHp;
        }
        else
        {
            easeHpSlider.value = targetHp;
        }
    }

    void UpdateResource()
    {
        if (playerResource == null || resourceSlider == null) return;

        resourceSlider.value = playerResource.NormalizedValue();
    }

    void UpdateSkillCooldowns()
    {
        if (normalSkill != null && skillCooldownOverlay != null)
        {
            float r = normalSkill.CooldownRemaining();
            float m = normalSkill.CooldownMax();

            skillCooldownOverlay.fillAmount = r > 0 ? r / m : 0;
        }

        
    }
}