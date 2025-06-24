using UnityEngine;
using TMPro;

public class ExtraModeTrigger : MonoBehaviour
{
    public GameObject extraModeText; // Kéo UI Text vào đây trong Inspector
    public AudioSource extraModeAudio; // Kéo AudioSource vào đây trong Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (extraModeText != null)
            {
                extraModeText.SetActive(true);
                // Nếu muốn tự động ẩn sau 2 giây:
                StartCoroutine(HideTextAfterSeconds(2f));
            }
            if (extraModeAudio != null)
            {
                extraModeAudio.Play();
            }
        }
    }

    private System.Collections.IEnumerator HideTextAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        extraModeText.SetActive(false);
    }
} 