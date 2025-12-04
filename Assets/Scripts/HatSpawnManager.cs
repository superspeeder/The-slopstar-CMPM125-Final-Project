using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class HatSpawnManager : MonoBehaviour
{
    [Header("Possible Hat Prefabs")]
    public GameObject[] hatPrefabs;

    [Header("Spawn Points Around Map")]
    public Transform[] spawnPoints;

    [Header("Spawn Settings")]
    public int hatsToSpawn = 5;

    void Start()
    {
        SpawnRandomHats();
    }

    void SpawnRandomHats()
    {
        // Shuffle the spawn points so we choose unique ones
        List<Transform> shuffledPoints = new List<Transform>(spawnPoints);
        shuffledPoints = shuffledPoints.OrderBy(x => Random.value).ToList();

        int count = Mathf.Min(hatsToSpawn, shuffledPoints.Count);

        for (int i = 0; i < count; i++)
        {
            GameObject hat = hatPrefabs[Random.Range(0, hatPrefabs.Length)];
            Transform point = shuffledPoints[i];

            Instantiate(hat, point.position, point.rotation);
        }
    }
}
