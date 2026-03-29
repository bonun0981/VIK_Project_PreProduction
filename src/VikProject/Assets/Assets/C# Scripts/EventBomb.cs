using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class EventBomb : MonoBehaviour
{
    // --- ระบบเสียงรายไฟล์ ---
    [System.Serializable]
    public class SoundSettings
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float individualVolume = 1f;
    }

    // --- ระบบเลือกการสั่นหลายแบบ ---
    [System.Serializable]
    public class ShakeSettings
    {
        public string name;
        public float duration = 0.5f;
        public float magnitude = 0.2f;
    }

    [Header("Camera Shake Settings")]
    public List<ShakeSettings> shakeStyles = new List<ShakeSettings>();
    public int shakeIndexToUse = 0;

    [Header("Particle Settings")]
    public List<GameObject> particlesOnce = new List<GameObject>();
    public float destroyDelay = 5f;
    public List<GameObject> particlesLoop = new List<GameObject>();

    [Header("Audio Settings (Audio Clips)")]
    [Tooltip("ใส่ไฟล์เสียงที่ต้องการให้เล่นตอนเกิด Event (จะสุ่มเล่น 1 เสียง)")]
    public List<SoundSettings> eventSounds = new List<SoundSettings>();

    [Header("Objects to Manage")]
    public GameObject objectToRemove;

    [Header("Trigger Settings")]
    public string targetTag = "Player";

    private bool hasTriggered = false;
    private Camera mainCamera;
    private AudioSource internalAudioSource; // ใช้เล่นไฟล์เสียงข้างบน

    void Awake()
    {
        mainCamera = Camera.main;

        // สร้าง AudioSource ขึ้นมาใช้ภายใน Object นี้
        internalAudioSource = gameObject.AddComponent<AudioSource>();
        internalAudioSource.playOnAwake = false;
        internalAudioSource.spatialBlend = 0f; // ตั้งเป็น 2D เพื่อให้ได้ยินชัดเจน

        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && !hasTriggered)
        {
            ExecuteEvent();
        }
    }

    void ExecuteEvent()
    {
        hasTriggered = true;

        // 1. เรียกการสั่นกล้อง
        if (shakeStyles.Count > 0 && shakeIndexToUse < shakeStyles.Count)
        {
            StartCoroutine(Shake(shakeStyles[shakeIndexToUse]));
        }

        // 2. สุ่มเล่นไฟล์เสียงจาก List
        PlayEventSound();

        // 3. Particle Once
        foreach (GameObject pOnce in particlesOnce)
        {
            if (pOnce != null)
            {
                pOnce.SetActive(true);
                ParticleSystem ps = pOnce.GetComponentInChildren<ParticleSystem>();
                if (ps != null) ps.Play();
                Destroy(pOnce, destroyDelay);
            }
        }

        // 4. Particle Loop
        foreach (GameObject pLoop in particlesLoop)
        {
            if (pLoop != null)
            {
                pLoop.SetActive(true);
                ParticleSystem ps = pLoop.GetComponentInChildren<ParticleSystem>();
                if (ps != null) ps.Play();
            }
        }

        // 5. Object Removal
        if (objectToRemove != null) HandleObjectRemoval();
    }

    void PlayEventSound()
    {
        if (eventSounds.Count > 0 && internalAudioSource != null)
        {
            // สุ่มเลือก 1 ไฟล์จากรายการ
            int randomIndex = Random.Range(0, eventSounds.Count);
            SoundSettings selected = eventSounds[randomIndex];

            if (selected.clip != null)
            {
                internalAudioSource.PlayOneShot(selected.clip, selected.individualVolume);
            }
        }
    }

    IEnumerator Shake(ShakeSettings settings)
    {
        Vector3 originalPos = mainCamera.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < settings.duration)
        {
            float x = Random.Range(-1f, 1f) * settings.magnitude;
            float y = Random.Range(-1f, 1f) * settings.magnitude;

            mainCamera.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPos;
    }

    void HandleObjectRemoval()
    {
        Collider col = objectToRemove.GetComponentInChildren<Collider>();
        if (col != null) col.enabled = false;

        ParticleSystem ps = objectToRemove.GetComponentInChildren<ParticleSystem>();
        UnityEngine.VFX.VisualEffect vfx = objectToRemove.GetComponentInChildren<UnityEngine.VFX.VisualEffect>();

        if (ps != null) ps.Stop();
        if (vfx != null) vfx.Stop();

        Destroy(objectToRemove, 5f);
    }
}