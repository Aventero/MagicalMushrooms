using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float growthTime = 3.0f;
    public Vector3 finalScale = new Vector3(2, 2, 2);
    public float speed = 5.0f;

    private bool isCharging = true;
    private Vector3 initialScale;
    private float elapsedTime = 0.0f;
    private Transform target;

    private void Start()
    {
        initialScale = transform.localScale;
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (isCharging)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / growthTime;
            transform.localScale = Vector3.Lerp(initialScale, finalScale, t);

            if (elapsedTime >= growthTime)
                isCharging = false;
        }
        else
            MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }
}
