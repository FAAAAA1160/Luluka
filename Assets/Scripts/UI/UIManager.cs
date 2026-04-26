using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace LULUKA
{
    public class UIManager : MonoBehaviour
    {
        [Header("玩家血条")]
        [SerializeField] private Slider playerHealthSlider;
        [SerializeField] private Slider playerEnergySlider;
        
        [Header("BOSS血条")]
        [SerializeField] private GameObject bossHealthBar;
        [SerializeField] private Slider bossHealthSlider;
        [SerializeField] private CanvasGroup bossHealthCanvasGroup;
        
        [Header("计时器UI")]
        [SerializeField] private Text timerText;
        
        [Header("暂停UI")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private CanvasGroup pauseCanvasGroup;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button pauseBackMenuButton;
        [SerializeField] private Button exitButton;
        
        [Header("游戏结束UI")]
        [SerializeField] private GameObject overPanel;
        [SerializeField] private CanvasGroup overCanvasGroup;
        [SerializeField] private GameObject winObject;
        [SerializeField] private GameObject loseObject;
        [SerializeField] private Button overRestartButton;
        [SerializeField] private Button overBackMenuButton;
        [SerializeField] private Button overExitButton;
        [SerializeField] private InputField userNameInput;
        [SerializeField] private Button saveButton;
        
        [Header("引用")]
        [SerializeField] private PlayerController player;
        [SerializeField] private BossEnemy boss;
        
        private bool isPaused;
        private bool isGameOver;
        private bool isBossActive;
        private float bossMaxHealth;
        
        public static UIManager Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
            isGameOver = false;
            isPaused = false;
            isBossActive = false;
        }
        
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
            
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }
            
            if (pauseBackMenuButton != null)
            {
                pauseBackMenuButton.onClick.AddListener(OnBackMenuButtonClick);
            }
            
            if (exitButton != null)
            {
                exitButton.onClick.AddListener(OnExitButtonClick);
            }
            
            if (overRestartButton != null)
            {
                overRestartButton.onClick.AddListener(OnRestartButtonClick);
            }
            
            if (overBackMenuButton != null)
            {
                overBackMenuButton.onClick.AddListener(OnBackMenuButtonClick);
            }
            
            if (overExitButton != null)
            {
                overExitButton.onClick.AddListener(OnExitButtonClick);
            }
            
            if (saveButton != null)
            {
                saveButton.onClick.AddListener(OnSaveButtonClick);
            }
            
            if (bossHealthBar != null)
            {
                bossHealthBar.SetActive(true);
            }
            
            if (bossHealthCanvasGroup != null)
            {
                bossHealthCanvasGroup.alpha = 0f;
                bossHealthCanvasGroup.blocksRaycasts = false;
            }
            
            if (bossHealthSlider != null)
            {
                bossHealthSlider.value = 1f;
            }
            
            if (pauseCanvasGroup != null)
            {
                pauseCanvasGroup.alpha = 0f;
                pauseCanvasGroup.blocksRaycasts = false;
            }
            
            if (overCanvasGroup != null)
            {
                overCanvasGroup.alpha = 0f;
                overCanvasGroup.blocksRaycasts = false;
            }
            
            if (overPanel != null)
            {
                overPanel.SetActive(false);
            }
            
            FindPlayerRef();
            FindBossRef();
        }
        
        private void Update()
        {
            if (isGameOver) return;
            
            UpdatePlayerHealthUI();
            UpdatePlayerEnergyUI();
            UpdateBossHealthUI();
            UpdateTimerUI();
            
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
            }
            
            if (boss != null && boss.Config != null)
            {
                bossMaxHealth = boss.Config.maxHealth;
            }
        }
        
        private void UpdatePlayerHealthUI()
        {
            if (player == null)
            {
                FindPlayerRef();
                return;
            }
            
            if (playerHealthSlider != null)
            {
                float healthPercent = player.CurrentHealth / player.MaxHealth;
                playerHealthSlider.value = Mathf.Clamp01(healthPercent);
            }
        }
        
        private void UpdatePlayerEnergyUI()
        {
            if (player == null)
            {
                FindPlayerRef();
                return;
            }
            
            if (playerEnergySlider != null)
            {
                float energyPercent = player.CurrentEnergy / player.MaxEnergy;
                playerEnergySlider.value = Mathf.Clamp01(energyPercent);
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
            
            if (isBossActive && bossHealthSlider != null && bossMaxHealth > 0)
            {
                float healthPercent = boss.CurrentHealth / bossMaxHealth;
                bossHealthSlider.value = Mathf.Clamp01(healthPercent);
            }
        }
        
        private void UpdateTimerUI()
        {
            if (timerText != null && GameManager.Instance != null)
            {
                if (GameManager.Instance.IsGameRunning && !isPaused)
                {
                    timerText.text = GameManager.Instance.GetFormattedTime();
                }
            }
        }
        
        private void ShowBossHealthBarUI()
        {
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
                bossHealthCanvasGroup.blocksRaycasts = false;
            }
            
            isBossActive = false;
        }
        
        private void OnPauseButtonClick()
        {
            TogglePauseGame();
        }
        
        private void OnCloseButtonClick()
        {
            if (isPaused)
            {
                TogglePauseGame();
            }
        }
        
        private void OnBackMenuButtonClick()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
        
        private void TogglePauseGame()
        {
            isPaused = !isPaused;
            
            if (isPaused)
            {
                ShowPausePanelUI();
                Time.timeScale = 0f;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.PauseGame();
                }
            }
            else
            {
                HidePausePanelUI();
                Time.timeScale = 1f;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ResumeGame();
                }
            }
        }
        
        private void ShowPausePanelUI()
        {
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
        
        public void ShowGameOverUI(bool isWin)
        {
            isGameOver = true;
            Time.timeScale = 0f;
            
            if (overPanel != null)
            {
                overPanel.SetActive(true);
            }
            
            if (overCanvasGroup != null)
            {
                overCanvasGroup.alpha = 1f;
                overCanvasGroup.blocksRaycasts = true;
            }
            
            if (winObject != null)
            {
                winObject.SetActive(isWin);
            }
            
            if (loseObject != null)
            {
                loseObject.SetActive(!isWin);
            }
            
            if (userNameInput != null)
            {
                userNameInput.gameObject.SetActive(isWin);
            }
            
            if (saveButton != null)
            {
                saveButton.gameObject.SetActive(isWin);
            }
        }
        
        private void OnSaveButtonClick()
        {
            if (userNameInput != null && GameManager.Instance != null)
            {
                string playerName = string.IsNullOrEmpty(userNameInput.text) ? "玩家" : userNameInput.text;
                LeaderboardManager.RecordCompletion(GameManager.Instance.GameTime, playerName);
            }
            
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
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