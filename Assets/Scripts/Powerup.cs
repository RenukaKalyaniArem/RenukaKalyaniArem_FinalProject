using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PowerupType { MultiMissile, Shield, ExtraLife }
public class Powerup : MonoBehaviour
{
    public PowerupType type;
    public ParticleSystem collectParticles;
    public AudioClip collectSound;

    void Start()
    {
        Destroy(gameObject, 20f); // auto destroy after 20 seconds
    }

    public void Collect(PlayerController player)
    {
        if (collectParticles)
            {
                var particleEffects = Instantiate(collectParticles, player.transform.position, player.transform.rotation, player.transform);
                particleEffects.Play();
                var main = particleEffects.main;
                Destroy(particleEffects.gameObject, main.duration + main.startLifetime.constantMax);
            }
        if (collectSound) AudioAndParticle.Instance.PlayClipAt(collectSound, transform.position);

        switch (type)
        {
            case PowerupType.MultiMissile:
                StartCoroutine(GrantMultiMissile(player));
                break;
            case PowerupType.Shield:
                StartCoroutine(GrantShield(player));
                break;
            case PowerupType.ExtraLife:
                GameManager gm = FindAnyObjectByType<GameManager>();

                gm.ChangeLives(1);
                break;
        }

        Destroy(gameObject);
    }

    IEnumerator GrantMultiMissile(PlayerController player)
    {
        float duration = 10f;
        float fireInterval = 1f;
        float fovDegrees = 200f; 
        float endTime = Time.time + duration;
        float cosThresh = Mathf.Cos((fovDegrees * 0.5f) * Mathf.Deg2Rad);
        var sp = FindAnyObjectByType<SpawnManager>();

        while (Time.time < endTime)
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");

            var front = new List<Transform>();
            foreach (var e in enemies)
            {
                var toEnemy = (e.transform.position - player.transform.position).normalized;
                float dot = Vector3.Dot(player.transform.forward, toEnemy);
                bool ahead = dot > cosThresh && e.transform.position.x >= player.transform.position.x;
                if (ahead) front.Add(e.transform);
            }

            if (front.Count > 0)
            {

                front.Sort((a, b) =>
                    Vector3.Distance(player.transform.position, a.position)
                    .CompareTo(Vector3.Distance(player.transform.position, b.position)));

                for (int i = 0; i < player.missileSpawnPoints.Length; i++)
                {
                    Transform point = player.missileSpawnPoints[i];
                    Transform tgt = front[i % front.Count];

                    Quaternion lookRot = Quaternion.LookRotation((tgt.position - point.position).normalized);
                    GameObject hmObj = Instantiate(sp.homingMissilePrefab, point.position, lookRot);
                    var hm = hmObj.GetComponent<HomingMissile>();
                    if (hm) hm.target = tgt;
                }

                if (sp.multiMissileSFX)
                    AudioAndParticle.Instance.PlayClipAt(sp.multiMissileSFX, player.transform.position);
            }

            yield return new WaitForSeconds(fireInterval);
        }
    }


    IEnumerator GrantShield(PlayerController player)
    {
        float duration = 10f;
        player.ActivateShield(true);
        yield return new WaitForSeconds(duration);
        player.ActivateShield(false);
    }
}
    
