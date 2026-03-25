using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [Header("Audio Clip")]
    [SerializeField] private AudioClip backgroundMusic;
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();
    }
}
