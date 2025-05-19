using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;

    private string worldId;
    private Dictionary<string, GameObject> worldObjects;

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
        NetworkManager.Instance.onWorldObjectLoad += LoadWorld;
    }

    void Update()
    {
        
    }

    public void LoadWorld(WorldData data){
        Debug.Log($"Loading world: {data.worldId}");
        isLoading = true;
        worldId = data.worldId;
        foreach(WorldObject worldObject in data.objects){
            GameObject prefab = WorldObjectUtils.LoadPrefab(worldObject.objectId);
            GameObject obj = Instantiate(prefab, transform);
            worldObjects.Add(worldObject.worldObjectId, obj);

            if(worldObject.objectId[0] != '_'){
                LayerMaskUtils.SetLayerRecursively(obj, LayerMask.NameToLayer("WorldObject"));
            }

            obj.transform.position = worldObject.position;
            obj.transform.localRotation = Quaternion.Euler(worldObject.rotation.x, worldObject.rotation.y, worldObject.rotation.z);
            obj.transform.localScale = worldObject.scale;

            WorldObjectComponent component = obj.AddComponent<WorldObjectComponent>();
            component.worldId = worldObject.worldId;
            component.worldObjectId = worldObject.worldObjectId;
            component.objectId = worldObject.objectId;
        }
        isLoading = true;
        Debug.Log($"Loaded {data.objects.Count} Objects");
    }

    public void AddObject(GameObject obj)
    {
        obj.transform.parent = transform;

        WorldData worldData = new WorldData();
        worldData.type = "addWorldObjects";
        worldData.worldId = worldId;

        WorldObject worldObject = WorldObject.FromGameObject(obj);

        worldData.objects.Add(worldObject);

        NetworkManager.Instance.Broadcast(JsonHelper.ToJson(worldData));
    }


    void OnDestroy()
    {
        NetworkManager.Instance.onWorldObjectLoad -= LoadWorld;
    }
    
}
