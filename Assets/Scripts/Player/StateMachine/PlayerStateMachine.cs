using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    private CharacterController characterController;

    // Movement
    private readonly float sneakingSpeed = 1.5f;
    private readonly float walkingSpeed = 3.0f;
    private readonly float slurpSpeed = 1.0f;
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
    public float smoothTime = 0.1f;

    // Object
    public float ObjectLerpSpeed = 2f;
    public Transform heldObject;
    [Header("Rod Sway when moving camera")]
    public float swayAmount = 0.002f;
    [Header("Rod wobbling when moving")]
    public float walkWobbleAmount = 0.02f;
    [Header("Rod y travel when jumping")]
    public float YInertiaStrength = 0.05f;
    [Header("Rod rotational travel when jumping")]
    public float RotionalInertiaStrength = 0.05f;
    public Vector3 InitialHeldObjectPosition { get; private set; }
    public Quaternion InitialHeldObjectRotation { get; private set; }

    private bool CanMove = true;
    private bool CanRotate = true;
    private bool IsSlurping = false;

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

    public float SlurpSpeed { get => slurpSpeed; }
    public float WalkingSpeed { get => IsSlurping ? SlurpSpeed : walkingSpeed; }
    public float CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }

    public void ExternalSwitchStates(string stateToSwitchTo)
    {
        CurrentState.SwitchState(states.GetState(stateToSwitchTo));
    }

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
        StateManager.Instance.PausePlayerMovementEvent.AddListener(() => CanMove = false);
        StateManager.Instance.ResumePlayerMovementEvent.AddListener(() => CanMove = true);
        StateManager.Instance.PausePlayerCameraEvent.AddListener(() => CanRotate = false);
        StateManager.Instance.ResumePlayerCameraEvent.AddListener(() => CanRotate = true);
        StateManager.Instance.PauseGameEvent.AddListener(() => CanMove = false);
        StateManager.Instance.PauseGameEvent.AddListener(() => CanRotate = false);
        StateManager.Instance.ResumeGameEvent.AddListener(() => CanMove = true);
        StateManager.Instance.ResumeGameEvent.AddListener(() => CanRotate = true);
        StateManager.Instance.StartSlurpingEvent.AddListener(() => IsSlurping = true);
        StateManager.Instance.EndSlurpingEvent.AddListener(() => IsSlurping = false);
        StateManager.Instance.EndCutsceneEvent.AddListener(() => LockMouseInput());
    }

    private void Update()
    {
        if (StateManager.Instance.IsLockedOnWitchHead)
            return;

        if (StateManager.Instance.IsInCutscene)
            return;

        if (CanMove)
        {
            // Move the player
            characterController.Move(appliedMovement * Time.deltaTime);
            CurrentState.UpdateStates();
        }
    }

    private void LateUpdate()
    {
        if (StateManager.Instance.IsLockedOnWitchHead)
            return;

        if (StateManager.Instance.IsInCutscene)
            return;

        if (CanRotate)
        {
            // Rotate Camera
            HandleRotation();
            HandleHeldObjectSway();
        }
    }

    public void SwitchStateExternally(string stateName)
    {
        CurrentState.SwitchState(states.GetState(stateName));
    }

    void LockMouseInput()
    {
        currentMouseInput = Vector2.zero;
        currentMouseVector = Vector2.zero;
        xAxisRotation = 0;
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

        if (StateManager.Instance == null || StateManager.Instance.IsInCutscene)
            return;

        currentMouseInput = context.ReadValue<Vector2>();
        currentMouseVector = currentMouseInput * mouseSensitivity * Time.deltaTime;
    }
}
