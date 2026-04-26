using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LULUKA
{
    public class SettingsManager : MonoBehaviour
    {
        [Header("Resolution Settings")]
        public Dropdown resolutionDropdown;
        public Toggle fullscreenToggle;

        [Header("Audio Settings")]
        public Slider masterVolumeSlider;
        public Slider musicVolumeSlider;
        public Slider sfxVolumeSlider;

        private Resolution[] resolutions;
        private List<string> resolutionOptions = new List<string>();
        private int currentResolutionIndex = 0;

        private const string PREF_RESOLUTION = "Settings_Resolution";
        private const string PREF_FULLSCREEN = "Settings_Fullscreen";

        private void Start()
        {
            InitializeResolutions();
            LoadSettings();
        }

        private void InitializeResolutions()
        {
            resolutions = Screen.resolutions;
            resolutionOptions.Clear();

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "Hz";
                resolutionOptions.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            if (resolutionDropdown != null)
            {
                resolutionDropdown.ClearOptions();
                resolutionDropdown.AddOptions(resolutionOptions);
                resolutionDropdown.value = currentResolutionIndex;
                resolutionDropdown.RefreshShownValue();
                resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            }

            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = Screen.fullScreen;
                fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
            }
        }

        public void OnResolutionChanged(int index)
        {
            currentResolutionIndex = index;
            Resolution resolution = resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            SaveSettings();
        }

        public void OnFullscreenChanged(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
            SaveSettings();
        }

        public void OnMasterVolumeChanged(float volume)
        {
            PlayerPrefs.SetFloat("MasterVolume", volume);
            PlayerPrefs.Save();
        }

        public void OnMusicVolumeChanged(float volume)
        {
            PlayerPrefs.SetFloat("MusicVolume", volume);
            PlayerPrefs.Save();
        }

        public void OnSFXVolumeChanged(float volume)
        {
            PlayerPrefs.SetFloat("SFXVolume", volume);
            PlayerPrefs.Save();
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetInt(PREF_RESOLUTION, currentResolutionIndex);
            PlayerPrefs.SetInt(PREF_FULLSCREEN, Screen.fullScreen ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void LoadSettings()
        {
            if (PlayerPrefs.HasKey(PREF_RESOLUTION))
            {
                int savedIndex = PlayerPrefs.GetInt(PREF_RESOLUTION);
                if (savedIndex < resolutions.Length)
                {
                    currentResolutionIndex = savedIndex;
                    if (resolutionDropdown != null)
                    {
                        resolutionDropdown.value = currentResolutionIndex;
                        resolutionDropdown.RefreshShownValue();
                    }
                    Resolution resolution = resolutions[currentResolutionIndex];
                    Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
                }
            }

            if (PlayerPrefs.HasKey(PREF_FULLSCREEN))
            {
                bool isFullscreen = PlayerPrefs.GetInt(PREF_FULLSCREEN) == 1;
                Screen.fullScreen = isFullscreen;
                if (fullscreenToggle != null)
                {
                    fullscreenToggle.isOn = isFullscreen;
                }
            }
        }
    }
}