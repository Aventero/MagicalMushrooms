using UnityEngine;

public class SlimeLookAt : MonoBehaviour
{
    public Transform playerTransform;

    void Start()
    {
        // Find the player by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object not found. Please make sure your player is tagged correctly.");
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            // Rotate the slime to face the player each frame
            transform.LookAt(playerTransform);
        }
    }
}
