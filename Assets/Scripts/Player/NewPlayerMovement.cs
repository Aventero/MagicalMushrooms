using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System;

public class NewPlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private CharacterController characterController;

    // Movement
    private float runningSpeed = 8.0f;
    private float walkingSpeed = 4.0f;
    private float currentSpeed; // Decided, by what user is pressing
    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private bool isMovementPressed;

    // Gravity
    private float gravity = -9.8f;
    private float groundedGravity = -0.05f;

    // Jump
    private bool isJumping = false;
    private bool isJumpPressed = false;
    private float initialJumpVelocity;  // Explosive jump force
    public float maxJumpHeight = 2.0f; // 
    public float maxJumpTime = 1.0f; // Time it takes to complete the jump

    // Mouse looking
    private Vector2 currentMouseInput;
    private Vector2 currentMouseVector;
    private Vector2 mouseSensitivity = new Vector2(15f, 15f);
    private float xAxisClamp = 85f;
    private float xAxisRotation = 0f;

    public bool IsFalling { private set; get; }

    // Called before Start()
    private void Awake()
    {
        // Initialize
        currentSpeed = walkingSpeed;
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();

        // Run input
        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;

        // Jump input
        playerInput.CharacterControls.Jump.started += onJump;
        playerInput.CharacterControls.Jump.canceled += onJump;

        // JumpSetup
        SetupJumpVariables();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2.0f;
        gravity = (-2.0f * maxJumpHeight) / Mathf.Pow(timeToApex, 2.0f); // Calculated gravity based on height and time
        initialJumpVelocity = (2.0f * maxJumpHeight) / timeToApex;  // Starting velocity of the jump
    }

    private void Update()
    {
        if (StateManager.Instance.isLockedOnWitchHead || StateManager.Instance.InMenu)
            return;

        readMouseInput();
        readMovementInput();
        handleRotation();

        // Move the player
        characterController.Move(currentMovement * Time.deltaTime);

        if (!StateManager.Instance.OnElevator)
            handleGravity();
        handleJump();
    }

    private void onJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    void onRun(InputAction.CallbackContext context)
    {
        if (context.started)
            currentSpeed = runningSpeed;

        if (context.canceled)
            currentSpeed = walkingSpeed;
    }

    void readMovementInput()
    {
        currentMovementInput = playerInput.CharacterControls.Move.ReadValue<Vector2>();
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0; // x or y != 0 means player moves

        // The Vector multiplied by whats being pressed
        Vector3 movement = (transform.right * currentMovementInput.x + transform.forward * currentMovementInput.y) * currentSpeed;
        currentMovement.x = movement.x;
        currentMovement.z = movement.z;
    }

    void readMouseInput()
    {
        currentMouseInput = playerInput.CharacterControls.Look.ReadValue<Vector2>();
        currentMouseVector = currentMouseInput * mouseSensitivity * Time.deltaTime;
    }

    void handleRotation()
    {
        // Rotate Player around the Y-Axis
        transform.Rotate(Vector3.up, currentMouseVector.x);

        // Rotate the camera around X-Axis
        xAxisRotation -= currentMouseVector.y;
        xAxisRotation = Mathf.Clamp(xAxisRotation, -xAxisClamp, xAxisClamp);
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = xAxisRotation;
        Camera.main.transform.eulerAngles = targetRotation;
    }

    void handleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            // Jump if Player is on the ground
            isJumping = true;
            currentMovement.y = initialJumpVelocity * 0.5f;
        } 
        else if (!isJumpPressed && isJumping && characterController.isGrounded)
        {
            // Reset while the player is not pressing the jump button
            isJumping = false;
        }
    }

    void handleGravity()
    {
        // Increase the Falling speed when at apex or the player has released the jump button
        IsFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 2.0f;

        // Set a small grounding gravity
        if (characterController.isGrounded)
        {
            currentMovement.y = groundedGravity;
        } 
        else if (IsFalling)
        {
            // When the player is falling, increase his falling velocity!
            float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = Mathf.Max((currentMovement.y + newYVelocity) * 0.5f, -20.0f); // Player will not fall faster after -20.0f 
            currentMovement.y = nextYVelocity;
        }
        else
        {
            // Gravity, when the player has not reached the jumping apex yet!
            float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);    
            float nextYVelocity = (currentMovement.y + newYVelocity) * 0.5f;    // Average the velocity
            currentMovement.y = nextYVelocity;
        }
    }

    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
