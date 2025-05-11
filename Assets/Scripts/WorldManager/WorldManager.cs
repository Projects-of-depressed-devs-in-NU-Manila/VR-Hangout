using System;
using System.ComponentModel;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;

    private string worldId;
    public bool isLoading = true;

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
        NetworkManager.Instance.onWorldDataRecieved += LoadWorld;
    }

    void Update()
    {
        
    }

    public void LoadWorld(WorldData data){
        Debug.Log($"Loading world: {data.worldId}");
        isLoading = true;
        worldId = data.worldId;
        foreach(WorldObject worldObject in data.objects){
            WorldObjectScriptableObject SO = WorldObjectUtils.LoadScriptableObject(worldObject.objectId);
            GameObject obj = Instantiate(SO.prefab, transform);

            obj.transform.position = worldObject.position;
            obj.transform.localRotation = Quaternion.Euler(worldObject.rotation.x, worldObject.rotation.y, worldObject.rotation.z);
            obj.transform.localScale = worldObject.scale;
        }
        isLoading = true;
        Debug.Log($"Loaded {data.objects.Count} Objects");
    }

    public void addObject(GameObject obj)
    {
        obj.transform.parent = transform;
    }


    void OnDestroy()
    {
        NetworkManager.Instance.onWorldDataRecieved -= LoadWorld;
    }
}
