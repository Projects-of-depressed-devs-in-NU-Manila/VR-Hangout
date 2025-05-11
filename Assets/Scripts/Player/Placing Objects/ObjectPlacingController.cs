using Unity.VisualScripting;
using UnityEngine;

public class ObjectPlacingController : MonoBehaviour
{
    public enum State
    {
        Default,
        Placing,
        Rotating,
    }

    public float maxRayDistance = 3f;
    public float objectRotationSpeed = 3f;
    public float objectScalingFactor = 2f;
    public State state = State.Default;

    public Transform playerCamera = null;
    public WorldObjectScriptableObject currentObjectSelected = null;

    public Material placingMaterial = null;
    public Material rotatingMaterial = null;
    public Material previewObjectOriginalMaterial = null;

    private GameObject previewObject = null;
    private float objectHeight = 0;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            setState(State.Default);
            setPreviewVisibility(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (previewObject == null) 
            {
                setPreviewObject(Instantiate(currentObjectSelected.prefab));
                setState(State.Placing);
                LayerMaskUtils.SetLayerRecursively(previewObject, LayerMask.NameToLayer("PendingWorldObject"));
                return;
            }
        }

        switch (state)
        {
            case State.Default:
                HandleDefault();
                return;
            case State.Placing:
                HandlePlacing();
                HandleScaling();
                break;
            case State.Rotating:
                HandleRotating();
                HandleScaling();
                break;
        }
    }

    private void HandleDefault()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LayerMask layerMask = LayerMask.GetMask("WorldObject");
            var ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hitInfo;

            if(Physics.Raycast(ray, out hitInfo, maxRayDistance, layerMask))
            {
                hitInfo.transform.parent = null;
                setPreviewObject(hitInfo.transform.gameObject);
                LayerMaskUtils.SetLayerRecursively(previewObject, LayerMask.NameToLayer("PendingWorldObject"));
                setState(State.Placing);
            }
        }
    }

    private void HandlePlacing()
    {
        LayerMask layerMask = ~LayerMask.GetMask("PendingWorldObject");
        var ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo, maxRayDistance, layerMask))
        {
            setPreviewVisibility(true);
            previewObject.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + (objectHeight/2), hitInfo.point.z); // we add half the height of object for plaing in surfaces
        }
        else
        {
            setPreviewVisibility(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            setState(State.Rotating);
        }
    }

    private void HandleRotating()
    {
        if (Input.GetMouseButton(1)) 
        {
            float mouseX = Input.GetAxis("Mouse X");
            previewObject.transform.Rotate(Vector3.up, -mouseX * objectRotationSpeed, Space.World);
        }
        else if (Input.GetMouseButtonDown(0)) 
        {
            finalizeObject();
            setState(State.Default);
        }
    }

    private void HandleScaling()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            if ((previewObject.transform.localScale + Vector3.one * scroll * objectScalingFactor).x  < 0)
            {
                return;
            } 
            previewObject.transform.localScale += Vector3.one * scroll * objectScalingFactor;
            objectHeight = getPreviewObjectHeight();
        }
    }

    private void setState(State newState)
    {
        Debug.Log(newState.ToString());
        switch (newState)
        {
            case State.Default:
                break;
            case State.Placing:
                if(previewObject != null)
                    previewObject.GetComponentInChildren<Renderer>().material = placingMaterial;
                break;
            case State.Rotating:
                if(previewObject != null)
                    previewObject.GetComponentInChildren<Renderer>().material = rotatingMaterial;
                break;
        }

        state = newState;
    }

    private void setPreviewObject(GameObject obj)
    {
        previewObject = obj;
        objectHeight = getPreviewObjectHeight();
        previewObjectOriginalMaterial = previewObject.GetComponentInChildren<Renderer>().material;
    } 

    private void setPreviewVisibility(bool visibility)
    {
        if (previewObject != null)
            previewObject.SetActive(visibility);
    }

    private void finalizeObject()
    {
        previewObject.GetComponentInChildren<Renderer>().material = previewObjectOriginalMaterial;
        WorldManager.Instance.addObject(previewObject);
        LayerMaskUtils.SetLayerRecursively(previewObject, LayerMask.NameToLayer("WorldObject"));
        previewObject = null;
        previewObjectOriginalMaterial = null;
    }

    float getPreviewObjectHeight()
    {
        return previewObject.GetComponentInChildren<Renderer>().bounds.size.y;
    }

}
