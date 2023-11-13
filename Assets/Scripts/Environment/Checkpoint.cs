using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool activated = false;

    private Vector3 respawnPosition;
    private Quaternion playerRotation;
    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (this.transform.childCount > 0)
            respawnPosition = transform.GetChild(0).position;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag("Player"))
            return;

        StateManager.Instance.NewCheckpointEvent.Invoke();

        activated = true;
        playerRotation = player.transform.rotation;
        CheckpointManager.Instance.Checkpoint = this;
    }

    public void SetActivated(bool activated)
    {
        this.activated = activated;
    }

    public Vector3 GetRespawnPoint()
    {
        if(this.transform.childCount == 0)
            return this.transform.position;
        else
            return respawnPosition;
    }

    public void SetRotation(Quaternion rotation)
    {
        this.playerRotation = rotation;
    }

    public Quaternion GetRotation()
    {
        return playerRotation;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 respawnPos;

        if (transform.childCount == 0)
            respawnPos = transform.position;
        else
            respawnPos = transform.GetChild(0).position;

        Gizmos.DrawWireSphere(respawnPos, 0.2f);
    }
}
