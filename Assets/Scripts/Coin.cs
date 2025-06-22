using UnityEngine;

/// <summary>
/// Attach this script to your coin GameObject.
/// The coin must have a Collider2D set as 'Is Trigger'.
/// </summary>
public class Coin : MonoBehaviour
{
    public int coinValue = 100; // Points to add per coin
    public AudioClip coinCollectSound; // Assign in Inspector
    [Range(0f, 5f)]
    public float coinCollectVolume = 2.0f; // Set in Inspector, 1.0 = normal, >1 = louder

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerBalance player = other.GetComponent<PlayerBalance>();
        if (player != null)
        {
            player.AddScore(coinValue);
            if (coinCollectSound != null)
            {
                // Play the sound at the camera position for consistent 2D volume
                AudioSource.PlayClipAtPoint(coinCollectSound, Camera.main.transform.position, coinCollectVolume);
            }
            Destroy(gameObject);
        }
    }
}
