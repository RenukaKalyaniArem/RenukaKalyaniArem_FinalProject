using UnityEngine;

public class AudioAndParticle : MonoBehaviour
{
    
    public static AudioAndParticle Instance;

    public AudioClip explosionClip;
    public ParticleSystem explosionPrefab;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayExplosionAt(Vector3 pos)
    {
        if (explosionPrefab)
        {
            var particleEffects = Instantiate(explosionPrefab, pos, Quaternion.identity);
            particleEffects.Play();
            var main = particleEffects.main;
            Destroy(particleEffects.gameObject, main.duration + main.startLifetime.constantMax);
        }
        if (explosionClip) PlayClipAt(explosionClip, pos);
    }

    public void PlayImpactAt(Vector3 pos)
    {
        if (explosionPrefab)
        {
            var particleEffects = Instantiate(explosionPrefab, pos, Quaternion.identity);
            particleEffects.Play();
            var main = particleEffects.main;
            Destroy(particleEffects.gameObject, main.duration + main.startLifetime.constantMax);
        }
    }

    public void PlayClipAt(AudioClip clip, Vector3 pos)
    {
        if (clip == null) return;
        GameObject go = new GameObject("OneShotAudio");
        go.transform.position = pos;
        AudioSource a = go.AddComponent<AudioSource>();
        a.PlayOneShot(clip);
        Destroy(go, clip.length + 0.1f);
    }
}
