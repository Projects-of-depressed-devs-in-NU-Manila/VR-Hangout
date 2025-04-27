using UnityEngine;

public class FreeFlyMovement : MonoBehaviour
{
    public float lookSensitivity = 2.0f;
    public float moveSpeed = 10.0f;
    public float boostMultiplier = 3.0f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // Lock the cursor at start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Always active movement and looking

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        rotationX -= mouseY;
        rotationY += mouseX;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);

        // Movement
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= boostMultiplier;

        Vector3 move = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
        );

        if (Input.GetKey(KeyCode.E)) move.y += 1; // Up
        if (Input.GetKey(KeyCode.Q)) move.y -= 1; // Down

        transform.Translate(move.normalized * speed * Time.deltaTime, Space.Self);
    }
}

