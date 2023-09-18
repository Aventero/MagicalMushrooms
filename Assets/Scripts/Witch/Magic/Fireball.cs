using TMPro;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float growthTime = 3.0f;
    public Vector3 finalScale = new Vector3(2, 2, 2);
    public float speed = 5.0f;
    public float MaxVelocity = 10f;

    private bool isCharging = true;
    private Vector3 initialScale;
    private float elapsedTime = 0.0f;
    private Vector3 target;
    private Rigidbody rb;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        if (isCharging)
        {
            elapsedTime += Time.fixedDeltaTime;
            float t = elapsedTime / growthTime;
            t = Mathf.SmoothStep(0, 1, t);
            transform.localScale = Vector3.Lerp(initialScale, finalScale, t);

            if (elapsedTime >= growthTime)
            {
                target = GameObject.FindGameObjectWithTag("Player").transform.position;
                isCharging = false;
            }
        }
        else
            MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        Vector3 velocity = (target - transform.position) * speed * Time.fixedDeltaTime;
        velocity.x = Mathf.Clamp(velocity.x, -MaxVelocity, MaxVelocity);
        velocity.y = Mathf.Clamp(velocity.y, -MaxVelocity, MaxVelocity);
        velocity.z = Mathf.Clamp(velocity.z, -MaxVelocity, MaxVelocity);
        rb.velocity = velocity;
    }
}
