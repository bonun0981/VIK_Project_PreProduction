using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider effectSlider;
    public Slider environmentSlider;

    void Start()
    {
        // โหลดค่า
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        effectSlider.value = PlayerPrefs.GetFloat("EffectVolume", 1f);
        environmentSlider.value = PlayerPrefs.GetFloat("EnvironmentVolume", 1f);

        // Apply ค่าเริ่มต้น
        ApplyAll();

        // Listener
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        effectSlider.onValueChanged.AddListener(SetEffectVolume);
        environmentSlider.onValueChanged.AddListener(SetEnvironmentVolume);
    }

    void ApplyAll()
    {
        SetMasterVolume(masterSlider.value);
        SetMusicVolume(musicSlider.value);
        SetEffectVolume(effectSlider.value);
        SetEnvironmentVolume(environmentSlider.value);
    }

    // ⭐ แก้ตรงนี้
    float LinearToDB(float value)
    {
        if (value <= 0f)
            return -80f; // mute จริง

        return Mathf.Log10(value) * 20f;
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", LinearToDB(value));
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", LinearToDB(value));
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetEffectVolume(float value)
    {
        audioMixer.SetFloat("EffectVolume", LinearToDB(value));
        PlayerPrefs.SetFloat("EffectVolume", value);
    }

    public void SetEnvironmentVolume(float value)
    {
        audioMixer.SetFloat("EnvironmentVolume", LinearToDB(value));
        PlayerPrefs.SetFloat("EnvironmentVolume", value);
    }
}