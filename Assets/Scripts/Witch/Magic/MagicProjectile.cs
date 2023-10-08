using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public bool CollidedWithPlayer = false;
    public float pushBackDistance = 0.2f;
    private void Start()
    {
        CollidedWithPlayer = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            CollidedWithPlayer = true;
        else
            CollidedWithPlayer = false;
    }
}
