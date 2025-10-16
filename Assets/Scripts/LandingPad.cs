using UnityEngine;

public class LandingPad : MonoBehaviour
{
    public ParticleSystem landingEffect;
    public AudioClip landingSound;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (landingEffect) landingEffect.Play();

            AudioSource audio = GetComponent<AudioSource>();
            if (audio && landingSound)
                audio.PlayOneShot(landingSound);

            GameManager gm = FindAnyObjectByType<GameManager>();
            if (gm) gm.LevelComplete();
        }
    }
}
