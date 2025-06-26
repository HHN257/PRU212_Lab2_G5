using UnityEngine;

public class BatAttackTrigger : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerBalance player = other.GetComponent<PlayerBalance>();
            if (player != null && !player.hasDoubleJumped)
            {
                // Dơi tấn công
                if (animator != null)
                {
                    animator.SetTrigger("AttackTrigger");
                }
                // Gọi hàm chết cho player
                player.TriggerGameOverByObstacle(); // Đảm bảo PlayerBalance có hàm này
            }
        }
    }
} 