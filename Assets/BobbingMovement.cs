using UnityEngine;

public class BobbingMovement : MonoBehaviour
{
    [Header("Bob Settings")]
    public float bobSpeed = 2f;       // Speed of the up and down motion
    public float bobHeight = 0.25f;   // How high it moves up and down

    [Header("Rotation Settings")]
    public float rotationSpeed = 50f; // Degrees per second

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Bobbing motion
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotation
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    
}
