using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsUI : MonoBehaviour
{
    [Header("volume sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("fullscreen toggle")]
    [SerializeField] private Toggle fullscreenToggle;

    [Header("audio mixer")]
    [SerializeField] private AudioMixer audioMixer;

    private const string masterVolumeKey = "master_volume";
    private const string musicVolumeKey = "music_volume";
    private const string sfxVolumeKey = "sfx_volume";
    private const string fullscreenKey = "fullscreen";

    private void Start()
    {
        // loads the saved settings when the game starts
        LoadSettings();

        // listens for changes to the volume sliders
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        // listens for changes to the fullscreen toggle
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    private void LoadSettings()
    {
        // loads the saved volume values or uses full volume by default
        float masterVolume = PlayerPrefs.GetFloat(masterVolumeKey, 1f);
        float musicVolume = PlayerPrefs.GetFloat(musicVolumeKey, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(sfxVolumeKey, 1f);

        // loads the saved fullscreen state or uses the current state by default
        bool fullscreen = PlayerPrefs.GetInt(
            fullscreenKey,
            Screen.fullScreen ? 1 : 0
        ) == 1;

        // updates the ui to match the saved settings
        masterVolumeSlider.value = masterVolume;
        musicVolumeSlider.value = musicVolume;
        sfxVolumeSlider.value = sfxVolume;
        fullscreenToggle.isOn = fullscreen;

        // applies the saved settings
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        SetFullscreen(fullscreen);
    }

    private void SetMasterVolume(float value)
    {
        // saves the master volume setting
        PlayerPrefs.SetFloat(masterVolumeKey, value);

        // applies the volume to the audio mixer
        SetMixerVolume("MasterVolume", value);
    }

    private void SetMusicVolume(float value)
    {
        // saves the music volume setting
        PlayerPrefs.SetFloat(musicVolumeKey, value);

        // applies the volume to the audio mixer
        SetMixerVolume("MusicVolume", value);
    }

    private void SetSFXVolume(float value)
    {
        // saves the sfx volume setting
        PlayerPrefs.SetFloat(sfxVolumeKey, value);

        // applies the volume to the audio mixer
        SetMixerVolume("SFXVolume", value);
    }

    private void SetMixerVolume(string parameterName, float value)
    {
        // prevents log10 from receiving zero
        if (value <= 0.0001f)
        {
            audioMixer.SetFloat(parameterName, -80f);
            return;
        }

        // converts the sliders linear value into decibels
        float decibels = Mathf.Log10(value) * 20f;

        audioMixer.SetFloat(parameterName, decibels);
    }

    private void SetFullscreen(bool enabled)
    {
        // saves the fullscreen setting
        PlayerPrefs.SetInt(fullscreenKey, enabled ? 1 : 0);

        // changes the game between fullscreen and windowed mode
        Screen.fullScreen = enabled;
    }

    private void OnDestroy()
    {
        // removes the slider listeners when this object is destroyed
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
        }
    }
}