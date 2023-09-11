using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class CageOpener : Interactable
{
    public float OpeningSpeed = 1f;
    public float MaxTime = 3;
    public int Degrees = 150;

    public override void Interact()
    {
        base.Interact();
        Debug.Log("Starting to turn?");
        CanInteract = false;
        GetComponent<Outline>().enabled = false;
        StartCoroutine(OpenDoorOverTime());
    }

    private IEnumerator OpenDoorOverTime()
    {

        float deltaTime = 0f;
        while (deltaTime < MaxTime)
        {
            deltaTime += Time.deltaTime;
            float t = deltaTime / MaxTime;
            transform.localRotation = Quaternion.AngleAxis(t * Degrees, transform.up);
            yield return null;
        }
        enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 10f);
    }
}
