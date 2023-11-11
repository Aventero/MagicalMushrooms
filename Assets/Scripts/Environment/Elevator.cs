using System.Collections;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStateMachine playerStateMachine = other.GetComponent<PlayerStateMachine>();
            if (playerStateMachine != null)
            {
                playerStateMachine.SwitchStateExternally("Elevate");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStateMachine playerStateMachine = other.GetComponent<PlayerStateMachine>();
            if (playerStateMachine != null)
            {
                playerStateMachine.SwitchStateExternally("Fall");
            }
        }
    }
}
