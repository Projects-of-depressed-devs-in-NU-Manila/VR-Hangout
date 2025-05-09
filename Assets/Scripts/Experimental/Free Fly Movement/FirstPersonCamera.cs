using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    public Transform playerCamera = null;
    public float lookSensitivity = 2.0f;
    private float rotationX = 0f;

    void Start()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }

    }
}

