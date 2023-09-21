using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [HideInInspector]
    public Vector3 MaxSize;
    [HideInInspector]
    public float GrowthTime;
    public float ShrinkSpeed = 0.2f;
    public float WaitTillShrink = 5f;
    private Vector3 currentSize;
    private bool grow = false;
    private float elapsedTime = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
            return;

        GetComponent<Rigidbody>().isKinematic = true;
        currentSize = transform.localScale;
        grow = true;
    }

    private void Update()
    {
        if (!grow)
            return;

        float percentage = elapsedTime / GrowthTime;
        transform.localScale = Vector3.Lerp(currentSize, MaxSize, percentage);
        elapsedTime += Time.deltaTime;

        if(percentage >= 1)
        {
            GetComponent<Collider>().enabled = false;
            currentSize = transform.localScale;
            StartCoroutine(Shrink());
            grow = false;
        }
    }

    IEnumerator Shrink()
    {
        yield return new WaitForSeconds(WaitTillShrink); // wait a bit

        float delta = 0;
        while (delta < GrowthTime)
        {
            delta += Time.deltaTime * ShrinkSpeed;
            transform.localScale = Vector3.Lerp(currentSize, Vector3.zero, delta / GrowthTime);
            yield return null;
        }

        yield return null;
    }
}
