using UnityEngine;

public class SmokeHazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng va chạm có phải là người chơi không
        if (other.CompareTag("Player"))
        {
            // Lấy component PlayerBalance từ người chơi
            PlayerBalance player = other.GetComponent<PlayerBalance>();

            // Nếu tìm thấy component và người chơi *chưa* thực hiện double jump
            // (Sử dụng biến hasDoubleJumped mà chúng ta vừa đổi thành public)
            if (player != null && !player.hasDoubleJumped)
            {
                // Gọi hàm làm mờ màn hình từ script ScreenFade
                if (ScreenFade.instance != null)
                {
                    ScreenFade.instance.StartFadeToBlack();
                }
            }
        }
    }
} 