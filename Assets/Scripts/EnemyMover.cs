using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public float speed = 8f;
    public float torque = 20f;
    public ParticleSystem deathParticleEffects;
    public AudioClip deathSoundEffects;


    Rigidbody rb;
    void Awake()
    {
    rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
    // move left along -X axis
    transform.position += Vector3.left * speed * Time.deltaTime;
    transform.Rotate(Vector3.up * torque * Time.deltaTime);
    if (transform.position.x < 1f) Destroy(gameObject);
    }



    public void DestroyByHit()
    {
        if (deathParticleEffects)
        {
            var particleEffects = Instantiate(deathParticleEffects, transform.position, Quaternion.identity);
            particleEffects.Play();
            var main = particleEffects.main;
            Destroy(particleEffects.gameObject, main.duration + main.startLifetime.constantMax);
        }
        else
        {
            AudioAndParticle.Instance.PlayExplosionAt(transform.position);
        }

        if (deathSoundEffects)
        {
            AudioAndParticle.Instance.PlayClipAt(deathSoundEffects, transform.position);
        }

        Destroy(gameObject);
    }

}
