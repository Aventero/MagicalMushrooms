using System.Collections;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Vector3 GoalSize = new(5, 5, 5);
    public float GrowthSpeed = 1;
    public float ShrinkSpeed = 4;
    public float WaitTillShrink = 5f;
    private float percentCompleted = 0;

    private Vector3 currentSize;
    private bool isTransforming = false;
    private float transformingSpeed;
    private float elapsedTime = 0;
    public Renderer smokeRendererOuter;
    public Renderer smokeRendererBottom;
    private MaterialPropertyBlock propBlock;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
            return;

        CollisionUtil.Side collisionSide = CollisionUtil.CalculateCollisionSideSphere(collision);
        
        // Start Growing 
        if (collisionSide == CollisionUtil.Side.Bottom)
        {
            // Material
            propBlock = new MaterialPropertyBlock();

            // Reached the destination -> Let the ball explode
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().isTrigger = true;

            currentSize = transform.localScale;
            transform.localRotation = Quaternion.identity;
            transformingSpeed = GrowthSpeed;
            isTransforming = true;
        } else
        {
            // Dont Start yet!
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

        // Scale or Shrink Smoke
        percentCompleted = elapsedTime / transformingSpeed;
        transform.localScale = Vector3.Lerp(currentSize, GoalSize, percentCompleted);
        elapsedTime += Time.deltaTime;

        UpdateMaterial(percentCompleted);
        if (percentCompleted >= 1)
        {
            if (transform.localScale.x <= 0.01f)
                Destroy(gameObject);

            // Shrink the bomb
            SetUpShrinkVariables();
            StartCoroutine(WaitUntilShrink());
        }
    }

    private void UpdateMaterial(float percentage)
    {

        float amplitude;
        float frequency;

        if (transformingSpeed == ShrinkSpeed)
        {
            // Object is shrinking
            frequency = 50;
            amplitude = Mathf.Clamp(0.5f + percentage * 2.5f, 0f, 3f); // 0.5 -> 
        }
        else
        {
            // Object Growing
            frequency = 25f + percentage * 25f;
            amplitude = 0.3f + percentage * 0.2f; // 0.5
        }

        smokeRendererOuter.GetPropertyBlock(propBlock);
        smokeRendererBottom.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Frequency", frequency); // 0 - 50
        propBlock.SetFloat("_Amplitude", amplitude); // 0 - 0.5
        smokeRendererOuter.SetPropertyBlock(propBlock);
        smokeRendererBottom.SetPropertyBlock(propBlock);
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
