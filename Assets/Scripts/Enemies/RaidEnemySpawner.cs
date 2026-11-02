using UnityEngine;

public class RaidEnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs; // Drag your enemy prefabs here

    [Header("Spawn Settings")]
    public int minEnemies = 3;
    public int maxEnemies = 7;
    
    [Header("Spawn Distance Around Player")]
    public float minDistanceFromPlayer = 3f; // Don't spawn on top of player
    public float maxDistanceFromPlayer = 8f; // Keep close and inside view

    void Start()
    {
        SpawnEnemiesAroundPlayer();
    }

    void SpawnEnemiesAroundPlayer()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No enemy prefabs assigned to RaidEnemySpawner!");
            return;
        }

        // Find the player in the scene
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) playerObj = GameObject.Find("Player");

        if (playerObj == null)
        {
            Debug.LogWarning("Player not found by RaidEnemySpawner!");
            return;
        }

        Vector3 playerPos = playerObj.transform.position;
        int enemiesToSpawn = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // Pick a random enemy type
            GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            // Generate a random position in a circle around the player
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
            
            Vector3 spawnPos = playerPos + new Vector3(randomDirection.x * randomDistance, randomDirection.y * randomDistance, 0f);

            // Instantiate enemy
            Instantiate(randomEnemyPrefab, spawnPos, Quaternion.identity);
        }

        Debug.Log($"Spawned {enemiesToSpawn} randomized enemies around the player!");
    }
}