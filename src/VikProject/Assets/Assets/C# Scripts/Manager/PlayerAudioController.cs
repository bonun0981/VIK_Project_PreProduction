using Unity.VisualScripting;
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
    [Header("Footstep Volume")]
    [SerializeField] float walkVolume = 0.5f;
    [SerializeField] float runVolume = 0.8f;
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Combat")]
    public AudioClip[] attackClips;
    public AudioClip[] hurtClips;

    [Header("Footsteps")]
    public TerrainFootstep[] terrainFootsteps;
    public float rayRange = 2f;

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

    public void PlayWalk()
    {
        var terrain = GetTerrain(currentTerrain);
        if (terrain == null) return;

        PlayRandom(terrain.walkClips, walkVolume);
        Debug.Log("walk");
    }

    public void PlayRun()
    {
        var terrain = GetTerrain(currentTerrain);
        if (terrain == null) return;

        PlayRandom(terrain.runClips, runVolume);
        Debug.Log("run");
    }
    // ---------------------------
    // CORE LOGIC
    // ---------------------------

    void PlayRandom(AudioClip[] clips, float volumeMultiplier = 1f)
    {
        if (clips == null || clips.Length == 0) return;

        int index;
        do
        {
            index = Random.Range(0, clips.Length);
        } while (clips.Length > 1 && index == lastIndex);

        lastIndex = index;

        audioSource.pitch = Random.Range(minPitch, maxPitch);

        float randomVolume = Random.Range(minVolume, maxVolume);

        // 🔥 ใช้ PlayOneShot + volumeMultiplier
        audioSource.PlayOneShot(clips[index], randomVolume * volumeMultiplier);
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

        // 🔼 ยกจุดเริ่ม ray ขึ้น
        Vector3 origin = transform.position + Vector3.up * 0.3f;

        // 🔴 debug ray
        Debug.DrawRay(origin, Vector3.down * rayRange, Color.red);

        if (Physics.Raycast(origin, Vector3.down, out hit, rayRange))
        {
            currentTerrain = hit.collider.tag;

            // 🟢 hit แล้วเป็นสีเขียว
            Debug.DrawRay(origin, Vector3.down * hit.distance, Color.green);

            Debug.Log("Hit: " + hit.collider.name + " | Tag: " + currentTerrain);
        }
        else
        {
            currentTerrain = null;
            Debug.LogWarning("No terrain detected!");
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector3 origin = transform.position + Vector3.up * 0.3f;
        Gizmos.DrawLine(origin, origin + Vector3.down * rayRange);
    }
}