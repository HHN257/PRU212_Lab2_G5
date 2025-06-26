using UnityEngine;

public class MoveParticleSystem : MonoBehaviour
{
    [Header("Movement")]
    public float speed = -2f; // Tốc độ di chuyển ngang (âm để bay sang trái)

    [Header("Ground Following")]
    public LayerMask groundLayer; // Kéo Layer 'Ground' vào đây
    public float heightAboveGround = 1.5f; // Độ cao mong muốn so với mặt đất
    public float raycastCheckHeight = 100f; // Độ cao an toàn để bắt đầu dò tìm mặt đất
    public float raycastMaxDistance = 200f; // Khoảng cách tối đa tia dò sẽ đi

    void Update()
    {
        // 1. Di chuyển emitter theo chiều ngang
        transform.position += Vector3.right * speed * Time.deltaTime;

        // 2. Tạo một điểm bắt đầu cho tia Raycast ở trên cao, nhưng cùng tọa độ X
        Vector2 rayStartPoint = new Vector2(transform.position.x, raycastCheckHeight);
        
        // 3. Bắn một tia Raycast từ điểm trên cao thẳng xuống dưới
        RaycastHit2D hit = Physics2D.Raycast(rayStartPoint, Vector2.down, raycastMaxDistance, groundLayer);

        // 4. Nếu tia Raycast chạm vào một vật thể thuộc 'groundLayer'
        if (hit.collider != null)
        {
            // Cập nhật vị trí Y của emitter để nó luôn cao hơn mặt đất một khoảng 'heightAboveGround'
            float targetY = hit.point.y + heightAboveGround;
            transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        }
    }

    // (Tùy chọn) Vẽ một đường Gizmo màu đỏ trong cửa sổ Scene để bạn có thể thấy tia raycast đang hoạt động
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 startPoint = new Vector2(transform.position.x, raycastCheckHeight);
        Gizmos.DrawLine(startPoint, startPoint + Vector2.down * raycastMaxDistance);
    }
} 