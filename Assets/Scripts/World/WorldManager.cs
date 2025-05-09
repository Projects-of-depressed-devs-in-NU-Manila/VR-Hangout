using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void addObject(GameObject obj)
    {
        obj.transform.parent = transform;
    }
}
