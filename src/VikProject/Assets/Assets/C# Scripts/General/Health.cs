using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public event Action<float> OnDamaged;

    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [SerializeField] Animator animator;

    public bool IsDead => currentHealth <= 0;

    [SerializeField] private CameraShakeManager cameraShakeManager;

    [Header("Invincibility Frames")]
    [SerializeField] float iframeDuration = 0.3f;

    bool isInvincible;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void Die()
    {
        EnemyNormal enemy = GetComponent<EnemyNormal>();

        if (enemy != null)
        {
            enemy.aiActive = false;
            EnemyCombatDirector.Instance.UnregisterEnemy(enemy);
        }

        gameObject.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        Debug.Log("Enemy took damage: " + damage);

        OnDamaged?.Invoke(damage);

        if (IsDead) return;

        currentHealth -= damage;

        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        Debug.Log(gameObject.name + " take damage");

        if (IsDead)
        {
            Die();
            return;
        }

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