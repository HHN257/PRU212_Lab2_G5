using UnityEngine;

/// <summary>
/// Attach this script to any obstacle GameObject.
/// The obstacle must have a Collider2D (set to isTrigger or not, both work).
/// </summary>
public class Obstacle : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerBalance player = collision.gameObject.GetComponent<PlayerBalance>();
        if (player != null)
        {
            // Only trigger game over if the player is not landing on top
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < 0.9f)
                {
                    player.TriggerGameOverByObstacle();
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerBalance player = other.GetComponent<PlayerBalance>();
        if (player != null)
        {
            // Try to get the relative position to determine if the player is above
            float playerBottom = player.transform.position.y - player.GetComponent<Collider2D>().bounds.extents.y;
            float obstacleTop = this.transform.position.y + this.GetComponent<Collider2D>().bounds.extents.y;
            // If player's bottom is above obstacle's top, don't trigger game over
            if (playerBottom < obstacleTop - 0.05f) // 0.05f is a small tolerance
            {
                player.TriggerGameOverByObstacle();
            }
        }
    }
}
