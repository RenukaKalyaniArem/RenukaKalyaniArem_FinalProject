using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Projectile : MonoBehaviour
{
    public float speed = 60f;
    public float lifeTime = 5f;
    public AudioClip audioHit;
    public ParticleSystem hitParticleEffects;

    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }
    void OnEnable()
    {
        if (rb != null) rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (transform.position.x > 350f) Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyMover e = other.GetComponent<EnemyMover>();
            if (e) e.DestroyByHit();
            Destroy(gameObject);
        }
    }
}