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
        
        //player.transform.SetParent(gameObject.transform);
        StartCoroutine(MoveTowards(Destination));
    }

    private IEnumerator MoveTowards(Vector3 destination)
    {
        PlayerStateMachine playerStateMachine = player.GetComponent<PlayerStateMachine>();
        float deltaTime = 0;
        while (deltaTime < MaxTime)
        {
            // TODO: Player has an External Velocity vector thats added to the players movement.
            // TODO: Make the Platform a cage?

            // TODO: Just make the player float and let him fly to another place
            // So the platform is the child thats moving with the player
            // Teleport the player on the platform
            // Cage transport system? xD

            Vector3 change = transform.position;
            transform.position = Vector3.Lerp(StartingPosition, destination, deltaTime / MaxTime);
            playerStateMachine.ExternalMovement = transform.position - change;
            deltaTime += Time.deltaTime * Speed;
            yield return null;
        }
        playerStateMachine.ExternalMovement = Vector3.zero;
        CanInteract = true;
        player.transform.SetParent(null);
    }
}
