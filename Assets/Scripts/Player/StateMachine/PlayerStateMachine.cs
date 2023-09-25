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
    private Quaternion initialCameraRotation;
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
    public float rotationSpeed = 1.0f;
    public float smoothTime = 0.1f;

    // Object
    public float ObjectLerpSpeed = 2f;
    public Transform heldObject; 
    public float swayAmount = 0.002f; 
    public float walkWobbleAmount = 0.02f;
    private Vector3 initialHeldObjectPosition;

    private bool CanMove = true;
    private bool CanRotate = true;

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
        initialHeldObjectPosition = heldObject.localPosition;
        initialCameraRotation = Camera.main.transform.localRotation;
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
            HandleCameraWobble();
            HandleHeldObjectWobble();
            CurrentState.UpdateStates();
            characterController.Move(appliedMovement * Time.deltaTime + ExternalMovement);
        }
    }

    void HandleRotation()
    {
        // Calculate targetRotations
        targetPlayerYRotation += currentMouseVector.x * rotationSpeed;
        targetCameraXRotation -= currentMouseVector.y * rotationSpeed;

        // Clamp camera X rotation
        targetCameraXRotation = Mathf.Clamp(targetCameraXRotation, -xAxisClamp, xAxisClamp);

        // Rotate Player around the Y-Axis
        float smoothedPlayerYRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetPlayerYRotation, ref currentRotationVelocity.y, smoothTime);
        transform.rotation = Quaternion.Euler(0f, smoothedPlayerYRotation, 0f);

        // Rotate Camera around X-Axis
        float smoothedCameraXRotation = Mathf.SmoothDampAngle(Camera.main.transform.eulerAngles.x, targetCameraXRotation, ref currentCameraVelocity.x, smoothTime);
        Camera.main.transform.rotation = Quaternion.Euler(smoothedCameraXRotation, transform.eulerAngles.y, 0f);

    }

    void HandleCameraWobble()
    {
        if (isMovementPressed)
        {
            // Camera wobble and Object wobble are the same Frequency, making it feel more "correct"
            float wobblePitch = Mathf.Sin(Time.time * wobbleFrequency) * cameraWobbleAmount; // Up - down
            float wobbleYaw = Mathf.Sin(Time.time * wobbleFrequency * 2f) * cameraWobbleAmount * 0.5f; // Left -Right

            // Apply wobble to camera's rotation
            Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(wobblePitch, wobbleYaw, 0f);
            Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, targetRotation, Time.deltaTime * ObjectLerpSpeed);
        }
        else
        {
            // Not moving -> Camera initial rotation
            Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, initialCameraRotation, Time.deltaTime * ObjectLerpSpeed);
        }
    }

    void HandleHeldObjectSway()
    {
        // Movement of the Object towards the opposite turn
        float movementX = -currentMouseVector.x * swayAmount;
        float movementY = -currentMouseVector.y * swayAmount;

        Vector3 targetPosition = new Vector3(initialHeldObjectPosition.x + movementX, initialHeldObjectPosition.y, initialHeldObjectPosition.z + movementY);
        heldObject.localPosition = Vector3.Lerp(heldObject.localPosition, targetPosition, Time.deltaTime * ObjectLerpSpeed);
    }

    void HandleHeldObjectWobble()
    {
        if (isMovementPressed)
        {
            // Calculate wobble amount -> sin(oscilation) * amplitude
            float wobbleX = Mathf.Sin(Time.time * wobbleFrequency) * walkWobbleAmount; // For side-to-side wobble
            float wobbleY = Mathf.Sin(Time.time * wobbleFrequency * 2f) * walkWobbleAmount * 0.5f; // For up-and-down wobble

            // Apply wobble to object's position
            Vector3 targetPosition = new Vector3(initialHeldObjectPosition.x + wobbleX, initialHeldObjectPosition.y + wobbleY, initialHeldObjectPosition.z);
            heldObject.localPosition = Vector3.Lerp(heldObject.localPosition, targetPosition, Time.deltaTime * ObjectLerpSpeed);
        }
        else
        {
            // If not moving, smoothly reset the object's position to its initial position
            heldObject.localPosition = Vector3.Lerp(heldObject.localPosition, initialHeldObjectPosition, Time.deltaTime * ObjectLerpSpeed);
        }
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
