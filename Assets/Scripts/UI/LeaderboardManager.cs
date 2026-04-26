using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace LULUKA
{
    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public float completionTime;
        public string date;

        public LeaderboardEntry(string name, float time, string dateStr)
        {
            playerName = name;
            completionTime = time;
            date = dateStr;
        }
    }

    public class LeaderboardManager : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private Transform contentContainer;
        [SerializeField] private GameObject entryPrefab;
        [SerializeField] private int maxEntries = 20;

        private const string PREF_LEADERBOARD = "Leaderboard_Data";
        private List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

        public static LeaderboardManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            LoadLeaderboard();
        }

        public void RefreshLeaderboard()
        {
            LoadLeaderboard();
            DisplayLeaderboard();
        }

        public void AddEntry(string playerName, float completionTime)
        {
            string date = System.DateTime.Now.ToString("yyyy-MM-dd");
            var entry = new LeaderboardEntry(playerName, completionTime, date);
            entries.Add(entry);
            entries = entries.OrderBy(e => e.completionTime).Take(maxEntries).ToList();
            SaveLeaderboard();
        }

        public static void RecordCompletion(float completionTime, string playerName = "Player")
        {
            string date = System.DateTime.Now.ToString("yyyy-MM-dd");
            string data = PlayerPrefs.GetString(PREF_LEADERBOARD, "");
            List<LeaderboardEntry> tempEntries = new List<LeaderboardEntry>();

            if (!string.IsNullOrEmpty(data))
            {
                string[] entryStrings = data.Split('|');
                foreach (string entryStr in entryStrings)
                {
                    if (!string.IsNullOrEmpty(entryStr))
                    {
                        string[] parts = entryStr.Split(',');
                        if (parts.Length == 3)
                        {
                            float time;
                            if (float.TryParse(parts[1], out time))
                            {
                                tempEntries.Add(new LeaderboardEntry(parts[0], time, parts[2]));
                            }
                        }
                    }
                }
            }

            tempEntries.Add(new LeaderboardEntry(playerName, completionTime, date));
            tempEntries = tempEntries.OrderBy(e => e.completionTime).Take(20).ToList();

            List<string> newEntryStrings = new List<string>();
            foreach (var e in tempEntries)
            {
                newEntryStrings.Add($"{e.playerName},{e.completionTime},{e.date}");
            }
            PlayerPrefs.SetString(PREF_LEADERBOARD, string.Join("|", newEntryStrings));
            PlayerPrefs.Save();
        }

        private void LoadLeaderboard()
        {
            entries.Clear();
            string data = PlayerPrefs.GetString(PREF_LEADERBOARD, "");

            if (!string.IsNullOrEmpty(data))
            {
                string[] entryStrings = data.Split('|');
                foreach (string entryStr in entryStrings)
                {
                    if (!string.IsNullOrEmpty(entryStr))
                    {
                        string[] parts = entryStr.Split(',');
                        if (parts.Length == 3)
                        {
                            float time;
                            if (float.TryParse(parts[1], out time))
                            {
                                entries.Add(new LeaderboardEntry(parts[0], time, parts[2]));
                            }
                        }
                    }
                }
            }

            entries = entries.OrderBy(e => e.completionTime).ToList();
        }

        private void SaveLeaderboard()
        {
            List<string> entryStrings = new List<string>();
            foreach (var entry in entries)
            {
                entryStrings.Add($"{entry.playerName},{entry.completionTime},{entry.date}");
            }
            PlayerPrefs.SetString(PREF_LEADERBOARD, string.Join("|", entryStrings));
            PlayerPrefs.Save();
        }

        private void DisplayLeaderboard()
        {
            if (contentContainer == null) return;

            List<Transform> childrenToDestroy = new List<Transform>();
            foreach (Transform child in contentContainer)
            {
                if (child.name != "Text (Legacy)")
                {
                    childrenToDestroy.Add(child);
                }
            }
            foreach (Transform child in childrenToDestroy)
            {
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            if (entries.Count == 0)
            {
                if (entryPrefab != null)
                {
                    GameObject emptyEntry = Instantiate(entryPrefab, contentContainer);
                    var texts = emptyEntry.GetComponentsInChildren<Text>();
                    if (texts.Length >= 3)
                    {
                        texts[0].text = "-";
                        texts[1].text = "暂无记录";
                        texts[2].text = "--:--.--";
                    }
                    emptyEntry.name = "EmptyEntry";
                }
                return;
            }

            for (int i = 0; i < entries.Count; i++)
            {
                GameObject entryObj;
                if (entryPrefab != null)
                {
                    entryObj = Instantiate(entryPrefab, contentContainer);
                    entryObj.name = $"Entry_{i}";
                }
                else
                {
                    entryObj = CreateDefaultEntry(i);
                }

                var rankText = entryObj.transform.Find("Rank");
                var nameText = entryObj.transform.Find("Name");
                var timeText = entryObj.transform.Find("Time");

                if (rankText != null)
                {
                    var txt = rankText.GetComponent<Text>();
                    if (txt != null) txt.text = $"{i + 1}";
                }

                if (nameText != null)
                {
                    var txt = nameText.GetComponent<Text>();
                    if (txt != null) txt.text = entries[i].playerName;
                }

                if (timeText != null)
                {
                    var txt = timeText.GetComponent<Text>();
                    if (txt != null) txt.text = FormatTime(entries[i].completionTime);
                }
            }
        }

        private GameObject CreateDefaultEntry(int index)
        {
            GameObject entryObj = new GameObject($"Entry_{index}");
            entryObj.transform.SetParent(contentContainer, false);

            var rect = entryObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(350, 35);

            var layout = entryObj.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            CreateEntryText(entryObj.transform, "Rank", 40, $"{index + 1}");
            CreateEntryText(entryObj.transform, "Name", 150, entries[index].playerName);
            CreateEntryText(entryObj.transform, "Time", 100, FormatTime(entries[index].completionTime));

            return entryObj;
        }

        private void CreateEntryText(Transform parent, string name, float width, string content)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            var rect = textObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, 30);
            var text = textObj.AddComponent<Text>();
            text.text = content;
            text.fontSize = 16;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
        }

        private string FormatTime(float seconds)
        {
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int secs = Mathf.FloorToInt(seconds % 60f);
            int ms = Mathf.FloorToInt((seconds * 100) % 100);
            return $"{minutes:00}:{secs:00}.{ms:00}";
        }

        public static int GetRank(float completionTime)
        {
            string data = PlayerPrefs.GetString(PREF_LEADERBOARD, "");
            List<LeaderboardEntry> tempEntries = new List<LeaderboardEntry>();

            if (!string.IsNullOrEmpty(data))
            {
                string[] entryStrings = data.Split('|');
                foreach (string entryStr in entryStrings)
                {
                    if (!string.IsNullOrEmpty(entryStr))
                    {
                        string[] parts = entryStr.Split(',');
                        if (parts.Length == 3)
                        {
                            float time;
                            if (float.TryParse(parts[1], out time))
                            {
                                tempEntries.Add(new LeaderboardEntry(parts[0], time, parts[2]));
                            }
                        }
                    }
                }
            }

            tempEntries = tempEntries.OrderBy(e => e.completionTime).ToList();

            for (int i = 0; i < tempEntries.Count; i++)
            {
                if (completionTime <= tempEntries[i].completionTime)
                {
                    return i + 1;
                }
            }

            return tempEntries.Count + 1;
        }

        public void ClearLeaderboard()
        {
            entries.Clear();
            PlayerPrefs.DeleteKey(PREF_LEADERBOARD);
            PlayerPrefs.Save();
            DisplayLeaderboard();
        }
    }
}