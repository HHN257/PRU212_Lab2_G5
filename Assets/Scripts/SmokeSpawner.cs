using UnityEngine;

public class SmokeSpawner : MonoBehaviour
{
    [Header("Setup")]
    public GameObject smokePrefab; // Kéo Prefab của đám khói vào đây
    public LayerMask groundLayer; // Chọn Layer 'Ground'

    [Header("Spawning Logic")]
    public float spawnAheadDistance = 20f; // Khoảng cách spawn phía trước camera
    public float minSpawnTime = 3f; // Thời gian tối thiểu giữa 2 lần spawn
    public float maxSpawnTime = 7f; // Thời gian tối đa giữa 2 lần spawn
    public float verticalOffset = 2f; // Độ cao của khói so với mặt đất spawn

    private float spawnTimer;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        ResetSpawnTimer();
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnSmoke();
            ResetSpawnTimer();
        }
    }

    void SpawnSmoke()
    {
        if (smokePrefab == null || mainCamera == null)
        {
            Debug.LogWarning("Smoke Prefab hoặc Main Camera chưa được thiết lập trong Spawner.");
            return;
        }

        // 1. Tính toán vị trí spawn X (phía trước camera)
        // Lấy vị trí cạnh phải của màn hình và cộng thêm khoảng cách
        float spawnX = mainCamera.transform.position.x + GetCameraHalfWidth() + spawnAheadDistance;

        // 2. Tìm vị trí Y của mặt đất tại điểm spawnX
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(spawnX, 100f), Vector2.down, 200f, groundLayer);

        if (hit.collider != null)
        {
            // 3. Nếu tìm thấy mặt đất, tạo đám khói ở đó
            float spawnY = hit.point.y + verticalOffset;
            Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);

            Instantiate(smokePrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy mặt đất tại vị trí x = {spawnX} để spawn khói.");
        }
    }

    void ResetSpawnTimer()
    {
        spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
    }

    float GetCameraHalfWidth()
    {
        // Tính toán nửa chiều rộng của camera trong world space
        return mainCamera.aspect * mainCamera.orthographicSize;
    }

    // (Tùy chọn) Vẽ một đường Gizmo màu đỏ trong cửa sổ Scene để bạn có thể thấy tia raycast đang hoạt động
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return; // Chỉ vẽ khi game đang chạy

        Gizmos.color = Color.red;

        // Tính toán vị trí X mà spawner đang kiểm tra
        float spawnX = mainCamera.transform.position.x + GetCameraHalfWidth() + spawnAheadDistance;
        Vector2 startPoint = new Vector2(spawnX, 100f);

        // Vẽ đường thẳng từ trên xuống
        Gizmos.DrawLine(startPoint, startPoint + Vector2.down * 200f);
    }
} 