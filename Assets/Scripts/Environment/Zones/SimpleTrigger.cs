using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleTrigger : MonoBehaviour
{
    public UnityEvent OnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        OnTrigger.Invoke();
        Destroy(gameObject);
    }
}
