using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeSpin : MonoBehaviour
{
    public float Speed = 15f;

    void Update()
    {
        transform.Rotate(0, Speed * Time.deltaTime, 0);
    }
}
