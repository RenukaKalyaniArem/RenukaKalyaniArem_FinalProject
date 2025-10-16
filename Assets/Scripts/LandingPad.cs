using UnityEngine;

public class LandingPad : MonoBehaviour
{
    public ParticleSystem landingEffect;
    public AudioClip landingSound;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (landingEffect) landingEffect.Play();
        var audio = GetComponent<AudioSource>();
        if (audio && landingSound) audio.PlayOneShot(landingSound);

        var gm = GameManager.Instance != null ? GameManager.Instance : FindAnyObjectByType<GameManager>();
        if (gm) gm.OnShipLanded();
    }
}
