using System.Collections;
using UnityEngine;
using SmoothShakeFree;

public class Enemy : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField] Material originalMat;
    [SerializeField] Material damageMat;
    [SerializeField] MeshRenderer render;
    [SerializeField] float flashTime = 0.1f;
    [Range(0f, 1f)] public float flashAmount;

    
    [Header("Particle Combo Settings")]
    [SerializeField] private ParticleSystem[] hitParticles = new ParticleSystem[3]; 
    [SerializeField] private float comboResetTime = 1.5f; 
    private int currentHitIndex = 0; 
    private float lastHitTime; 
    

    [Header("Stats")]
    [SerializeField] float health = 100f;
    Rigidbody enemyRigidbody;
    [SerializeField] private float knockbackForce = 2.75f;

    private SmoothShake shake;
    GameObject shakeManager;

    Renderer rend;
    Material mat;
    Color originalEmission;

    Coroutine flashRoutine;
    SmoothShake enemyShake;

    void Awake()
    {
        
        rend = GetComponentInChildren<Renderer>();
        mat = rend.material;

        mat.SetColor("_EmissionColor", Color.black);
        mat.DisableKeyword("_EMISSION");

        originalEmission = Color.black;
    }

    private void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody>();
        enemyShake = GetComponent<SmoothShake>();
        shakeManager = GameObject.FindWithTag("ShakeController");

        if (shakeManager != null)
            shake = shakeManager.GetComponent<SmoothShake>();
    }

    public void TakeDamage(float damage, Vector3 playerDirection)
    {
        health -= damage;

        
        HandleHitParticle(playerDirection);
        

        Vector3 knockDir = playerDirection;
        knockDir.y = 0f;
        knockDir.Normalize();

        enemyRigidbody.linearVelocity = Vector3.zero;
        enemyRigidbody.AddForce(knockDir * knockbackForce, ForceMode.VelocityChange);

        PlayHitFlash(flashTime);

        if (shake != null) shake.StartShake();
        if (enemyShake != null) enemyShake.StartShake();

        if (health <= 0)
        {
            Die();
        }
    }

    private void HandleHitParticle(Vector3 playerDirection)
    {
        
        if (Time.time - lastHitTime > comboResetTime)
        {
            currentHitIndex = 0;
        }

        
        if (hitParticles.Length > 0 && hitParticles[currentHitIndex] != null)
        {
            
            Instantiate(hitParticles[currentHitIndex], transform.position + Vector3.up, Quaternion.LookRotation(-playerDirection));

            
            currentHitIndex = (currentHitIndex + 1) % hitParticles.Length;
        }

       
        lastHitTime = Time.time;
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    void PlayHitFlash(float duration)
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(HitFlashRoutine(duration));
    }

    IEnumerator HitFlashRoutine(float totalTime)
    {
        mat.EnableKeyword("_EMISSION");
        float half = totalTime * 0.5f;
        for (float t = 0; t < half; t += Time.deltaTime)
        {
            float v = t / half;
            mat.SetColor("_EmissionColor", Color.white * v * 3f);
            yield return null;
        }
        for (float t = 0; t < half; t += Time.deltaTime)
        {
            float v = 1f - (t / half);
            mat.SetColor("_EmissionColor", Color.white * v * 3f);
            yield return null;
        }
        mat.SetColor("_EmissionColor", originalEmission);
        mat.DisableKeyword("_EMISSION");
    }
}