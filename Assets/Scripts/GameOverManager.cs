using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText; // Kéo ScoreText từ Hierarchy vào
    public TMP_InputField nameInputField; // Kéo NameInputField vào
    public TextMeshProUGUI rankingText; // Kéo RankingText vào
    public Button replayButton; // Kéo ReplayButton vào
    public Button backToMenuButton; // Kéo BackToMenuButton vào
    public AudioSource buttonClickSound; // Kéo AudioSource vào đây trong Inspector

    [Header("Scene Names")]
    public string gameSceneName = "Level1"; // Tên scene game chính
    public string mainMenuSceneName = "MainMenu"; // Tên scene menu chính

    private int finalScore;
    private const string RANKING_KEY = "GameRanking";

    void Awake()
    {
        // Tắt màn hình Game Over khi khởi đầu
        gameObject.SetActive(false);

        // Gán sự kiện cho các nút
        replayButton.onClick.AddListener(OnReplayButtonClick);
        backToMenuButton.onClick.AddListener(OnBackToMenuButtonClick);
        // Gán sự kiện khi người dùng nhấn Enter sau khi nhập tên
        nameInputField.onEndEdit.AddListener(OnNameInputEndEdit);

        // Ban đầu, hiển thị tiêu đề chung cho bảng xếp hạng
        UpdateRankingDisplayInitial();
    }

    // Hàm này được gọi từ script PlayerBalance khi Game Over
    public void ShowGameOver(int score)
    {
        gameObject.SetActive(true); // Bật màn hình Game Over
        Time.timeScale = 0f; // Dừng game
        finalScore = score;
        scoreText.text = "Your Score Is: " + finalScore.ToString();
        nameInputField.text = ""; // Xóa tên cũ
        nameInputField.interactable = true; // Cho phép nhập lại
        nameInputField.ActivateInputField(); // Tự động chọn ô nhập tên

        // Vô hiệu hóa nút Replay và Back To Menu cho đến khi nhập tên
        replayButton.interactable = false;
        backToMenuButton.interactable = false;

        UpdateRankingDisplayInitial(); // Reset hiển thị ranking về trạng thái ban đầu
    }

    void OnReplayButtonClick()
    {
        if (buttonClickSound != null)
        {
            buttonClickSound.Play();
            StartCoroutine(LoadSceneAfterSound(gameSceneName, 2f));
        }
        else
        {
            Time.timeScale = 1f;
            EventSystem.current.SetSelectedGameObject(null);
            SceneManager.LoadScene(gameSceneName);
        }
    }

    void OnBackToMenuButtonClick()
    {
        if (buttonClickSound != null)
        {
            buttonClickSound.Play();
            StartCoroutine(LoadSceneAfterSound(mainMenuSceneName, 2f));
        }
        else
        {
            Time.timeScale = 1f;
            EventSystem.current.SetSelectedGameObject(null);
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    private IEnumerator LoadSceneAfterSound(string sceneName, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        EventSystem.current.SetSelectedGameObject(null);
        SceneManager.LoadScene(sceneName);
    }

    void OnNameInputEndEdit(string playerName)
    {
        if (!string.IsNullOrWhiteSpace(playerName) && finalScore > 0)
        {
            SaveScore(playerName, finalScore);
            DisplayCurrentPlayerRank(playerName, finalScore);
            nameInputField.DeactivateInputField();
            EventSystem.current.SetSelectedGameObject(null);
            nameInputField.interactable = false;
            replayButton.interactable = true;
            backToMenuButton.interactable = true;
        }
    }

    [System.Serializable]
    public class ScoreEntry
    {
        public string playerName;
        public int score;

        public ScoreEntry(string name, int s)
        {
            playerName = name;
            score = s;
        }
    }

    [System.Serializable]
    public class RankingList
    {
        public List<ScoreEntry> entries = new List<ScoreEntry>();
    }

    void SaveScore(string playerName, int score)
    {
        RankingList ranking = LoadRanking();
        ranking.entries.Add(new ScoreEntry(playerName, score));
        // Sắp xếp giảm dần theo điểm
        ranking.entries = ranking.entries.OrderByDescending(e => e.score).ToList();
        // Giới hạn số lượng entries (ví dụ: 10 người đứng đầu)
        if (ranking.entries.Count > 10) ranking.entries.RemoveAt(ranking.entries.Count - 1);

        string json = JsonUtility.ToJson(ranking);
        PlayerPrefs.SetString(RANKING_KEY, json);
        PlayerPrefs.Save();
        Debug.Log($"Score saved: {playerName} - {score}");
    }

    RankingList LoadRanking()
    {
        if (PlayerPrefs.HasKey(RANKING_KEY))
        {
            string json = PlayerPrefs.GetString(RANKING_KEY);
            return JsonUtility.FromJson<RankingList>(json);
        }
        return new RankingList(); // Trả về danh sách rỗng nếu chưa có
    }

    // Hiển thị tiêu đề chung cho bảng xếp hạng (ban đầu)
    void UpdateRankingDisplayInitial()
    {
        rankingText.text = "YOUR RANKING";
    }

    // Hiển thị hạng của người chơi hiện tại
    void DisplayCurrentPlayerRank(string playerName, int score)
    {
        RankingList ranking = LoadRanking();
        int playerRank = -1;
        var sorted = ranking.entries.OrderByDescending(e => e.score).ToList();
        int topCount = Mathf.Min(5, sorted.Count);

        // Tìm hạng của người chơi vừa nộp điểm
        for (int i = 0; i < sorted.Count; i++)
        {
            if (sorted[i].playerName == playerName && sorted[i].score == score)
            {
                playerRank = i + 1;
                break;
            }
        }

        if (playerRank != -1 && playerRank <= 5)
        {
            rankingText.text = $"YOUR RANKING: #{playerRank}";
        }
        else
        {
            // Nếu không nằm trong top 5, tính số điểm cần để vào top 5
            int pointsToTop5 = 0;
            if (sorted.Count >= 5)
            {
                int fifthScore = sorted[4].score;
                pointsToTop5 = fifthScore - score + 1;
                if (pointsToTop5 < 1) pointsToTop5 = 1;
                rankingText.text = $"You need {pointsToTop5} more points to reach Top 5!";
            }
            else
            {
                // Nếu chưa đủ 5 người chơi, vẫn hiển thị thứ hạng
                if (playerRank != -1)
                    rankingText.text = $"YOUR RANKING: #{playerRank}";
                else
                    rankingText.text = "YOUR RANKING: Not Found.";
            }
        }
    }

    // Hàm Reset Ranking (Chỉ dùng cho debug nếu cần)
    public void ResetRanking()
    {
        PlayerPrefs.DeleteKey(RANKING_KEY);
        PlayerPrefs.Save();
        UpdateRankingDisplayInitial(); // Trở về hiển thị ban đầu
        Debug.Log("Ranking Reset!");
    }
} 