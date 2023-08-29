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

    private void Awake()
    {
        // Initialize
        currentSpeed = walkingSpeed;
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();

        states = new PlayerStateFactory(this);
        currentState = states.Grounded();
        currentState.EnterState();

        // Look
        playerInput.CharacterControls.Look.started += OnMouseInput;
        playerInput.CharacterControls.Look.canceled += OnMouseInput;
        playerInput.CharacterControls.Look.performed += OnMouseInput;

        // Move
        playerInput.CharacterControls.Move.started += OnMovementInput;
        playerInput.CharacterControls.Move.canceled += OnMovementInput;
        playerInput.CharacterControls.Move.performed += OnMovementInput;

        // Run 
        playerInput.CharacterControls.Sneak.started += OnSneakInput;
        playerInput.CharacterControls.Sneak.canceled += OnSneakInput;

        // Jump 
        playerInput.CharacterControls.Jump.started += OnJumpInput;
        playerInput.CharacterControls.Jump.canceled += OnJumpInput;

        // JumpSetup
        SetupJumpVariables();


    }

    private void Start()
    {
        // Move the play just a bit, so he won't fly around.
        CharacterController.Move(appliedMovement * Time.deltaTime); 

        // Register Events
        StateManager.Instance.PauseGameEvent.AddListener(PauseMovement);
        StateManager.Instance.ResumeGameEvent.AddListener(ResumeMovement);
    }

    private void Update()
    {
        if (StateManager.Instance.isLockedOnWitchHead)
            return;
        CurrentState.UpdateStates();
        
        // Move the player
        if (CanMove)
            characterController.Move(appliedMovement * Time.deltaTime);
    }

    private void LateUpdate()
    {
        HandleRotation();
    }

    private void OnDrawGizmos()
    {
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

    public void ActivateMovement(bool activate)
    {
        CanMove = activate;
    }

    private void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2.0f;
        gravity = -2.0f * maxJumpHeight / Mathf.Pow(timeToApex, 2.0f); // Calculated gravity based on height and time
        initialJumpVelocity = 2.0f * maxJumpHeight / timeToApex;  // Starting velocity of the jump
    }

    void OnJumpInput(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    void OnSneakInput(InputAction.CallbackContext context)
    {
        isSneakPressed = context.ReadValueAsButton();

    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0; // x or y != 0 means player moves
    }

    void OnMouseInput(InputAction.CallbackContext context)
    {
        currentMouseInput = context.ReadValue<Vector2>();
        currentMouseVector = currentMouseInput * mouseSensitivity * Time.deltaTime;
    }

    private void PauseMovement()
    {
        Cursor.lockState = CursorLockMode.None;
        CanMove = false;
        OnDisable();
    }

    private void ResumeMovement()
    {
        Cursor.lockState = CursorLockMode.Locked;
        CanMove = true;
        OnEnable();
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
