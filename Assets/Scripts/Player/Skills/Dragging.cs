using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Dragging : PlayerSkill
{
    //public LayerMask clippingAvoidanceLayerMask;
    public LayerMask draggingLayerMask;

    [ReadOnly]
    public GameObject draggingObject = null;
    private Rigidbody draggingBody = null;
    [ReadOnly]
    public bool IsDragging = false;
    public float DistanceFromCamera = 3.0f;
    public float DraggingSpeed = 2.0f;
    public float AvoidanceDistance = 1f;
    public float MaxVelocity = 10f;
    private Camera mainCamera;

    [Header("Particles")]
    public ParticleSystem draggingParticleSystem;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsDragging)
            return;
        Debug.Log("Dragging");
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

        //if (IsDragging && draggingObject != null && Input.GetMouseButton(1))
        //{
        //    draggingBody.transform.rotation = Quaternion.identity;
        //    draggingBody.rotation = Quaternion.identity;
        //    draggingBody.angularVelocity = Vector3.zero;
        //    Debug.Log("Right Mouse button on draggign");
        //}

        //if (IsDragging && Input.GetMouseButtonDown(0))
        //{
        //    Outline outline = draggingBody.gameObject.AddComponent<Outline>();
        //    outline.OutlineMode = Outline.Mode.OutlineAll;
        //    outline.enabled = true;
        //    outline.OutlineColor = Color.white;
        //    outline.OutlineWidth = 1;
        //}

        //if (IsDragging && Input.GetMouseButtonUp(0))
        //{
        //    Outline outline = draggingBody.GetComponent<Outline>();
        //    outline.enabled = false;
        //    Destroy(outline);
        //    draggingBody.interpolation = RigidbodyInterpolation.None;
        //    draggingBody.isKinematic = false;
        //    IsDragging = false;
        //    draggingObject = null;
        //    draggingBody = null;
        //}
    }

    public override void ShowPreview()
    {
        UIManager.Instance.ShowSkillTooltip(TooltipText, MouseSide.LeftClick);
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
        if (DraggableManager.Instance.DraggableObject == null)
            return false;

        IsDragging = true;

        if (DraggableManager.Instance.DraggableObject.GetComponent<Rigidbody>() == null)
            DraggableManager.Instance.DraggableObject.AddComponent<Rigidbody>();

        draggingObject = DraggableManager.Instance.DraggableObject.gameObject;
        draggingBody = draggingObject.GetComponent<Rigidbody>();

        return false;
    }

    public void OnDraggingShoot(InputAction.CallbackContext callback)
    {
        if (!callback.performed || IsDragging == false)
            return;
        Debug.Log("OnDraggingShoot Performed");

        // Performed
        //IsDragging = false;
    }
}
