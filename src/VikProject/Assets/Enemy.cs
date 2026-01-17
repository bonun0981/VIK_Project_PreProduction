using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Material originalMat;
    [SerializeField] Material damageMat;
    [SerializeField] MeshRenderer render;
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
        StartCoroutine(freeztime(0.5f, 0.06f));



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
        
        render.material=damageMat;
        yield return new WaitForSeconds(time);
        render.material=originalMat;
       
    }

    IEnumerator freeztime(float slowScale,float duration)
    {
        Time.timeScale = slowScale;
        Time.fixedDeltaTime = 0.02f * slowScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}
