using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{


    public GameObject[] enemyPrefabs; 
    public Vector2 enemyXRange = new Vector2(20f, 300f);
    public Vector2 enemyYRange = new Vector2(10f, 50f);
    public Vector2 enemyZRange = new Vector2(-20f, 20f);
    public float enemySpawnInterval = 0.5f;

    
    public GameObject[] powerupPrefabs; 
    public Vector2 powerupXRange = new Vector2(20f, 300f);
    public Vector2 powerupYRange = new Vector2(10f, 50f);
    public float powerupSpawnInterval = 2f;

    
    public GameObject homingMissilePrefab;
    public AudioClip multiMissileSFX;
     private bool gameActive = true; 

    void Start()
    {
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPowerups());
    }

    IEnumerator SpawnEnemies()
    {
        while (gameActive)
        {
            if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            {
                yield return new WaitForSeconds(enemySpawnInterval);
                continue;
            }

            int index = Random.Range(0, enemyPrefabs.Length);
            GameObject prefab = enemyPrefabs[index];

            Vector3 pos = new Vector3(
                Random.Range(enemyXRange.x, enemyXRange.y),
                Random.Range(enemyYRange.x, enemyYRange.y),
                Random.Range(enemyZRange.x, enemyZRange.y)
            );

            Instantiate(prefab, pos, Quaternion.identity);

            yield return new WaitForSeconds(enemySpawnInterval);
        }
    }

    IEnumerator SpawnPowerups()
    {
        while (gameActive)
        {
            if (powerupPrefabs == null || powerupPrefabs.Length == 0)
            {
                yield return new WaitForSeconds(powerupSpawnInterval);
                continue;
            }

            int index = Random.Range(0, powerupPrefabs.Length);
            GameObject prefab = powerupPrefabs[index];

            Vector3 pos = new Vector3(
                Random.Range(powerupXRange.x, powerupXRange.y),
                Random.Range(powerupYRange.x, powerupYRange.y),
                0f
            );

            Instantiate(prefab, pos, Quaternion.identity);

            yield return new WaitForSeconds(powerupSpawnInterval);
        }
    }
    public void StopSpawning()
    {
        gameActive = false;
        StopAllCoroutines();
    }
}

