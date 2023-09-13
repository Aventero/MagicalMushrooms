using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
    public float Radius;
    public float Speed = 2.0f;
    public AnimationCurve levitationCurveY;

    public Vector3 WobbleAtTime(Vector3 position, float time)
    {
        float angle = time * 360f * Speed;

        float x = position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * Radius;
        float z = position.z + Mathf.Sin(angle * Mathf.Deg2Rad) * Radius;
        return new Vector3(x, position.y + levitationCurveY.Evaluate(time), z);
    }
}