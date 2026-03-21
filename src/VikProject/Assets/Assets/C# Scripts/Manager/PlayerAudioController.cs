using UnityEngine;

[System.Serializable]
public class TerrainFootstep
{
    public string terrainTag;
    public AudioClip[] walkClips;
    public AudioClip[] runClips;
}

public class PlayerAudioController : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Combat")]
    public AudioClip[] attackClips;
    public AudioClip[] hurtClips;

    [Header("Footsteps")]
    public TerrainFootstep[] terrainFootsteps;

    [Header("Random Settings")]
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    public float minVolume = 0.8f;
    public float maxVolume = 1f;

    private string currentTerrain;
    private int lastIndex = -1;

    // ---------------------------
    // PUBLIC API (เรียกจาก Animation)
    // ---------------------------

    public void PlayAttack()
    {
        PlayRandom(attackClips);
    }

    public void PlayHurt()
    {
        PlayRandom(hurtClips);
    }

    public void PlayFootstep(bool isRunning)
    {
        var terrain = GetTerrain(currentTerrain);
        if (terrain == null) return;

        if (isRunning)
            PlayRandom(terrain.runClips);
        else
            PlayRandom(terrain.walkClips);
    }

    // ---------------------------
    // CORE LOGIC
    // ---------------------------

    void PlayRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;

        int index;
        do
        {
            index = Random.Range(0, clips.Length);
        } while (clips.Length > 1 && index == lastIndex);

        lastIndex = index;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.volume = Random.Range(minVolume, maxVolume);

        audioSource.PlayOneShot(clips[index]);
    }

    TerrainFootstep GetTerrain(string tag)
    {
        foreach (var t in terrainFootsteps)
        {
            if (t.terrainTag == tag)
                return t;
        }
        return null;
    }

    void Update()
    {
        DetectTerrain();
    }

    void DetectTerrain()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            currentTerrain = hit.collider.tag;
        }
    }
}