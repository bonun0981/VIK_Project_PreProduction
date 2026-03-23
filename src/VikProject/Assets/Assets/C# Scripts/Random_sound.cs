using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomAudioStarter : MonoBehaviour
{
    [Header("Settings")]
    [Range(0.8f, 1.2f)]
    public float minPitch = 0.9f;
    [Range(0.8f, 1.2f)]
    public float maxPitch = 1.1f;

    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        // ตรวจสอบว่ามี Clip หรือไม่
        if (_audioSource.clip == null) return;

        // 1. สุ่มจุดเริ่มเล่น (Time) ภายในความยาวของ Clip
        // วิธีนี้จะทำให้เสียงไฟแต่ละกองเริ่ม "ซู่" คนละจังหวะกัน
        float randomStartTime = Random.Range(0f, _audioSource.clip.length);
        _audioSource.time = randomStartTime;

        // 2. สุ่ม Pitch เล็กน้อย (ช่วยให้โทนเสียงต่างกัน ไม่ดูเป็น Robot)
        _audioSource.pitch = Random.Range(minPitch, maxPitch);

        // 3. สั่ง Play
        _audioSource.Play();
    }
}