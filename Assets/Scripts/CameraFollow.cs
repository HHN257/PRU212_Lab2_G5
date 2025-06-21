using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Kéo thả nhân vật vào đây trong Inspector
    public float smoothSpeed = 0.125f; // Tốc độ di chuyển mượt của camera
    public Vector3 offset; // Khoảng cách giữa camera và nhân vật

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Nếu bạn muốn camera nhìn vào nhân vật
        // transform.LookAt(target);
    }
} 