using UnityEngine;

public class CursorLocker : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
