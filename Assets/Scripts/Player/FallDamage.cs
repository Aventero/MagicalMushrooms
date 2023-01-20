using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDamage : MonoBehaviour
{
    public float minFallDistance = 2;

    private CharacterController characterController;
    private NewPlayerMovement playerMovement;
    private Vector3 startFallPosition;

    private bool wasGrounded;
    private float fallingDistance = 0;

    // Start is called before the first frame update
    void Start()
    {
        characterController = this.GetComponent<CharacterController>();
        playerMovement = this.GetComponent<NewPlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        // Player is moving negative on the y
        if (playerMovement.IsFalling)
        {
            if (wasGrounded)
            {
                startFallPosition = this.transform.position;
                wasGrounded = false;
            }

            fallingDistance = startFallPosition.y - transform.position.y;
        }

        if (characterController.isGrounded && fallingDistance > minFallDistance)
        {
            // calc dmg
            int fallDamage = CalculateFallDamage();
            StateManager.Instance.DealDamageEvent(fallDamage);
        }

        // set the position the player was last grounded
        if (characterController.isGrounded)
        {
            fallingDistance = 0;
            wasGrounded = true;
        }
    }

    private int CalculateFallDamage()
    {
        float fallDistance = startFallPosition.y - this.transform.position.y;
        int damage = (int)(fallDistance / 2.0f);
        return damage;
    }
}
