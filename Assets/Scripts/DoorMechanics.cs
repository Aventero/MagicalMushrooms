using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMechanics : MonoBehaviour
{
    private Animator Animator;

    void Start()
    {
        Animator = GetComponent<Animator>();
        Animator.Play("CloseDoor");
        StartCoroutine(DisableAfter(5f));
        StateManager.Instance.AllItemsCollectedEvent += OpenDoor;
    }

    private void OpenDoor()
    {
        StopAllCoroutines();
        Animator.enabled = true;
        Animator.Play("OpenDoor");
        StartCoroutine(DisableAfter(5f));
    }

    IEnumerator DisableAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Animator.enabled = false;
    }
}
