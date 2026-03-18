using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundManager : MonoBehaviour
{
    [Header("Audio Sources (3D)")]
    [SerializeField] private AudioSource oneShotSource;
    [SerializeField] private AudioSource movementSource;

    [Header("Sound Sets")]
    [SerializeField] private List<AudioClip> attackClips;
    [SerializeField] private List<AudioClip> hurtClips;
    [SerializeField] private List<AudioClip> walkClips;
    [SerializeField] private List<AudioClip> runClips;

    [Header("Random Settings")]
    [SerializeField] private Vector2 pitchRange = new Vector2(0.9f, 1.1f);
    [SerializeField] private Vector2 volumeRange = new Vector2(0.8f, 1f);

    AudioClip lastClip;

    // ------------------------
    // PUBLIC API
    // ------------------------

    public void PlayAttack()
    {
        PlayOneShot(attackClips);
    }

    public void PlayHurt()
    {
        PlayOneShot(hurtClips);
    }

    public void PlayFootstep(bool isRunning)
    {
        if (isRunning)
            PlayMovement(runClips);
        else
            PlayMovement(walkClips);
    }

    // ------------------------
    // CORE
    // ------------------------

    void PlayOneShot(List<AudioClip> clips)
    {
        if (clips == null || clips.Count == 0) return;

        AudioClip clip = GetRandomClip(clips);

        ApplyRandom(oneShotSource);

        oneShotSource.PlayOneShot(clip);
    }

    void PlayMovement(List<AudioClip> clips)
    {
        if (clips == null || clips.Count == 0) return;

        AudioClip clip = GetRandomClip(clips);

        ApplyRandom(movementSource);

        movementSource.PlayOneShot(clip);
    }

    void ApplyRandom(AudioSource source)
    {
        source.pitch = Random.Range(pitchRange.x, pitchRange.y);
        source.volume = Random.Range(volumeRange.x, volumeRange.y);
    }

    AudioClip GetRandomClip(List<AudioClip> clips)
    {
        if (clips.Count == 1) return clips[0];

        AudioClip newClip;

        do
        {
            newClip = clips[Random.Range(0, clips.Count)];
        }
        while (newClip == lastClip);

        lastClip = newClip;

        return newClip;
    }
}