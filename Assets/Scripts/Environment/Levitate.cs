using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Levitate : MonoBehaviour
{
    public Vector3 levitationPoint;
    public float MaxVelocity = 10f;
    public float LevitationHeight = 0;
    public bool isLevitating = false;
    Rigidbody rb;

    public void Initialize(float height)
    {
        float randomMax = Random.value * 2.0f;
        rb = GetComponent<Rigidbody>();
        levitationPoint = new Vector3(0, height, 0) + transform.position;
        rb.AddRelativeTorque(Random.value * 0.01f, Random.value * 0.01f, Random.value * 0.01f, ForceMode.Impulse);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isLevitating)
        {
            Vector3 velocity = (levitationPoint - rb.transform.position) * 10f;
            velocity.x = Mathf.Clamp(velocity.x, -MaxVelocity, MaxVelocity);
            velocity.y = Mathf.Clamp(velocity.y, -MaxVelocity, MaxVelocity);
            velocity.z = Mathf.Clamp(velocity.z, -MaxVelocity, MaxVelocity);
            rb.velocity = velocity;
        }
    }
}
