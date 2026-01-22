using System.Collections;
using UnityEngine;
using SmoothShakeFree;

public class Enemy : MonoBehaviour
{
    [SerializeField] Material originalMat;
    [SerializeField] Material damageMat;
    [SerializeField] MeshRenderer render;
    [SerializeField] float flashTime = 0.1f;
    [Range(0f, 1f)] public float flashAmount;
    [SerializeField] float health = 100f;
    Rigidbody enemyRigidbody;
    [SerializeField]private float knockbackForce = 2.75f;
    private SmoothShake shake;
    GameObject shakeManager;

    Renderer rend;
    Material mat;
    Color originalEmission;

    Coroutine flashRoutine;
    Coroutine hitStopRoutine;
    SmoothShake enemyShake;
    void Awake()
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        mat = rend.material;

        // FORCE no glow at start
        mat.SetColor("_EmissionColor", Color.black);
        mat.DisableKeyword("_EMISSION");

        originalEmission = Color.black;
    }
    private void Start()
    {
        enemyRigidbody =GetComponent<Rigidbody>();
        enemyShake = GetComponent<SmoothShake>();
        shakeManager = GameObject.FindWithTag("ShakeController");
        shake= shakeManager.GetComponent<SmoothShake>();
    }
    public void TakeDamage(float damage, Vector3 playerDirection)
    {
        health -= damage;

        Vector3 knockDir = playerDirection;
        knockDir.y = 0f;
        knockDir.Normalize();

        enemyRigidbody.linearVelocity = Vector3.zero;
        enemyRigidbody.AddForce(knockDir * knockbackForce, ForceMode.VelocityChange);

        PlayHitFlash(flashTime);
        //StartCoroutine(freeztime(0.1f, 0.04f));
        shake.StartShake();
        enemyShake.StartShake();

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
    void PlayHitFlash(float duration)
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

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


    IEnumerator freeztime(float slowScale,float duration)
    {
        Time.timeScale = slowScale;
        Time.fixedDeltaTime = 0.02f * slowScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}
