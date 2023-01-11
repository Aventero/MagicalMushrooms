using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public GameObject Player;
    private CharacterController CharacterController;
    public float Range = 10.0f;
    public float Power = 100.0f;

    private void Start()
    {
        CharacterController = Player.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Hopefully range on 2D plane
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 player2DPos = new Vector2(Player.transform.position.x, Player.transform.position.z);

        float distance = Vector2.Distance(pos2D, player2DPos);
        if (distance <= Range)
        {
            StateManager.Instance.OnElevator = true;
            Debug.DrawRay(transform.position, Player.transform.position - transform.position);
            CharacterController.Move(Vector3.up * Power * Time.deltaTime);
        }
        else
        {
            StateManager.Instance.OnElevator = false;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
