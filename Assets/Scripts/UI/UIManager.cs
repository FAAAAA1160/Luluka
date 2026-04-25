using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace LULUKA
{
    public class UIManager : MonoBehaviour
    {
        [Header("玩家血条")]
        [SerializeField] private Image playerHealthFill;
        [SerializeField] private Image playerEnergyFill;
        
        [Header("BOSS血条")]
        [SerializeField] private GameObject bossHealthBar;
        [SerializeField] private Image bossHealthFill;
        [SerializeField] private CanvasGroup bossHealthCanvasGroup;
        
        [Header("暂停UI")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private CanvasGroup pauseCanvasGroup;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;
        
        [Header("引用")]
        [SerializeField] private PlayerController player;
        [SerializeField] private BossEnemy boss;
        
        private bool isPaused;
        private bool isBossActive;
        private float bossMaxHealth;
        
        private void Start()
        {
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(OnPauseButtonClick);
            }
            
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartButtonClick);
            }
            
            if (exitButton != null)
            {
                exitButton.onClick.AddListener(OnExitButtonClick);
            }
            
            if (bossHealthBar != null)
            {
                bossHealthBar.SetActive(false);
            }
            
            FindPlayerRef();
            FindBossRef();
        }
        
        private void Update()
        {
            UpdatePlayerHealthUI();
            UpdateBossHealthUI();
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseGame();
            }
        }
        
        private void FindPlayerRef()
        {
            if (player == null)
            {
                player = FindObjectOfType<PlayerController>();
            }
        }
        
        private void FindBossRef()
        {
            if (boss == null)
            {
                boss = FindObjectOfType<BossEnemy>();
                if (boss != null)
                {
                    bossMaxHealth = boss.CurrentHealth;
                }
            }
        }
        
        private void UpdatePlayerHealthUI()
        {
            if (player == null)
            {
                FindPlayerRef();
                return;
            }
            
            if (playerHealthFill != null)
            {
                float healthPercent = player.CurrentHealth / player.MaxHealth;
                playerHealthFill.fillAmount = Mathf.Clamp01(healthPercent);
            }
            
            if (playerEnergyFill != null)
            {
                playerEnergyFill.fillAmount = 0f;
            }
        }
        
        private void UpdateBossHealthUI()
        {
            if (boss == null)
            {
                FindBossRef();
                return;
            }
            
            if (boss.CurrentHealth <= 0)
            {
                HideBossHealthBarUI();
                return;
            }
            
            if (boss.IsChasing && !isBossActive)
            {
                isBossActive = true;
                ShowBossHealthBarUI();
            }
            
            if (isBossActive && bossHealthFill != null)
            {
                float healthPercent = boss.CurrentHealth / bossMaxHealth;
                bossHealthFill.fillAmount = Mathf.Clamp01(healthPercent);
            }
        }
        
        private void ShowBossHealthBarUI()
        {
            if (bossHealthBar != null)
            {
                bossHealthBar.SetActive(true);
            }
            
            if (bossHealthCanvasGroup != null)
            {
                bossHealthCanvasGroup.alpha = 1f;
                bossHealthCanvasGroup.blocksRaycasts = false;
            }
        }
        
        private void HideBossHealthBarUI()
        {
            if (bossHealthCanvasGroup != null)
            {
                bossHealthCanvasGroup.alpha = 0f;
            }
            
            isBossActive = false;
        }
        
        private void OnPauseButtonClick()
        {
            TogglePauseGame();
        }
        
        private void TogglePauseGame()
        {
            isPaused = !isPaused;
            
            if (isPaused)
            {
                ShowPausePanelUI();
                Time.timeScale = 0f;
            }
            else
            {
                HidePausePanelUI();
                Time.timeScale = 1f;
            }
        }
        
        private void ShowPausePanelUI()
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }
            
            if (pauseCanvasGroup != null)
            {
                pauseCanvasGroup.alpha = 1f;
                pauseCanvasGroup.blocksRaycasts = true;
            }
        }
        
        private void HidePausePanelUI()
        {
            if (pauseCanvasGroup != null)
            {
                pauseCanvasGroup.alpha = 0f;
                pauseCanvasGroup.blocksRaycasts = false;
            }
        }
        
        private void OnRestartButtonClick()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        private void OnExitButtonClick()
        {
            Time.timeScale = 1f;
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}