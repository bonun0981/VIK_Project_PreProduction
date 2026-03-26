using UnityEngine;

public class EntitySoundEmitter : MonoBehaviour
{
    [Header("Sound Lists")]
    public AudioClip[] attackClips;
    public AudioClip[] hurtClips;
    public AudioClip[] walkClips;
    public AudioClip[] runClips;

    AudioClip GetRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }

    // -------------------------
    // CALL FROM ANIMATION / AI
    // -------------------------

    public void PlayAttack()
    {
        SoundManager.Instance.Play3DSound(
            GetRandom(attackClips),
            transform.position
        );
    }

    public void PlayHurt()
    {
        SoundManager.Instance.Play3DSound(
            GetRandom(hurtClips),
            transform.position
        );
    }

    public void PlayWalk()
    {
        SoundManager.Instance.Play3DSound(
            GetRandom(walkClips),
            transform.position,
            0.95f, 1.05f,
            0.6f, 0.9f
        );
    }

    public void PlayRun()
    {
        SoundManager.Instance.Play3DSound(
            GetRandom(runClips),
            transform.position,
            0.95f, 1.1f,
            0.8f, 1f
        );
    }
}