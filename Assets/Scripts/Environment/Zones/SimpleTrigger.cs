using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleTrigger : MonoBehaviour
{
    public UnityEvent OnTrigger;
    public bool DestroyOnTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        OnTrigger.Invoke();
        if (DestroyOnTrigger)
            Destroy(gameObject);
    }
}
