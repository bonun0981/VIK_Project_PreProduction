using UnityEngine;

public class TargetObserver : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("ลาก Boss ใน Hierarchy มาใส่ที่นี่")]
    public GameObject targetToWatch;

    [Tooltip("ลากม่านหมอก (FogWall_System) มาใส่ที่นี่")]
    public GameObject objectToRemove;

    [Header("Audio Settings")]
    [Tooltip("ลากไฟล์เสียงที่ต้องการให้เล่นตอนม่านสลายมาใส่ที่นี่")]
    public AudioClip victorySound;
    [Range(0f, 1f)]
    public float volume = 1f;

    [Header("Effect")]
    public bool useFadeOut = true;

    private bool hasTriggered = false;
    private AudioSource audioSource;

    void Awake()
    {
        // สร้าง AudioSource อัตโนมัติเพื่อใช้เล่นเสียง
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = victorySound;
        audioSource.volume = volume;
    }

    void Update()
    {
        if (!hasTriggered && targetToWatch != null)
        {
            if (!targetToWatch.activeInHierarchy)
            {
                RemoveFogWall();
            }
        }
        else if (!hasTriggered && targetToWatch == null)
        {
            RemoveFogWall();
        }
    }

    void RemoveFogWall()
    {
        hasTriggered = true;
        Debug.Log("Target is inactive or destroyed! Playing Sound and Removing Fog Wall...");

        // 1. เล่นเสียงที่กำหนดไว้
        if (victorySound != null && audioSource != null)
        {
            audioSource.Play();
        }

        if (objectToRemove != null)
        {
            // 2. ปิด Collider ทันทีเพื่อให้เดินผ่านได้เลย
            Collider col = objectToRemove.GetComponentInChildren<Collider>();
            if (col != null) col.enabled = false;

            if (useFadeOut)
            {
                // สำหรับ Particle System
                ParticleSystem ps = objectToRemove.GetComponentInChildren<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop();
                    Destroy(objectToRemove, 5f);
                }
                else
                {
                    // สำหรับ VFX Graph
                    UnityEngine.VFX.VisualEffect vfx = objectToRemove.GetComponentInChildren<UnityEngine.VFX.VisualEffect>();
                    if (vfx != null)
                    {
                        vfx.Stop();
                        Destroy(objectToRemove, 5f);
                    }
                    else
                    {
                        Destroy(objectToRemove);
                    }
                }
            }
            else
            {
                Destroy(objectToRemove);
            }
        }
    }
}