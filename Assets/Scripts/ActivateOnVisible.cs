using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ActivateOnVisible : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        // Lấy component ParticleSystem trên cùng đối tượng
        ps = GetComponent<ParticleSystem>();
    }

    // Hàm này sẽ tự động được Unity gọi khi đối tượng lọt vào khung hình của bất kỳ camera nào
    void OnBecameVisible()
    {
        if (ps != null && !ps.isPlaying)
        {
            ps.Play();
        }
    }

    // Hàm này sẽ được gọi khi đối tượng ra khỏi khung hình của tất cả camera
    void OnBecameInvisible()
    {
        if (ps != null && ps.isPlaying)
        {
            ps.Stop();
        }
    }
} 