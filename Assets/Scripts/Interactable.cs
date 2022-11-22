using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public float interactionSize = 2f;

    public virtual void Start()
    {
        CreateSphereCollider();
    }

    protected virtual void CreateSphereCollider()
    {
        SphereCollider sphereCollider = this.gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = interactionSize;
        sphereCollider.isTrigger = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, interactionSize);
    }
}
