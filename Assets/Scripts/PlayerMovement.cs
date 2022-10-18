using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Vector3 moveVal;
    [SerializeField] private float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        this.transform.position += moveSpeed * Time.deltaTime * moveVal;
    }

    public void OnMove(InputValue value)
    {
        Vector2 inputVector = value.Get<Vector2>();

        moveVal = new Vector3(inputVector.x, 0, inputVector.y);
    }

}
