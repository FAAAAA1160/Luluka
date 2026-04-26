using UnityEngine;
using UnityEngine.UI;

namespace LULUKA
{
    public class GameOverUI : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private CanvasGroup canvasGroup;
        
        [Header("胜利UI")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private Text winTimeText;
        [SerializeField] private Text winRankText;
        
        [Header("失败UI")]
        [SerializeField] private GameObject losePanel;
        
        [Header("按钮")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;
        
        private void Start()
        {
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClick);
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuClick);
            }
            
            Hide();
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameEnd += Show;
            }
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameEnd -= Show;
            }
        }
        
        public void Show()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }
            
            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.IsGameWon)
                {
                    ShowWinScreen();
                }
                else
                {
                    ShowLoseScreen();
                }
            }
        }
        
        public void Hide()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
            }
        }
        
        private void ShowWinScreen()
        {
            if (winPanel != null) winPanel.SetActive(true);
            if (losePanel != null) losePanel.SetActive(false);
            
            if (winTimeText != null && GameManager.Instance != null)
            {
                winTimeText.text = "通关时间: " + GameManager.Instance.GetFormattedTime();
            }
            
            if (winRankText != null && GameManager.Instance != null)
            {
                int rank = LeaderboardManager.GetRank(GameManager.Instance.GameTime);
                winRankText.text = "排名: #" + rank;
            }
        }
        
        private void ShowLoseScreen()
        {
            if (winPanel != null) winPanel.SetActive(false);
            if (losePanel != null) losePanel.SetActive(true);
        }
        
        private void OnRestartClick()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        }
        
        private void OnMainMenuClick()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReturnToMainMenu();
            }
        }
    }
}