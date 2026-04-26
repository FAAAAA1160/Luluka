using System;
using System.Linq;
using UnityEngine;

namespace LULUKA
{
    public class GameSettings : MonoBehaviour
    {
        public static GameSettings Instance { get; private set; }
        
        [Header("音频设置")]
        [SerializeField] private float masterVolume = 1f;
        [SerializeField] private float musicVolume = 1f;
        [SerializeField] private float sfxVolume = 1f;
        
        [Header("分辨率设置")]
        [SerializeField] private bool isFullscreen = true;
        [SerializeField] private int resolutionIndex = 0;
        
        private Resolution[] resolutions;
        
        private const string MasterVolumeKey = "MasterVolume";
        private const string MusicVolumeKey = "MusicVolume";
        private const string SFXVolumeKey = "SFXVolume";
        private const string FullscreenKey = "Fullscreen";
        private const string ResolutionKey = "ResolutionIndex";
        
        public float MasterVolume => masterVolume;
        public float MusicVolume => musicVolume;
        public float SFXVolume => sfxVolume;
        public bool IsFullscreen => isFullscreen;
        public int ResolutionIndex => resolutionIndex;
        public Resolution[] Resolutions => resolutions;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeResolutions();
                LoadSettings();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeResolutions()
        {
            var allResolutions = Screen.resolutions;
            resolutions = allResolutions;
        }
        
        public void SetMasterVolume(float volume)
        {
            masterVolume = volume;
            SaveSettings();
        }
        
        public void SetMusicVolume(float volume)
        {
            musicVolume = volume;
            SaveSettings();
        }
        
        public void SetSFXVolume(float volume)
        {
            sfxVolume = volume;
            SaveSettings();
        }
        
        public void SetFullscreen(bool fullscreen)
        {
            isFullscreen = fullscreen;
            Screen.fullScreen = fullscreen;
            SaveSettings();
        }
        
        public void SetResolution(int index)
        {
            if (resolutions == null || index < 0 || index >= resolutions.Length)
                return;
            
            resolutionIndex = index;
            Resolution res = resolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            SaveSettings();
        }
        
        public void ApplyCurrentSettings()
        {
            SetFullscreen(isFullscreen);
            SetResolution(resolutionIndex);
        }
        
        private void SaveSettings()
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, masterVolume);
            PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
            PlayerPrefs.SetFloat(SFXVolumeKey, sfxVolume);
            PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0);
            PlayerPrefs.SetInt(ResolutionKey, resolutionIndex);
            PlayerPrefs.Save();
        }
        
        private void LoadSettings()
        {
            masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
            musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
            sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
            isFullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;
            resolutionIndex = PlayerPrefs.GetInt(ResolutionKey, 0);
            
            ApplyCurrentSettings();
        }
    }
}