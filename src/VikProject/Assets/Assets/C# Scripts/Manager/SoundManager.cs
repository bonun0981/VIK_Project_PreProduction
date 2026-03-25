using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source Pool")]
    public AudioSource audioSourcePrefab;
    public int poolSize = 20;

    private List<AudioSource> pool = new List<AudioSource>();

    void Awake()
    {
        Instance = this;
        InitPool();
    }

    void InitPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource src = Instantiate(audioSourcePrefab, transform);
            src.gameObject.SetActive(false);
            pool.Add(src);
        }
    }

    AudioSource GetSource()
    {
        foreach (var src in pool)
        {
            if (!src.gameObject.activeInHierarchy)
                return src;
        }

        // fallback (expand pool)
        AudioSource newSrc = Instantiate(audioSourcePrefab, transform);
        newSrc.gameObject.SetActive(false);
        pool.Add(newSrc);
        return newSrc;
    }

    public Transform listener; // assign เป็น player หรือ camera

    public void Play3DSound(AudioClip clip, Vector3 position,
        float minPitch = 0.9f, float maxPitch = 1.1f,
        float minVolume = 0.8f, float maxVolume = 1f,
        float minDistance = 2f, float maxDistance = 15f)
    {
        if (clip == null) return;

        // ✅ เช็คระยะก่อนเล่นเสียง
        if (listener != null)
        {
            float dist = Vector3.Distance(listener.position, position);

            // ถ้าไกลเกิน → ไม่ต้องเล่นเลย
            if (dist > maxDistance) return;
        }

        AudioSource src = GetSource();

        src.transform.position = position;
        src.gameObject.SetActive(true);

        src.spatialBlend = 1f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.minDistance = minDistance;
        src.maxDistance = maxDistance;

        src.pitch = Random.Range(minPitch, maxPitch);
        src.volume = Random.Range(minVolume, maxVolume);

        src.clip = clip;
        src.Play();

        StartCoroutine(DisableAfterPlay(src));
    }

    System.Collections.IEnumerator DisableAfterPlay(AudioSource src)
    {
        yield return new WaitForSeconds(src.clip.length);
        src.gameObject.SetActive(false);
    }
}