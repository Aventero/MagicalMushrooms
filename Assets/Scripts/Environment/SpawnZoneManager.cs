using System.Collections.Generic;
using UnityEngine;

public class SpawnZoneManager : MonoBehaviour
{
    public GameObject objectToSpawn;
    public int spawnRate = 1; // Number of objects to spawn per second
    public int maxObjects = 100;
    public float NoSpawnRange = 25f;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Collider[] spawnZones;
    private GameObject player;

    private void Start()
    {
        // Get all colliders attached to this GameObject
        spawnZones = GetComponents<Collider>();
        player = GameObject.FindGameObjectWithTag("Player");

        InvokeRepeating(nameof(SpawnObject), 0, 1.0f / spawnRate);
    }

    private void SpawnObject()
    {
        // If max object count is reached, exit early
        if (spawnedObjects.Count >= maxObjects || Vector3.Distance(transform.position, player.transform.position) > NoSpawnRange) 
            return;

        // Randomly select one of the colliders to be the spawn zone
        Collider selectedZone = spawnZones[Random.Range(0, spawnZones.Length)];
        Vector3 spawnPosition;

        // Depending on collider type, get a random spawn position
        if (selectedZone is BoxCollider box)
            spawnPosition = GetRandomPositionInsideBox(box);
        else
            return;

        GameObject spawned = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        spawnedObjects.Add(spawned);

        // Add the auto-destruct notifier to the spawned object
        spawned.AddComponent<AutoDestructNotifier>().manager = this;
    }

    private Vector3 GetRandomPositionInsideBox(BoxCollider b)
    {
        Vector3 randomLocalPos = new Vector3(
            Random.Range(-b.size.x * 0.5f, b.size.x * 0.5f),
            Random.Range(-b.size.y * 0.5f, b.size.y * 0.5f),
            Random.Range(-b.size.z * 0.5f, b.size.z * 0.5f)
        );

        return b.transform.TransformPoint(randomLocalPos + b.center);
    }

    public void NotifyDestroyed(GameObject obj)
    {
        spawnedObjects.Remove(obj);
    }
}