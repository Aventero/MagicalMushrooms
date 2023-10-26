using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Dragging : PlayerSkill
{
    public LayerMask draggingLayerMask;

    public bool IsDragging = false;
    public float DistanceFromCamera = 3.0f;
    public float DraggingSpeed = 2.0f;
    public float AvoidanceDistance = 1f;
    public float MaxVelocity = 10f;
    public float ShootForce = 10f;

    [Header("Particles")]
    public ParticleSystem draggingParticleSystem;

    public GameObject draggingObject = null;
    private Rigidbody draggingBody = null;
    private Camera mainCamera;
    private bool isReadyToShoot = false;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Activly looking at an object
        if (IsActivated && DraggableManager.Instance.DraggableObject != null && !IsDragging)
            UIManager.Instance.ShowSkillTooltip("Grab!", MouseSide.LeftClick);

        if (!IsDragging)
            return;

        // Activly looking at an object
        UIManager.Instance.ShowSkillTooltip("Shoot!", MouseSide.LeftClick);
        DraggingLogic();
    }

    private void DraggingLogic()
    {
        // Activly Dragging

        // Dragging the object
        draggingBody.isKinematic = false;
        draggingBody.interpolation = RigidbodyInterpolation.Interpolate;

        // Calculate target position
        Vector3 cursorPosition = Input.mousePosition;
        cursorPosition.z = DistanceFromCamera;

        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(cursorPosition);
        Vector3 velocity = (targetPosition - draggingBody.position) * 10f;
        velocity.x = Mathf.Clamp(velocity.x, -MaxVelocity, MaxVelocity);
        velocity.y = Mathf.Clamp(velocity.y, -MaxVelocity, MaxVelocity);
        velocity.z = Mathf.Clamp(velocity.z, -MaxVelocity, MaxVelocity);
        draggingBody.velocity = velocity;
    }

    public override void ShowPreview()
    {
        DraggableManager.Instance.EnableSearch();
        draggingParticleSystem.Play();
        IsActivated = true;
    }

    public override void HidePreview()
    {
        UIManager.Instance.HideTooltip();
        DraggableManager.Instance.DisableSearch();
        draggingParticleSystem.Stop();
        IsActivated = false;
    }

    public override bool Execute()
    {
        // No object -> Stop the skill Or already dragging
        if (DraggableManager.Instance.DraggableObject == null || IsDragging)
            return false;

        // Activate the skill!
        if (DraggableManager.Instance.DraggableObject.GetComponent<Rigidbody>() == null)
            DraggableManager.Instance.DraggableObject.AddComponent<Rigidbody>();

        draggingObject = DraggableManager.Instance.DraggableObject.gameObject;
        draggingBody = draggingObject.GetComponent<Rigidbody>();

        IsDragging = true;
        StartCoroutine(DelayedShootEnable());
        PlayerSkillManager.Instance.LockSkills();
        UIManager.Instance.HideTooltip();
        return false;
    }

    IEnumerator DelayedShootEnable()
    {
        isReadyToShoot = false;
        yield return new WaitForSeconds(0.2f); // Wait for 200ms
        isReadyToShoot = true;
    }

    public void OnDraggingShoot(InputAction.CallbackContext callback)
    {
        if (!callback.performed || !isReadyToShoot)
            return;

        // Performed
        IsDragging = false;
        isReadyToShoot = false;
        HidePreview();

        PlayerSkillManager.Instance.SkillCompleted();
        PlayerSkillManager.Instance.UnlockSkills();
        draggingBody.GetComponent<DraggableObject>().IsFlying = true;
        draggingBody.AddForce(Camera.main.transform.forward * ShootForce, ForceMode.Impulse);
    }
}
