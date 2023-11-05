using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine.Events;

public class CameraZone : MonoBehaviour
{
    public CinemachineVirtualCamera scriptedSequenceCam;
    public List<Transform> lookAtTargets;
    public float timeToLookAtEachTarget = 3f; // time in seconds to look at each target

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Disable the player's camera control script
            StateManager.Instance.IsInCutscene = true;
            StateManager.Instance.StartCutsceneEvent.Invoke();

            scriptedSequenceCam.LookAt = lookAtTargets.First();
            StartCameraSequence();
        }
    }

    private void StartCameraSequence()
    {
        scriptedSequenceCam.Priority = 20;
        StartCoroutine(LookAtTargetsRoutine());
    }

    private IEnumerator LookAtTargetsRoutine()
    {
        // Ensure there are targets to look at
        if (lookAtTargets.Count == 0)
        {
            yield break; // Exit if there are no targets
        }

        // Create an interim LookAt object to smoothly transition between targets
        GameObject interimLookAt = new GameObject("InterimLookAt");
        scriptedSequenceCam.LookAt = interimLookAt.transform;

        // Start by looking at the first target in the list
        interimLookAt.transform.position = lookAtTargets[0].position;

        foreach (var target in lookAtTargets)
        {
            Vector3 startPosition = interimLookAt.transform.position;
            Vector3 endPosition = target.position;

            float timeElapsed = 0f;

            while (timeElapsed < timeToLookAtEachTarget)
            {
                // Calculate the lerp factor, which should go from 0 to 1
                float lerpFactor = timeElapsed / timeToLookAtEachTarget;
                lerpFactor = Mathf.SmoothStep(0f, 1f, lerpFactor); 
                interimLookAt.transform.position = Vector3.Lerp(startPosition, endPosition, lerpFactor);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure the interim LookAt object is precisely at the target's position at the end of the lerp
            interimLookAt.transform.position = target.position;
            yield return new WaitForSeconds(0.1f);
        }

        // Clean up
        Destroy(interimLookAt);
        scriptedSequenceCam.Priority = 0;
        StateManager.Instance.IsInCutscene = false;
        StateManager.Instance.EndCutsceneEvent.Invoke();

        // Remove this object
        Destroy(gameObject);
    }
}
