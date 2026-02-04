using SmoothShakeFree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Animator animator;

    [Header("Stats")]
    [SerializeField] float health = 100f;
    [SerializeField]Rigidbody enemyRigidbody;
    [SerializeField] private float knockbackForce = 2.75f;

    private SmoothShake shake;
    GameObject shakeManager;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] attackClips;

    [SerializeField] private Vector2 pitchRange = new Vector2(0.9f, 1.1f);
    [SerializeField] private Vector2 volumeRange = new Vector2(0.9f, 1f);


    private Renderer[] renderers;
    private List<Material> allMaterials = new List<Material>();
    private List<Color> originalEmissions = new List<Color>();
    Coroutine flashRoutine;
    SmoothShake enemyShake;





    void Awake()
    {
        audioSource = GameObject.FindWithTag("enemysound").GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            Material[] mats = r.materials; // IMPORTANT: plural

            foreach (Material m in mats)
            {
                if (m.HasProperty("_EmissionColor"))
                {
                    allMaterials.Add(m);
                    originalEmissions.Add(m.GetColor("_EmissionColor"));

                    m.SetColor("_EmissionColor", Color.black);
                    m.DisableKeyword("_EMISSION");
                }
            }
        }
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
        int value = Random.Range(1, 3);
        health -= damage;
        animator.SetInteger("randomNumber", value);
        animator.SetTrigger("takeDamage");
        PlayHitSFX();
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
        foreach (Material m in allMaterials)
            m.EnableKeyword("_EMISSION");

        float half = totalTime * 0.5f;

        for (float t = 0; t < half; t += Time.deltaTime)
        {
            float v = t / half;
            SetEmission(Color.white * v * 3f);
            yield return null;
        }

        for (float t = 0; t < half; t += Time.deltaTime)
        {
            float v = 1f - (t / half);
            SetEmission(Color.white * v * 3f);
            yield return null;
        }

        for (int i = 0; i < allMaterials.Count; i++)
        {
            allMaterials[i].SetColor("_EmissionColor", originalEmissions[i]);
            allMaterials[i].DisableKeyword("_EMISSION");
        }
    }

    void SetEmission(Color color)
    {
        foreach (Material m in allMaterials)
        {
            m.SetColor("_EmissionColor", color);
        }
    }

    public void PlayHitSFX()
    {
        if (attackClips.Length == 0) return;

        AudioClip clip = attackClips[Random.Range(0, attackClips.Length)];

        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.volume = Random.Range(volumeRange.x, volumeRange.y);

        audioSource.PlayOneShot(clip);
    }
}