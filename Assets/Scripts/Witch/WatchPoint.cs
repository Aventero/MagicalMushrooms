using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WatchPoint : MonoBehaviour
{
    [Range(0.5f, 10f)]
    public float WatchTime = 2f;
}
