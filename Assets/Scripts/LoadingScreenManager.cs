using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // Nếu bạn có TextMeshProUGUI để hiển thị tiến trình
using UnityEngine.UI; // Nếu bạn có Slider để hiển thị tiến trình

public class LoadingScreenManager : MonoBehaviour
{
    // Tên scene game chính mà chúng ta sẽ tải
    public string gameSceneToLoad = "Level1";

    // Các tham chiếu UI (kéo thả từ Inspector)
    public TextMeshProUGUI loadingText; // Kéo thả TextMeshProUGUI hiển thị "Loading..." hoặc "%"
    public Slider loadingSlider; // Kéo thả Slider để hiển thị tiến trình

    void Start()
    {
        // Bắt đầu quá trình tải scene bất đồng bộ
        StartCoroutine(LoadGameSceneAsync());
    }

    IEnumerator LoadGameSceneAsync()
    {
        // Tạo một thao tác tải bất đồng bộ
        AsyncOperation operation = SceneManager.LoadSceneAsync(gameSceneToLoad);

        // Ngăn không cho scene mới kích hoạt ngay lập tức
        // Điều này cho phép bạn giữ màn hình tải hiển thị cho đến khi bạn sẵn sàng
        operation.allowSceneActivation = false;

        // Vòng lặp cho đến khi scene game gần như được tải hoàn chỉnh
        while (!operation.isDone)
        {
            // Tính toán tiến trình tải
            // progress sẽ từ 0.0 đến 0.9. Sau đó nó sẽ dừng ở 0.9 cho đến khi allowSceneActivation = true
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // Cập nhật UI (nếu có)
            if (loadingText != null) loadingText.text = "Loading... " + (progress * 100f).ToString("F0") + "%";
            if (loadingSlider != null) loadingSlider.value = progress;

            // Khi tiến trình đạt 0.9 (tức là scene đã tải gần xong, sẵn sàng kích hoạt)
            if (operation.progress >= 0.9f)
            {
                // Nếu bạn muốn đợi một chút hoặc thực hiện animation trước khi chuyển scene
                // yield return new WaitForSeconds(1f); // Ví dụ: chờ thêm 1 giây

                // Kích hoạt scene mới
                operation.allowSceneActivation = true;
            }
            yield return null; // Đợi một frame trước khi kiểm tra lại
        }
    }
} 