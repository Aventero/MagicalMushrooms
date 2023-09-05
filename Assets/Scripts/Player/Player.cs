using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Witch")
        {
            StateManager.Instance.IsVisionConeOnPlayer = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Witch View Cone
        if (other.tag == "Witch")
        {
            StateManager.Instance.IsVisionConeOnPlayer = true;
        }
    }
}
