using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class RandomFireSound : MonoBehaviour
{
    [Header("Audio Selection")]
    [Tooltip("ใส่ไฟล์เสียงไฟไหม้หลายๆ แบบที่นี่")]
    public List<AudioClip> fireClips = new List<AudioClip>();

    [Header("Random Settings")]
    [Tooltip("สุ่มความเร็ว/โทนเสียง (Pitch) เพื่อความหลากหลาย")]
    [Range(0.1f, 0.5f)]
    public float pitchRandomness = 0.2f;

    [Tooltip("สุ่มระดับความดังเริ่มต้นเล็กน้อย")]
    [Range(0f, 0.3f)]
    public float volumeRandomness = 0.1f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // บังคับการตั้งค่าพื้นฐานสำหรับเสียง 3D
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        PlayRandomFire();
    }

    void PlayRandomFire()
    {
        if (fireClips.Count == 0)
        {
            Debug.LogWarning("กรุณาใส่ AudioClips ใน List ด้วยครับ!");
            return;
        }

        // 1. สุ่มเลือกไฟล์จาก List
        AudioClip selectedClip = fireClips[Random.Range(0, fireClips.Count)];
        audioSource.clip = selectedClip;

        // 2. สุ่มจุดเริ่มเล่น (สำคัญมาก! เพื่อไม่ให้เสียงเริ่มที่วินาทีที่ 0 พร้อมกัน)
        float randomStartTime = Random.Range(0f, selectedClip.length);
        audioSource.time = randomStartTime;

        // 3. สุ่ม Pitch (โทนเสียง)
        // เช่น ถ้าเดิมคือ 1.0 จะสุ่มอยู่ระหว่าง 0.9 - 1.1
        float basePitch = 1.0f;
        audioSource.pitch = basePitch + Random.Range(-pitchRandomness, pitchRandomness);

        // 4. สุ่ม Volume เล็กน้อย
        float baseVolume = audioSource.volume;
        audioSource.volume = baseVolume + Random.Range(-volumeRandomness, volumeRandomness);

        // 5. เริ่มเล่น
        audioSource.Play();
    }
}