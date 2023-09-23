using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerInput playerInput;
    private CharacterController characterController;

    // Movement
    private readonly float sneakingSpeed = 1.5f;
    private readonly float walkingSpeed = 3.0f;
    private float currentSpeed; 
    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 appliedMovement;
    private bool isMovementPressed;
    private bool isSneakPressed;

    // Gravity
    private float gravity;

    // Jump
    private bool isJumpPressed = false;
    private float initialJumpVelocity;
    public float maxJumpHeight = 2.0f;
    public float maxJumpTime = 1.0f; // Time it takes to complete the jump
    
    // Stepping
    public float StepDistance = 0.75f;
    public float WalkRingLifetime = 1.0f;
    public float WalkRingSize = 0.2f;
    public float SneakRingLifetime = 1.0f;
    public float SneakRingSize = 0.05f;
    public float JumpRingLifetime = 1.5f;
    public float JumpRingSize = 1.0f;

    // Mouse looking
    private Vector2 currentMouseInput;
    private Vector2 currentMouseVector;
    private Vector2 mouseSensitivity = new Vector2(15f, 15f);
    private float xAxisClamp = 85f;
    private float xAxisRotation = 0f;

    private bool CanMove = true;

    // states
    PlayerState currentState;
    PlayerStateFactory states;
    public PlayerState CurrentState { get => currentState; set => currentState = value;  }

    // Getters & Setters
    public bool IsJumpPressed { get => isJumpPressed; }
    public Vector2 CurrentMovementInput { get => currentMovementInput; }
    public CharacterController CharacterController { get => characterController; }
    public float CurrentMovementX { get => currentMovement.x; set => currentMovement.x = value; }
    public float CurrentMovementY { get => currentMovement.y; set => currentMovement.y = value; }
    public float CurrentMovementZ { get => currentMovement.z; set => currentMovement.z = value; }
    public float AppliedMovementX { get => appliedMovement.x; set => appliedMovement.x = value; }
    public float AppliedMovementY { get => appliedMovement.y; set => appliedMovement.y = value; }
    public float AppliedMovementZ { get => appliedMovement.z; set => appliedMovement.z = value; }
    public float InitialJumpVelocity { get => initialJumpVelocity; set => initialJumpVelocity = value; }
    public float Gravity { get => gravity; }
    public bool IsMovementPressed { get => isMovementPressed; }
    public bool IsSneakPressed { get => isSneakPressed; }
    public float SneakingSpeed { get => sneakingSpeed; }
    public float WalkingSpeed { get => walkingSpeed; }
    public float CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }

    public Vector3 ExternalMovement = Vector3.zero;

    private void Awake()
    {
        // Initialize
        currentSpeed = walkingSpeed;
        characterController = GetComponent<CharacterController>();

        states = new PlayerStateFactory(this);
        currentState = states.Grounded();
        currentState.EnterState();

        // JumpSetup
        SetupJumpVariables();
    }

    private void Start()
    {
        // Move the play just a bit, so he won't fly around.
        CharacterController.Move(appliedMovement * Time.deltaTime);

        // Register Events
        StateManager.Instance.PauseMovementEvent.AddListener(PauseMovement);
        StateManager.Instance.ResumeMovementEvent.AddListener(ResumeMovement);
        StateManager.Instance.PauseGameEvent.AddListener(Pause);
        StateManager.Instance.ResumeGameEvent.AddListener(Resume);
    }



    private void Update()
    {
        if (StateManager.Instance.isLockedOnWitchHead)
            return;
        CurrentState.UpdateStates();
        HandleRotation();

        // Move the player
        if (CanMove)
            characterController.Move(appliedMovement * Time.deltaTime + ExternalMovement);
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody hitRigidbody = hit.collider.attachedRigidbody;
        if (hitRigidbody != null)
            hitRigidbody.isKinematic = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody hitRigidbody = other.attachedRigidbody;
        if (hitRigidbody != null)
            hitRigidbody.isKinematic = false;
    }


    void HandleRotation()
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

    private void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2.0f;
        gravity = -2.0f * maxJumpHeight / Mathf.Pow(timeToApex, 2.0f); // Calculated gravity based on height and time
        initialJumpVelocity = 2.0f * maxJumpHeight / timeToApex;  // Starting velocity of the jump
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    public void OnSneak(InputAction.CallbackContext context)
    {
        isSneakPressed = context.ReadValueAsButton();

    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0; // x or y != 0 means player moves
    }

    public void OnMouse(InputAction.CallbackContext context)
    {
        currentMouseInput = context.ReadValue<Vector2>();
        currentMouseVector = currentMouseInput * mouseSensitivity * Time.deltaTime;
    }

    private void PauseMovement()
    {
        CanMove = false;
    }

    private void ResumeMovement()
    {
        CanMove = true;
    }

    private void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        CanMove = false;
        OnDisable();
    }

    private void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        CanMove = true;
        OnEnable();
    }


    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }
    
    private void OnDrawGizmos()
    {
    }
}
