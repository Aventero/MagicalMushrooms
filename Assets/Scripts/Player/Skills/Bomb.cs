using UnityEngine;

public class Bomb : MonoBehaviour
{
    [HideInInspector]
    public Vector3 maxSize;
    [HideInInspector]
    public float growthTime;

    private Vector3 currentSize;
    private bool grow = false;
    private float elapsedTime = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
            return;

        this.GetComponent<Rigidbody>().isKinematic = true;
        currentSize = this.transform.localScale;
        grow = true;
    }

    private void Update()
    {
        if (!grow)
            return;

        float percentage = elapsedTime / growthTime;
        this.transform.localScale = Vector3.Lerp(currentSize, maxSize, percentage);
        elapsedTime += Time.deltaTime;

        if(percentage >= 1) 
            grow = false;
    }
}
