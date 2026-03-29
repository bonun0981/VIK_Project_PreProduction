using UnityEngine;
using System.Collections.Generic;

public class TargetObserver : MonoBehaviour
{
    // สร้าง Class สำหรับเก็บข้อมูลเสียงรายตัว
    [System.Serializable]
    public class SoundSettings
    {
        public string name; // เอาไว้ใส่ชื่อกันหลงใน Inspector
        public AudioClip clip;
        [Range(0f, 1f)]
        public float individualVolume = 1f;
    }

    [Header("Settings")]
    public List<GameObject> targetsToWatch = new List<GameObject>();
    public GameObject objectToRemove;

    [Header("Audio Settings")]
    [Tooltip("ใส่เสียงและปรับระดับเสียงแยกแต่ละไฟล์ได้ที่นี่")]
    public List<SoundSettings> victorySounds = new List<SoundSettings>();

    [Header("Effect")]
    public bool useFadeOut = true;

    private bool hasTriggered = false;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (hasTriggered) return;
        if (IsAllTargetsDead()) RemoveFogWall();
    }

    bool IsAllTargetsDead()
    {
        if (targetsToWatch.Count == 0) return true;
        foreach (GameObject target in targetsToWatch)
        {
            if (target != null && target.CompareTag("Enemy") && target.activeInHierarchy)
                return false;
        }
        return true;
    }

    void RemoveFogWall()
    {
        hasTriggered = true;
        PlayVictorySounds();

        if (objectToRemove != null)
        {
            Collider col = objectToRemove.GetComponentInChildren<Collider>();
            if (col != null) col.enabled = false;

            if (useFadeOut)
            {
                var ps = objectToRemove.GetComponentInChildren<ParticleSystem>();
                var vfx = objectToRemove.GetComponentInChildren<UnityEngine.VFX.VisualEffect>();

                if (ps != null) ps.Stop();
                if (vfx != null) vfx.Stop();

                Destroy(objectToRemove, 5f);
            }
            else
            {
                Destroy(objectToRemove);
            }
        }
    }

    void PlayVictorySounds()
    {
        if (audioSource == null) return;

        foreach (SoundSettings sound in victorySounds)
        {
            if (sound.clip != null)
            {
                // เล่นเสียงโดยใช้ระดับเสียงเฉพาะของไฟล์นั้นๆ
                audioSource.PlayOneShot(sound.clip, sound.individualVolume);
            }
        }
    }
}