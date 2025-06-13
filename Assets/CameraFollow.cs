using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Kéo thả đối tượng nhân vật vào đây trong Inspector
    public Vector3 offset;   // Điều chỉnh vị trí offset của camera so với nhân vật

    // LateUpdate được gọi sau khi tất cả các tính toán Update đã hoàn tất.
    // Điều này đảm bảo camera di chuyển sau khi nhân vật đã di chuyển.
    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Target for CameraFollow is not set!");
            return;
        }

        // Đặt vị trí của camera bằng vị trí của nhân vật cộng với offset
        transform.position = target.position + offset;
    }
} 