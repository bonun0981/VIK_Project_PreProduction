using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public event Action<float> OnDamaged;

    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [SerializeField] Animator animator;
    [SerializeField] Renderer targetRenderer;

    [Header("Hit Flash")]
    [SerializeField] Color flashColor = Color.white;
    [SerializeField] float flashIntensity = 2f;
    [SerializeField] float flashDuration = 0.1f;

    Material materialInstance;
    Color originalEmission;


    [SerializeField] private CameraShakeManager cameraShakeManager;

    [Header("Invincibility Frames")]
    [SerializeField] float iframeDuration = 0.3f;

    bool isInvincible;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (targetRenderer != null)
        {
            materialInstance = targetRenderer.material; // instance ไม่ให้ไปแก้ shared

            if (materialInstance.HasProperty("_EmissionColor"))
            {
                originalEmission = materialInstance.GetColor("_EmissionColor");
            }
        }
    }
    IEnumerator FlashRoutine()
    {
        if (materialInstance == null || !materialInstance.HasProperty("_EmissionColor"))
            yield break;

        // เปิด emission
        materialInstance.EnableKeyword("_EMISSION");
        materialInstance.SetColor("_EmissionColor", flashColor * flashIntensity);

        yield return new WaitForSeconds(flashDuration);

        // กลับค่าเดิม
        materialInstance.SetColor("_EmissionColor", originalEmission);
    }
    public void Die()
    {
        EnemyNormal enemy = GetComponent<EnemyNormal>();

        if (enemy != null)
        {
            enemy.aiActive = false;
            EnemyCombatDirector.Instance.UnregisterEnemy(enemy);
        }

        // ปิด collider หรือ logic อื่นก่อน (optional)
        GetComponent<Collider>().enabled = false;

        // เล่น animation ตาย
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
    }
    public void DisableAfterDeath()
    {
        gameObject.SetActive(false);
    }
    bool isDead;

    public bool IsDead => isDead;

    public void TakeDamage(float damage)
    {
        if (isInvincible || isDead) return;

        currentHealth -= damage;

        // 🎯 เพิ่มตรงนี้
        StartCoroutine(FlashRoutine());

        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
            return;
        }

        animator?.SetTrigger("Hurt");
        StartCoroutine(IFrameRoutine());
    }
    IEnumerator IFrameRoutine()
    {
        isInvincible = true;

        yield return new WaitForSeconds(iframeDuration);

        isInvincible = false;
    }

    public void CameraShakeOnTakeDamage()
    {
        cameraShakeManager.TakeDamageShake();
    }
}