using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health = 100f;
    [SerializeField]Rigidbody enemyRigidbody;
    private float knockbackForce = 2.75f;

    public void TakeDamage(float damage,Vector3 playerDirection)
    {
        health -= damage;
        enemyRigidbody.AddForce(playerDirection * knockbackForce, ForceMode.Impulse);





        if (health <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        Destroy(gameObject);
    }
}
