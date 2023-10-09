using System.Collections.Generic;
using UnityEngine;

public class SpawnZoneManager : MonoBehaviour
{
    public GameObject objectToSpawn;
    public int spawnRate = 1; // Number of objects to spawn per second
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Collider[] spawnZones;

    private void Start()
    {
        // Get all colliders attached to this GameObject
        spawnZones = GetComponents<Collider>();

        InvokeRepeating(nameof(SpawnObject), 0, 1.0f / spawnRate);
    }

    private void SpawnObject()
    {
        // Randomly select one of the colliders to be the spawn zone
        Collider selectedZone = spawnZones[Random.Range(0, spawnZones.Length)];

        Vector3 spawnPosition;

        // Depending on collider type, get a random spawn position
        if (selectedZone is BoxCollider box)
        {
            spawnPosition = GetRandomPositionInsideBox(box);
        }
        // Extend with more conditions for other collider types if needed
        else
        {
            return; // If collider type is not handled, exit early
        }

        GameObject spawned = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        spawnedObjects.Add(spawned);
    }

    private Vector3 GetRandomPositionInsideBox(BoxCollider b)
    {
        // Generate a random local position inside the BoxCollider
        Vector3 randomLocalPos = new Vector3(
            Random.Range(-b.size.x * 0.5f, b.size.x * 0.5f),
            Random.Range(-b.size.y * 0.5f, b.size.y * 0.5f),
            Random.Range(-b.size.z * 0.5f, b.size.z * 0.5f)
        );

        // Return this position in world coordinates
        return b.transform.TransformPoint(randomLocalPos + b.center);
    }
}
