using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Witch")
        {
            StateManager.Instance.WitchConeOnPlayer = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Witch View Cone
        if (other.tag == "Witch")
        {
            StateManager.Instance.WitchConeOnPlayer = true;
        }
    }
}
