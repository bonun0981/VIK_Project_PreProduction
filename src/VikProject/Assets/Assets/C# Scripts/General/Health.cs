
using System;
using UnityEngine;

public class Health : MonoBehaviour,IDamageable
{
    public event Action<float> OnDamaged;
    [SerializeField] float maxHealth = 100f;
    [SerializeField]float currentHealth=100f;
    [SerializeField] Animator animator;
    public bool IsDead => currentHealth<=0;
    
    private void Awake()
    {
        animator=GetComponent<Animator>();
        currentHealth = maxHealth;
    }
   
    public void Die()
    {
        gameObject.SetActive(false);
    }
    public void TakeDamage(float damage)
    {
        Debug.Log("Enemy took damage: " + damage);

        OnDamaged?.Invoke(damage);
        if (IsDead) return;
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
        currentHealth -= damage;
        Debug.Log(gameObject.name + "take damage");
        if (IsDead)
        {
            Die();
        }
    }
    
    
}
