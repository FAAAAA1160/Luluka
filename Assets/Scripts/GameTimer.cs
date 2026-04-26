using UnityEngine;
using UnityEngine.SceneManagement;

namespace LULUKA
{
    public class GameTimer : MonoBehaviour
    {
        public static GameTimer Instance { get; private set; }

        private float startTime;
        private bool isRunning;
        private float currentPlayTime;

        public float CurrentPlayTime => currentPlayTime;
        public bool IsRunning => isRunning;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            StartTimer();
        }

        public void StartTimer()
        {
            startTime = Time.time;
            isRunning = true;
            currentPlayTime = 0f;
        }

        public void StopTimer()
        {
            if (isRunning)
            {
                currentPlayTime = Time.time - startTime;
                isRunning = false;
            }
        }

        public void PauseTimer()
        {
            if (isRunning)
            {
                currentPlayTime = Time.time - startTime;
                isRunning = false;
            }
        }

        public void ResumeTimer()
        {
            if (!isRunning)
            {
                startTime = Time.time - currentPlayTime;
                isRunning = true;
            }
        }

        private void Update()
        {
            if (isRunning)
            {
                currentPlayTime = Time.time - startTime;
            }
        }

        public void CompleteGame(string playerName = "Player")
        {
            StopTimer();
            LeaderboardManager.RecordCompletion(currentPlayTime, playerName);
        }

        public void CompleteGameAndReturnToMenu(string playerName = "Player")
        {
            CompleteGame(playerName);
            SceneManager.LoadScene("MainMenu");
        }

        public string GetFormattedTime()
        {
            return FormatTime(currentPlayTime);
        }

        public static string FormatTime(float seconds)
        {
            int minutes = (int)(seconds / 60);
            int secs = (int)(seconds % 60);
            int ms = (int)((seconds % 1) * 100);
            return $"{minutes:00}:{secs:00}.{ms:00}";
        }
    }
}
