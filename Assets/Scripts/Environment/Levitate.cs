using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Levitate : MonoBehaviour
{
    private Vector3 levitationPoint;
    public float MaxVelocity = 10f;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 startingPosition = transform.position;
        float randomMax = Random.value * 2.0f;
        levitationPoint = new Vector3(0, randomMax, 0) + startingPosition;
        rb.AddRelativeTorque(randomMax * 0.01f, randomMax * 0.01f, randomMax * 0.01f, ForceMode.Impulse);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 velocity = (levitationPoint - rb.transform.position) * 10f;
        velocity.x = Mathf.Clamp(velocity.x, -MaxVelocity, MaxVelocity);
        velocity.y = Mathf.Clamp(velocity.y, -MaxVelocity, MaxVelocity);
        velocity.z = Mathf.Clamp(velocity.z, -MaxVelocity, MaxVelocity);
        rb.velocity = velocity;
    }


}
