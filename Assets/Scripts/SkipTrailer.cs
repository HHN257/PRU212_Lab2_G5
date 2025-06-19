using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SkipTrailer : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Kéo VideoPlayer vào đây
    public string nextSceneName = "Level1"; // Đặt tên scene tiếp theo
    private bool isSkipped = false;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    void Update()
    {
        if (!isSkipped && Input.GetKeyDown(KeyCode.Space))
        {
            isSkipped = true;
            LoadNextScene();
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        if (!isSkipped)
        {
            isSkipped = true;
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
} 