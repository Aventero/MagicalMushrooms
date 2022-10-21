using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Vector3 moveVal;
    [SerializeField] private float moveSpeed;

    public float sensivityX = 50f;
    public float sensivityY = 50f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        characterController = this.GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        moveDirection = new Vector3(moveVal.x, 0f, moveVal.z);
        moveDirection = Camera.main.transform.TransformDirection(moveDirection);
        moveDirection.y = 0;
        moveDirection *= moveSpeed;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    public void OnMove(InputValue value)
    {
        Vector2 inputVector = value.Get<Vector2>().normalized;
        moveVal = new Vector3(inputVector.x, 0, inputVector.y);
    }


    public void OnLook(InputValue value)
    {
        Vector2 inputVector = value.Get<Vector2>();

        float mouseX = inputVector.x * sensivityX;
        float mouseY = inputVector.y * sensivityY;

        xRotation += mouseX;
        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -90, 90);

        Camera.main.transform.eulerAngles = new Vector3(yRotation, xRotation);
    }
}
