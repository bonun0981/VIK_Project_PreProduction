using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource oneShotSource; // attack / hurt
    [SerializeField] private AudioSource movementSource; // walk / run

    [Header("Player Sound")]
    [SerializeField] private List<AudioClip> playerAttackSound;
    [SerializeField] private List<AudioClip> playerHurtSound;
    [SerializeField] private List<AudioClip> playerWalkSound;
    [SerializeField] private List<AudioClip> playerRunSound;

    [Header("Random Settings")]
    [SerializeField] private Vector2 pitchRange = new Vector2(0.9f, 1.1f);
    [SerializeField] private Vector2 volumeRange = new Vector2(0.8f, 1f);

    // -------------------------
    // PUBLIC METHODS
    // -------------------------

    public void PlayAttack()
    {
        PlayRandomOneShot(playerAttackSound);
    }

    public void PlayHurt()
    {
        PlayRandomOneShot(playerHurtSound);
    }

    public void PlayFootstep(bool isRunning)
    {
        if (isRunning)
            PlayMovement(playerRunSound);
        else
            PlayMovement(playerWalkSound);
    }

    // -------------------------
    // CORE LOGIC
    // -------------------------

    void PlayRandomOneShot(List<AudioClip> clips)
    {
        if (clips == null || clips.Count == 0) return;

        AudioClip clip = GetRandomClip(clips);

        oneShotSource.pitch = GetRandomPitch();
        oneShotSource.volume = GetRandomVolume();

        oneShotSource.PlayOneShot(clip);
    }

    void PlayMovement(List<AudioClip> clips)
    {
        if (clips == null || clips.Count == 0) return;

        AudioClip clip = GetRandomClip(clips);

        movementSource.pitch = GetRandomPitch();
        movementSource.volume = GetRandomVolume();

        movementSource.PlayOneShot(clip);
    }

    AudioClip lastClip;

    AudioClip GetRandomClip(List<AudioClip> clips)
    {
        if (clips.Count == 1) return clips[0];

        AudioClip newClip;

        do
        {
            newClip = clips[Random.Range(0, clips.Count)];
        } while (newClip == lastClip);

        lastClip = newClip;
        return newClip;
    }

    float GetRandomPitch()
    {
        return Random.Range(pitchRange.x, pitchRange.y);
    }

    float GetRandomVolume()
    {
        return Random.Range(volumeRange.x, volumeRange.y);
    }
}