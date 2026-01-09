using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Material originalMat;
    [SerializeField] Material damageMat;
    [SerializeField] MeshRenderer reder;
    [SerializeField] float flashTime = 0.75f;

    [SerializeField] float health = 100f;
    [SerializeField]Rigidbody enemyRigidbody;
    private float knockbackForce = 2.75f;

    private void Start()
    {
        
    }
    public void TakeDamage(float damage,Vector3 playerDirection)
    {
        health -= damage;
        enemyRigidbody.AddForce(playerDirection * knockbackForce, ForceMode.Impulse);
        StartCoroutine(TakeDamageFlash(flashTime)); 




        if (health <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        Destroy(gameObject);
    }

    IEnumerator TakeDamageFlash(float time)
    {
       reder.material=damageMat;
        yield return new WaitForSeconds(time);
        reder.material=originalMat;
    }
}
