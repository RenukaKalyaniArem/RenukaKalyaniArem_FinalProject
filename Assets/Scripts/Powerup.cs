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
                player.GrantShield(10f);
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
    float duration = 1f;
    float fireInterval = 1f;
    float fovDegrees = 300f;
    float endTime = Time.time + duration;
    float cosThresh = Mathf.Cos(fovDegrees * 0.5f * Mathf.Deg2Rad);

    var sp = FindAnyObjectByType<SpawnManager>();

    while (Time.time < endTime)
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int spawnIdx = 0;

        foreach (var e in enemies)
        {
            Vector3 toEnemy = (e.transform.position - player.transform.position).normalized;
            float dot = Vector3.Dot(player.transform.forward, toEnemy);
            bool ahead = (dot > cosThresh) && (e.transform.position.x >= player.transform.position.x);

            if (ahead)
            {
                // Launch missile from current spawn point
                Transform point = player.missileSpawnPoints[spawnIdx % player.missileSpawnPoints.Length];
                Quaternion lookRot = Quaternion.LookRotation((e.transform.position - point.position).normalized);
                GameObject hmObj = Instantiate(sp.homingMissilePrefab, point.position, lookRot);
                var hm = hmObj.GetComponent<HomingMissile>();
                if (hm) hm.target = e.transform;

                if (sp.multiMissileSFX)
                    AudioAndParticle.Instance.PlayClipAt(sp.multiMissileSFX, player.transform.position);

                spawnIdx++; // Cycle through spawn points
            }
        }

        yield return new WaitForSeconds(fireInterval);
    }
}
}