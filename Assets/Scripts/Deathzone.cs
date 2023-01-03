using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Deathzone : MonoBehaviour
{
    public Vector2 Size;
    private Vector3 ColliderSize;

    void Start()
    {
        SetupBoxCollider();
    }

    private void SetupBoxCollider()
    {
        BoxCollider boxCollider= this.gameObject.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(Size.x, 0.01f, Size.y);
        boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.tag.Equals("Player"))
            return;

        StateManager.Instance.GameOverEvent.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(this.transform.position, new Vector3(Size.x, 0.01f, Size.y));
    }

}

