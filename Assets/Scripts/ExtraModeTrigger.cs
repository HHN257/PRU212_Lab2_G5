using UnityEngine;
using TMPro;

public class ExtraModeTrigger : MonoBehaviour
{
    public GameObject extraModeText; // Kéo UI Text vào đây trong Inspector
    public AudioSource extraModeAudio; // Kéo AudioSource vào đây trong Inspector
    public ParticleSystem crashEffect;
    public ParticleSystem dustParticles;

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

            // Đổi màu sau 5 giây
            StartCoroutine(ChangeParticlesColorAfterDelay(5f));
        }
    }

    private System.Collections.IEnumerator ChangeParticlesColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Color lavaColor = new Color(0.5f, 0f, 0.125f); // Đỏ đô
        ChangeParticleColor(crashEffect, lavaColor);
        ChangeParticleColor(dustParticles, lavaColor);
    }

    private void ChangeParticleColor(ParticleSystem ps, Color color)
    {
        if (ps != null)
        {
            var main = ps.main;
            main.startColor = color;
        }
    }

    private System.Collections.IEnumerator HideTextAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        extraModeText.SetActive(false);
    }
} 