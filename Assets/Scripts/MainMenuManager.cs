using UnityEngine;
using UnityEngine.SceneManagement; // Quan trọng: cần thư viện này để quản lý scene
using System.Collections; // Cần thêm thư viện này cho Coroutine

public class MainMenuManager : MonoBehaviour
{
    // Tên scene game chính của bạn
    // Đảm bảo tên này khớp chính xác với tên scene game của bạn trong Project.
    public string gameSceneName = "Level1"; // Hoặc "SampleScene" nếu bạn chưa đổi tên

    // Kéo thả AudioSource của nút START GAME vào đây từ Inspector
    public AudioSource buttonClickSound; 

    public float sceneLoadDelay = 0.5f; // Thời gian chờ (tính bằng giây) trước khi tải scene game

    public string loadingSceneName = "Loading Screen"; // Tên scene màn hình tải mới

    // Hàm này sẽ được gọi khi nút "START GAME" được nhấp
    public void StartGame()
    {
        Debug.Log("Đang tải trò chơi...");
        if (buttonClickSound != null)
        {
            buttonClickSound.Play();
            // Bắt đầu một coroutine để chờ một khoảng thời gian cố định rồi mới tải scene
            StartCoroutine(LoadLoadingSceneAfterSound(sceneLoadDelay));
        }
        else
        {
            SceneManager.LoadScene(loadingSceneName); // Tải màn hình tải
        }
    }

    // Coroutine để tải game sau khi âm thanh phát xong
    private IEnumerator LoadLoadingSceneAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay); // Chờ đủ thời gian âm thanh phát
        SceneManager.LoadScene(loadingSceneName); // Tải màn hình tải
    }

    // Hàm này sẽ được gọi khi nút "INTRODUCTION" được nhấp
    public void ShowIntroduction()
    {
        Debug.Log("Hiển thị giới thiệu...");
        // Ở đây bạn có thể tải một scene giới thiệu, hoặc hiển thị một panel thông tin.
    }

    // Hàm này sẽ được gọi khi nút "LEADERBOARD" được nhấp
    public void ShowLeaderboard()
    {
        Debug.Log("Hiển thị bảng xếp hạng...");
        // Ở đây bạn có thể tải một scene bảng xếp hạng, hoặc hiển thị một panel bảng xếp hạng.
    }

    // Hàm này sẽ được gọi khi nút "QUIT GAME" được nhấp (tùy chọn)
    public void QuitGame()
    {
        Debug.Log("Đang thoát trò chơi...");
        Application.Quit(); // Chú ý: Hàm này chỉ hoạt động khi chạy bản build game, không phải trong Editor.
    }
} 