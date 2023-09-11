using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class CageOpener : Interactable
{
    public float OpeningSpeed = 1f;
    public float MaxTime = 3;
    public int Degrees = 180;

    public override void Interact()
    {
        base.Interact();
        Debug.Log("Starting to turn?");
        CanInteract = false;
        StartCoroutine(OpenDoorOverTime());
    }

    private IEnumerator OpenDoorOverTime()
    {
        float deltaTime = 0f;
        Vector3 axis = transform.up;

        while (deltaTime < MaxTime)
        {
            deltaTime += Time.deltaTime;

            float rotationAmount = Mathf.LerpAngle(0, Degrees, deltaTime / MaxTime);
            transform.rotation = Quaternion.Euler(axis * rotationAmount);
            yield return null;
        }

        GetComponent<Outline>().enabled = false;
        enabled = false;
        // Todo: Actually make it not interactable anymore
    }
}
