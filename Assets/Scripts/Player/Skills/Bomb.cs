using System.Collections;
using Unity.VisualScripting;
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

        CollisionUtil.Side collisionSide = CollisionUtil.CalculateCollisionSideSphere(collision);
        
        if (collisionSide == CollisionUtil.Side.Bottom)
        {
            // Reached the destination -> Let the ball explode
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().isTrigger = true;

            currentSize = transform.localScale;
            transform.localRotation = Quaternion.identity;
            transformingSpeed = GrowthSpeed;
            isTransforming = true;
        } else
        {
            // Any other side 
            // Stop the movement -> let the ball fall downwards
            Rigidbody rb = GetComponent<Rigidbody>();
            float verticalVelocity = rb.velocity.y;
            rb.velocity = new Vector3(0, verticalVelocity, 0);
        }
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
            if (transform.localScale.x <= 0.01f)
                Destroy(gameObject);

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
