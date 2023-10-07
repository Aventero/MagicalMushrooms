using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WitchLocator : MonoBehaviour
{
    public Transform MainCamera;
    public Transform Witch;
    public GameObject WitchLocatorInUI;
    private RectTransform rectTransform;

    public void Start()
    {
        rectTransform = WitchLocatorInUI.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rectTransform == null)
            return;

        RotateIcon();
    }

    void RotateIcon()
    {
        // 1. Compute the direction from the camera to the enemy.
        Vector3 toEnemy = Witch.position - MainCamera.position;
        Vector3 toEnemy2D = Vector3.ProjectOnPlane(toEnemy, Vector3.up).normalized;
        Vector3 cameraForward2D = Vector3.ProjectOnPlane(MainCamera.forward, Vector3.up).normalized;

        // 2. Compute the angle between the camera's forward vector and the direction vector.
        float angle = -Vector3.SignedAngle(cameraForward2D, toEnemy2D, Vector3.up);

        // 4. Set the rotation of the UI Image
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
