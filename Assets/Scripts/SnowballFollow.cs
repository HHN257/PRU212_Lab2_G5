using UnityEngine;

public class SnowballFollow : MonoBehaviour
{
    public Transform player; // Kéo player vào đây
    public float followDistance = 2f; // Khoảng cách tối thiểu với player
    public float pushForce = 10f; // Lực đẩy snowball
    public float maxSlopeAngle = 30f; // Góc dốc tối đa snowball có thể leo
    public float killSpeed = 8f; // Ngưỡng vận tốc để gây chết player
    public float maxSpeed = 3f; // Tốc độ tối đa cho phép

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Kiểm tra khoảng cách với player
        float distance = player.position.x - transform.position.x;

        // Nếu player ở phía trước và snowball chưa bị kẹt
        if (Mathf.Abs(distance) > followDistance)
        {
            // Kiểm tra mặt đất phía trước snowball
            Vector2 origin = (Vector2)transform.position + Vector2.right * Mathf.Sign(distance) * 0.6f;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1.5f, LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                // Tính góc nghiêng mặt đất
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle < maxSlopeAngle)
                {
                    // Đẩy snowball về phía player
                    rb.AddForce(new Vector2(Mathf.Sign(distance) * pushForce, 0));
                }
                // Nếu dốc quá cao, không đẩy nữa (bị kẹt)
            }
        }

        // Hãm tốc độ tối đa
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Lấy vận tốc tuyệt đối của snowball tại thời điểm va chạm
            float speed = rb.linearVelocity.magnitude;
            if (speed > killSpeed)
            {
                // Gọi hàm game over cho player
                PlayerBalance pb = collision.gameObject.GetComponent<PlayerBalance>();
                if (pb != null)
                {
                    pb.TriggerGameOverByObstacle();
                }
            }
        }
    }
} 