using UnityEngine;
using System.Collections.Generic; // ต้องมีบรรทัดนี้เพื่อใช้ List

public class TargetObserver : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("ลาก Boss ทุกตัวในฉากมาใส่ในรายการนี้")]
    public List<GameObject> targetsToWatch = new List<GameObject>();

    [Tooltip("ลากม่านหมอก (FogWall_System) มาใส่ที่นี่")]
    public GameObject objectToRemove;

    [Header("Audio Settings")]
    public AudioClip victorySound;
    [Range(0f, 1f)]
    public float volume = 1f;

    [Header("Effect")]
    public bool useFadeOut = true;

    private bool hasTriggered = false;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = victorySound;
        audioSource.volume = volume;
    }

    void Update()
    {
        if (hasTriggered) return;

        // ตรวจสอบว่าใน List ยังมีบอสที่ยังมีชีวิตอยู่หรือไม่
        if (IsAllTargetsDead())
        {
            RemoveFogWall();
        }
    }

    // ฟังก์ชันเช็คว่าบอสทุกตัวใน List ตายหรือถูกปิดไปหมดแล้วยัง
    bool IsAllTargetsDead()
    {
        // ถ้าไม่มีบอสในลิสต์เลย ให้ถือว่าผ่าน (เปิดหมอก)
        if (targetsToWatch.Count == 0) return true;

        foreach (GameObject target in targetsToWatch)
        {
            // ถ้ายังมีแม้แต่ตัวเดียวที่ยัง Active อยู่ และยังไม่ถูก Destroy ให้ส่งค่า false
            if (target != null && target.activeInHierarchy)
            {
                return false;
            }
        }

        // ถ้าวนลูปจนจบแล้วไม่เจอตัวที่รอดอยู่เลย ให้ส่งค่า true
        return true;
    }

    void RemoveFogWall()
    {
        hasTriggered = true;
        Debug.Log("All targets are inactive or destroyed! Opening Fog Wall...");

        if (victorySound != null && audioSource != null)
        {
            audioSource.Play();
        }

        if (objectToRemove != null)
        {
            // ปิด Collider ทันที
            Collider col = objectToRemove.GetComponentInChildren<Collider>();
            if (col != null) col.enabled = false;

            if (useFadeOut)
            {
                ParticleSystem ps = objectToRemove.GetComponentInChildren<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop();
                    Destroy(objectToRemove, 5f);
                }
                else
                {
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