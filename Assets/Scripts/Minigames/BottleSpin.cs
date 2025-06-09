using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BottleSpin : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.angularDamping = 0.5f;

        rb.constraints = RigidbodyConstraints.FreezePosition |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    OnClick();
                }
            }
        }
    }

    void OnClick()
    {
        Debug.Log("Spinning " + gameObject.name);
        float randomZ = Random.Range(1000f, 2000f);
        rb.AddTorque(Vector3.back * randomZ * 99999f, ForceMode.Impulse);
    }
}
