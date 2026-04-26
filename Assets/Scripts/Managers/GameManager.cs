using UnityEngine;
using UnityEngine.SceneManagement;

namespace LULUKA
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("游戏状态")]
        public bool IsGameRunning { get; private set; }
        public bool IsGamePaused { get; private set; }
        public bool IsGameWon { get; private set; }
        
        [Header("计时器")]
        public float GameTime { get; private set; }
        
        [Header("场景设置")]
        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private string gameScene = "PlayerIn";
        
        public System.Action OnGameStart;
        public System.Action OnGameEnd;
        public System.Action OnGameWon;
        public System.Action OnGameLost;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == gameScene)
            {
                StartGame();
            }
        }
        
        private void Start()
        {
            if (UnityEngine.Object.FindObjectOfType<UIManager>() != null)
            {
                StartGame();
            }
        }
        
        private void Update()
        {
            if (IsGameRunning && !IsGamePaused)
            {
                GameTime += Time.deltaTime;
            }
        }
        
        public void StartGame()
        {
            IsGameRunning = true;
            IsGamePaused = false;
            IsGameWon = false;
            GameTime = 0f;
            Time.timeScale = 1f;
            OnGameStart?.Invoke();
        }
        
        public void PauseGame()
        {
            IsGamePaused = true;
            Time.timeScale = 0f;
        }
        
        public void ResumeGame()
        {
            IsGamePaused = false;
            Time.timeScale = 1f;
        }
        
        public void WinGame()
        {
            if (!IsGameRunning) return;
            
            IsGameRunning = false;
            IsGameWon = true;
            Time.timeScale = 0f;
            
            OnGameWon?.Invoke();
            OnGameEnd?.Invoke();
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOverUI(true);
            }
        }
        
        public void LoseGame()
        {
            if (!IsGameRunning) return;
            
            IsGameRunning = false;
            IsGameWon = false;
            Time.timeScale = 0f;
            
            OnGameLost?.Invoke();
            OnGameEnd?.Invoke();
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOverUI(false);
            }
        }
        
        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuScene);
        }
        
        public string GetFormattedTime()
        {
            int minutes = Mathf.FloorToInt(GameTime / 60f);
            int seconds = Mathf.FloorToInt(GameTime % 60f);
            int milliseconds = Mathf.FloorToInt((GameTime * 100f) % 100f);
            return $"{minutes:00}:{seconds:00}.{milliseconds:02}";
        }
        
        public static string FormatTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
            return $"{minutes:00}:{seconds:00}.{milliseconds:02}";
        }
    }
}