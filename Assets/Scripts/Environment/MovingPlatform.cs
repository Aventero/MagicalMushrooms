using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : Interactable
{
    public Transform StartPoint;
    public Transform EndPoint;
    public Vector3 Destination;
    public Vector3 StartingPosition;
    public float Speed = 2.0f;
    public float MaxTime = 3.0f;
    public float Height = 0.5f;
    public float ElapsedTime = 0;

    private void Awake()
    {
        StartingPosition = EndPoint.position;
        Destination = StartPoint.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(StartingPosition, Destination);
        Gizmos.DrawWireSphere(Destination, 0.3f);
    }

    private void SwapStartEnd()
    {
        Vector3 tmp = Destination;
        Destination = StartingPosition;
        StartingPosition = tmp;
    }

    public override void Interact()
    {
        CanInteract = false;
        SwapStartEnd();
        player.transform.SetParent(gameObject.transform);
    }

    private void FixedUpdate()
    {
        if (!ReachedDestination())
            MovePlatform();
        if (ReachedDestination())
            CanInteract = true;
    }

    private bool ReachedDestination()
    {
        if (Vector3.Distance(transform.position, Destination) < 0.05f)
            return true;
        return false;
    }

    private void MovePlatform()
    {
        transform.position = transform.position + (Speed * Time.deltaTime * (Destination - StartingPosition).normalized);
    }


    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.CompareTag("Player"))
            player.transform.SetParent(gameObject.transform);
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (other.CompareTag("Player"))
            player.transform.SetParent(null);
    }
}
