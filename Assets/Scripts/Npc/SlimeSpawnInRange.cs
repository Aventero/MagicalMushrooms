using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlimeSpawnInRange : MonoBehaviour
{
    private Transform player;
    private GameObject[] slimes;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        slimes = GameObject.FindGameObjectsWithTag("Slime");
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < slimes.Length; i++)
        {
            if (Vector3.Distance(player.transform.position, slimes[i].transform.position) < 15f)
            {
                slimes[i].SetActive(true);
            }
            else
            {
                slimes[i].SetActive(false);
            }
        }
    }
}
