using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Vector3 GoalSize = new(5, 5, 5);
    public float GrowthSpeed = 1;
    public float ShrinkSpeed = 4;
    public float WaitTillShrink = 5f;

    private Vector3 currentSize;
    private bool isTransforming = false;
    private float transformingSpeed;
    private float elapsedTime = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
            return;

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;

        currentSize = transform.localScale;
        transform.localRotation = Quaternion.identity;
        transformingSpeed = GrowthSpeed;
        isTransforming = true;
    }

    private void Update()
    {
        if (!isTransforming)
            return;

        float percentage = elapsedTime / transformingSpeed;
        transform.localScale = Vector3.Lerp(currentSize, GoalSize, percentage);
        elapsedTime += Time.deltaTime;

        if(percentage >= 1)
        {
            if (Equals(transform.localScale, Vector3.zero))
                Destroy(this);

            // Shrink the bomb
            SetUpShrinkVariables();
            StartCoroutine(WaitUntilShrink());
        }
    }

    private void SetUpShrinkVariables()
    {
        isTransforming = false;
        elapsedTime = 0;

        currentSize = transform.localScale;
        transformingSpeed = ShrinkSpeed;
        GoalSize = Vector3.zero;
    }

    private IEnumerator WaitUntilShrink()
    {
        yield return new WaitForSeconds(WaitTillShrink); // wait a bit
        isTransforming = true;
    }
}
