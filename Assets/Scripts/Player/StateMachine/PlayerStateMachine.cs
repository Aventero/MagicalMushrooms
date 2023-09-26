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
    
    // Stepping rings
    public float StepDistance = 0.75f;
    public float WalkRingLifetime = 1.0f;
    public float WalkRingSize = 0.2f;
    public float SneakRingLifetime = 1.0f;
    public float SneakRingSize = 0.05f;
    public float JumpRingLifetime = 1.5f;
    public float JumpRingSize = 1.0f;

    // Camera
    public Quaternion InitialCameraRotation { get; private set; }
    private Vector3 currentRotationVelocity;
    private Vector3 currentCameraVelocity;
    private float targetPlayerYRotation;
    private float targetCameraXRotation;
    public float cameraWobbleAmount = 5.0f;

    // Camera & Object wobbling frequency
    public float wobbleFrequency = 10.0f; 

    // Mouse looking
    private Vector2 currentMouseInput;
    private Vector2 currentMouseVector;
    private Vector2 mouseSensitivity = new Vector2(15f, 15f);
    private float xAxisClamp = 85f;
    private float xAxisRotation = 0;
    public float rotationSpeed = 1.0f;
    public float smoothTime = 0.1f;

    // Object
    public float ObjectLerpSpeed = 2f;
    public Transform heldObject; 
    public float swayAmount = 0.002f; 
    public float walkWobbleAmount = 0.02f;
    public float YInertiaStrength = 0.05f;
    public float RotionalInertiaStrength = 0.05f;
    public Vector3 InitialHeldObjectPosition { get; private set; }
    public Quaternion InitialHeldObjectRotation { get; private set; }

    private bool CanMove = true;
    private bool CanRotate = true;

    // Player States
    PlayerState currentState;
    PlayerStateFactory states;
    public PlayerState CurrentState { get => currentState; set => currentState = value;  }

    // Movement Getters & Setters
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
        InitialHeldObjectPosition = heldObject.localPosition;
        InitialHeldObjectRotation = heldObject.localRotation;
        InitialCameraRotation = Camera.main.transform.localRotation;
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


        if (CanRotate)
        {
            // Move Camera
            HandleRotation();
            HandleHeldObjectSway();
        }

        if (CanMove)
        {
            // Move the player
            CurrentState.UpdateStates();
            characterController.Move(appliedMovement * Time.deltaTime + ExternalMovement);
        }
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

    void HandleHeldObjectSway()
    {
        // Movement of the Object towards the opposite turn
        float movementX = -currentMouseVector.x * swayAmount;
        float movementY = -currentMouseVector.y * swayAmount;

        Vector3 targetPosition = new Vector3(InitialHeldObjectPosition.x + movementX, InitialHeldObjectPosition.y, InitialHeldObjectPosition.z + movementY);
        heldObject.localPosition = Vector3.Lerp(heldObject.localPosition, targetPosition, Time.deltaTime * ObjectLerpSpeed);
    }

    void ApplyHeldObjectInertia(float yVelocityDifference)
    {
        if (!heldObject)
            return;

        // Calculate position offset based on velocity difference.
        Vector3 positionOffset = new Vector3(0, -yVelocityDifference * 0.05f, 0); // Adjust multiplier as needed.

        // Calculate rotation offset. This will tilt the object slightly based on the change in velocity.
        Vector3 rotationOffsetEuler = new Vector3(-yVelocityDifference * 2f, 0, 0); // Adjust multiplier as needed.

        // Apply inertia effects to the held object.
        heldObject.localPosition = InitialHeldObjectPosition + positionOffset;
        heldObject.localRotation = InitialHeldObjectRotation * Quaternion.Euler(rotationOffsetEuler);
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
        CanRotate = false;
        OnDisable();
    }

    private void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        CanMove = true;
        CanRotate = true;
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
