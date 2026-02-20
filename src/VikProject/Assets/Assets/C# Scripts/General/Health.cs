using UnityEngine;

public class Health : MonoBehaviour,IDamageable
{
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
       if(IsDead) return;
        currentHealth -= damage;
        if (IsDead)
        {
            Die();
        }
    }
    
}
