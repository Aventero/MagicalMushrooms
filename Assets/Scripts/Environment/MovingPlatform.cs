using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform Start;
    public Transform End;
    public float Speed = 2.0f;
    public float WaitTime = 2f;
    private GameObject player;
    private float ElapsedTime = 0;
    private float TimeToDestination = 0;
    private float distance;
    private bool canMove = true;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        CalculateTimeNeeded();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(Start.position, End.position);
        Gizmos.DrawWireSphere(End.position, 0.3f);
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            float percentageTraveled = MovePlatform();
            if (percentageTraveled >= 1)
            {
                canMove = false;
                StartCoroutine(WaitThenSwap());
            }
        }
    }

    private float MovePlatform()
    {
        Wobble wobble = GetComponent<Wobble>();
        ElapsedTime += Time.deltaTime;
        float percentageTraveled = ElapsedTime / TimeToDestination;
        percentageTraveled = Mathf.SmoothStep(0, 1, percentageTraveled);
        Vector3 positionBefore = Vector3.Lerp(Start.position, End.position, percentageTraveled);
        transform.position = wobble.WobbleAtTime(positionBefore, percentageTraveled);
        return percentageTraveled;
    }

    IEnumerator WaitThenSwap()
    {
        yield return new WaitForSeconds(WaitTime);
        SwapStartEnd();
        canMove = true;
    }

    private void SwapStartEnd()
    {
        (Start, End) = (End, Start);
        CalculateTimeNeeded();
    }

    private void CalculateTimeNeeded()
    {
        distance = Vector3.Distance(transform.position, End.position);
        ElapsedTime = 0;
        TimeToDestination = distance / Speed;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            player.transform.SetParent(gameObject.transform);
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            player.transform.SetParent(null);
    }
}