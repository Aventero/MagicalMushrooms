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
    private bool isFlying = true;
    private bool isTransforming = false;
    private float transformingSpeed;
    private float elapsedTime = 0;
    public Renderer smokeRendererOuter;
    public Renderer smokeRendererBottom;
    private MaterialPropertyBlock propBlock;


    private void OnCollisionStay(Collision collision)
    {

        if (collision.collider.CompareTag("Player"))
            return;

        CollisionUtil.Side collisionSide = CollisionUtil.CalculateCollisionSideSphere(collision);
        Debug.Log(collisionSide);

        // Start Growing 
        if (collisionSide == CollisionUtil.Side.Bottom)
        {
            // Material
            isFlying = false;
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
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isFlying)
            UIManager.Instance.ShowSmokeFrame(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isFlying)
            UIManager.Instance.ShowSmokeFrame(false);
    }

    private void Update()
    {
        if (isFlying)
            return;

        if (isTransforming)
        {
            // Scale or Shrink Smoke
            percentCompleted = elapsedTime / transformingSpeed;
            percentCompleted = Mathf.SmoothStep(0, 1, percentCompleted);
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
        } else
        {
            UpdateMaterial(0);
        }
    }

    private void UpdateMaterial(float percentage)
    {

        float amplitude;
        float frequency;
        float speed; 

        if (transformingSpeed == ShrinkSpeed)
        {
            // Object is shrinking
            frequency = 50;
            amplitude = Mathf.Clamp(0.5f + percentage * 2.5f, 0f, 3f); // 0.5 ->
            speed = 0.1f;
        }
        else if (transformingSpeed == GrowthSpeed)
        {
            // Object Growing
            frequency = 49f + percentage * 1f;
            amplitude = 0.3f + percentage * 0.2f; // 0.5
            speed = 0.1f;
        }
        else
        {
            // Object is stationary
            amplitude = 0.5f;
            frequency = 50f;
            speed = 0.1f;
        }


        smokeRendererOuter.GetPropertyBlock(propBlock);
        smokeRendererBottom.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Frequency", frequency); // 0 - 50
        propBlock.SetFloat("_Amplitude", amplitude); // 0 - 0.5
        propBlock.SetFloat("_Speed", speed);
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
        transformingSpeed = 0;
        yield return new WaitForSeconds(WaitTillShrink); // wait a bit
        isTransforming = true;
        transformingSpeed = ShrinkSpeed;
    }
}
