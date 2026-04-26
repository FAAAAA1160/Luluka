using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace LULUKA
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject mainPanel;
        public GameObject settingsPanel;
        public GameObject aboutPanel;
        public GameObject leaderboardPanel;

        [Header("Settings")]
        public string gameSceneName = "PlayerIn";
        public string authorUrl = "https://github.com/yourusername";

        private GameObject currentPanel;

        private void Start()
        {
            currentPanel = mainPanel;
            HideAllPanels();
            mainPanel.SetActive(true);
        }

        public void OnStartGameClick()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        public void OnSettingsClick()
        {
            SwitchPanel(settingsPanel);
        }

        public void OnLeaderboardClick()
        {
            SwitchPanel(leaderboardPanel);
            var manager = GetComponent<LeaderboardManager>();
            if (manager != null)
            {
                manager.RefreshLeaderboard();
            }
        }

        public void OnAboutClick()
        {
            Application.OpenURL(authorUrl);
        }

        public void OnExitClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void OnBackClick()
        {
            SwitchPanel(mainPanel);
        }

        private void SwitchPanel(GameObject targetPanel)
        {
            if (currentPanel != null)
            {
                currentPanel.SetActive(false);
            }
            targetPanel.SetActive(true);
            currentPanel = targetPanel;
        }

        private void HideAllPanels()
        {
            if (mainPanel != null) mainPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (aboutPanel != null) aboutPanel.SetActive(false);
            if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
        }
    }
}