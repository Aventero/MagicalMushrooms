using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsLimiter : MonoBehaviour
{
    public int FrameRate = -1;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = FrameRate;
    }
}
