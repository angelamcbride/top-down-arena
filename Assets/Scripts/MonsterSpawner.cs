using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Spawner Configuration")]
    public float mobsPerMinute = 10f;
    public int maxActiveMobs = 6; // Maximum number of active mobs in the scene

    [Header("Mob Prefabs")]
    [SerializeField]
    private List<GameObject> mobPrefabs = new List<GameObject>();

    private List<Transform> spawnPoints = new List<Transform>();
    private float spawnInterval;
    private Transform lastSpawnPoint;
    private static int activeMobs = 0; // Tracks the number of active mobs in the scene

    private void Start()
    {
        PopulateSpawnPoints();

        if (mobPrefabs == null || mobPrefabs.Count == 0)
        {
            Debug.LogError("No mob prefabs assigned to the MonsterSpawner.");
            return;
        }

        spawnInterval = 60f / mobsPerMinute;
        StartCoroutine(SpawnMonsters());
    }

    private void PopulateSpawnPoints()
    {
        spawnPoints.Clear();
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child);
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No child objects found to use as spawn points.");
        }
    }

    private IEnumerator SpawnMonsters()
    {
        while (true)
        {
            // Wait for the next spawn interval (randomized slightly for variation)
            float randomInterval = Random.Range(spawnInterval * 0.75f, spawnInterval * 1.25f);
            yield return new WaitForSeconds(randomInterval);

            if (activeMobs < maxActiveMobs)
            {
                SpawnRandomMonster();
            }
        }
    }

    private void SpawnRandomMonster()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points available for spawning.");
            return;
        }

        // Find a spawn point that is not the same as the last one used
        Transform spawnPoint;
        do
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        } while (spawnPoint == lastSpawnPoint && spawnPoints.Count > 1);

        lastSpawnPoint = spawnPoint;

        GameObject selectedMobPrefab = mobPrefabs[Random.Range(0, mobPrefabs.Count)];
        if (selectedMobPrefab != null)
        {
            GameObject mob = Instantiate(selectedMobPrefab, spawnPoint.position, Quaternion.identity);
            mob.GetComponentInChildren<BaseMobController>().OnMobSpawned(); // Notify the mob that it has been spawned
        }
        else
        {
            Debug.LogError("Selected mob prefab is null.");
        }
    }

    public static void AdjustActiveMobCount(int amount)
    {
        activeMobs += amount;
        activeMobs = Mathf.Max(0, activeMobs); // Ensure the count doesn't go negative
    }
}
