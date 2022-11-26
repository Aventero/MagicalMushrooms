using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMechanics : MonoBehaviour
{
    private Animator Animator;

    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
        Animator.Play("DoorAnimator");
        StartCoroutine(DisableAfter(5f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DisableAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Animator.enabled = false;
    }
}
