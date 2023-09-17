using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeSpin : MonoBehaviour
{
    public float Speed = 5f;

    void Update()
    {
        transform.Rotate(0, Speed * Time.deltaTime, 0);
    }
}
