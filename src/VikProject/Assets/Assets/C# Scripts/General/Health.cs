
using System;
using UnityEngine;

public class Health : MonoBehaviour,IDamageable
{
    public event Action<float> OnDamaged;
    [SerializeField] float maxHealth = 100f;
    [SerializeField]float currentHealth;

    public bool IsDead => currentHealth<=0;

    private void Awake()
    {
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
        currentHealth -= damage;
        if (IsDead)
        {
            Die();
        }
    }
    
    
}
