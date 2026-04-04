using UnityEngine;
using System.Collections.Generic;

public class EndgameObserver : MonoBehaviour
{
    [System.Serializable]
    public class SoundSettings
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float individualVolume = 1f;
    }

    [Header("Target Settings")]
    [Tooltip("ลากศัตรูที่ต้องกำจัดให้หมดใส่ในนี้")]
    public List<GameObject> targetsToWatch = new List<GameObject>();

    [Header("Object to Remove (Optional)")]
    [Tooltip("เช่น กำแพงหมอก หรือประตูที่จะหายไปเมื่อจบเกม")]
    public GameObject objectToRemove;

    [Header("Audio Settings")]
    public List<SoundSettings> victorySounds = new List<SoundSettings>();

    [Header("Effect Settings")]
    public bool useFadeOut = true;
    [Tooltip("หน่วงเวลาก่อนหน้าจอ GameOver จะขึ้น (วินาที)")]
    public float delayBeforeGameOver = 2f;

    private bool hasTriggered = false;
    private AudioSource audioSource;

    
    void Start()
    {
        hasTriggered = false; // รีเซ็ตตัวล็อกการทำงาน
        Time.timeScale = 1f;  // บังคับให้เวลาเดินปกติเสมอเมื่อเริ่มด่าน
    }
    void Update()
    {
        if (hasTriggered) return;

        if (IsAllTargetsDead())
        {
            TriggerEndgame();
        }
    }

    bool IsAllTargetsDead()
    {
        if (targetsToWatch.Count == 0) return true;

        foreach (GameObject target in targetsToWatch)
        {
            // ตรวจสอบว่ายังมีศัตรูตัวไหนที่ยังมีชีวิตอยู่ (ไม่ null และยัง active)
            if (target != null && target.activeInHierarchy)
                return false;
        }
        return true;
    }

    void TriggerEndgame()
    {
        hasTriggered = true;

        // 1. เล่นเสียงชัยชนะ
        PlayVictorySounds();

        // 2. ลบวัตถุที่กำหนด (เช่น Fog Wall)
        HandleObjectRemoval();

        // 3. เรียกหน้าจอ GameOver (ใช้ Invoke เพื่อให้มีดีเลย์นิดหน่อย ดูไม่ตัดจบจนเกินไป)
        Invoke("CallGameOverUI", delayBeforeGameOver);
    }

    void CallGameOverUI()
    {
        Debug.Log("Calling GameOver UI now!");
        UiManager uiManager = FindObjectOfType<UiManager>();
        if (uiManager != null)
        {
            uiManager.GameOver();
        }
        else
        {
            Debug.LogWarning("ไม่พบ UiManager ใน Scene นี้! อย่าลืมใส่ UiManager ไว้บน GameObject สักอันนะครับ");
        }

    }

    void HandleObjectRemoval()
    {
        if (objectToRemove == null) return;

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

    void PlayVictorySounds()
    {
        if (audioSource == null) return;

        foreach (SoundSettings sound in victorySounds)
        {
            if (sound.clip != null)
            {
                audioSource.PlayOneShot(sound.clip, sound.individualVolume);
            }
        }
    }
}