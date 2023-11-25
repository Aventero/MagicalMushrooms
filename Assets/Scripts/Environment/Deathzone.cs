using UnityEngine;

public class Deathzone : MonoBehaviour
{
    public Vector2 Size;
    private Vector3 ColliderSize;

    void Start()
    {
        ColliderSize = new Vector3(Size.x, 1.0f, Size.y);
        SetupBoxCollider();
    }

    private void SetupBoxCollider()
    {
        BoxCollider boxCollider= this.gameObject.AddComponent<BoxCollider>();
        boxCollider.size = ColliderSize;
        boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        StateManager.Instance.PlayerDiedEvent.Invoke();
        CheckpointManager.Instance.RespawnPlayer();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(this.transform.position, ColliderSize);
    }

}

