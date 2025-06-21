using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardManager : MonoBehaviour
{
    [Header("Top 5 Texts (Kéo 5 TextMeshProUGUI vào đây)")]
    public TextMeshProUGUI[] topTexts; // Kéo 5 Text vào đây theo thứ tự Top1, Top2, ...

    private const string RANKING_KEY = "GameRanking";

    [System.Serializable]
    public class ScoreEntry
    {
        public string playerName;
        public int score;
    }

    [System.Serializable]
    public class RankingList
    {
        public List<ScoreEntry> entries = new List<ScoreEntry>();
    }

    void OnEnable()
    {
        ShowTop5();
    }

    void Update()
    {
        if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }

    void ShowTop5()
    {
        RankingList ranking = LoadRanking();
        var top5 = ranking.entries.OrderByDescending(e => e.score).Take(5).ToList();

        for (int i = 0; i < topTexts.Length; i++)
        {
            if (i < top5.Count)
            {
                topTexts[i].text = $"{i + 1}. {top5[i].playerName} - {top5[i].score}";
            }
            else
            {
                topTexts[i].text = $"{i + 1}. ---";
            }
        }
    }

    RankingList LoadRanking()
    {
        if (PlayerPrefs.HasKey(RANKING_KEY))
        {
            string json = PlayerPrefs.GetString(RANKING_KEY);
            return JsonUtility.FromJson<RankingList>(json);
        }
        return new RankingList();
    }
} 