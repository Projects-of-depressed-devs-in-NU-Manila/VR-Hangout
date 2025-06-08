using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;

    public string worldId;
    public GameObject HubPrefab;
    private Dictionary<string, GameObject> worldObjects;
    private event Action worldStartedLoading;
    private event Action worldFinishedLoading;

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
        worldObjects = new Dictionary<string, GameObject>();
        NetworkManager.Instance.onWorldObjectLoad += LoadWorld;
        NetworkManager.Instance.onGoToHub += GoToHub;
    }

    public void ClearWorld()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        worldObjects.Clear();
    }

    public void GoToHub()
    {
        ClearWorld();
        isLoading = true;
        worldStartedLoading?.Invoke();
        Debug.Log("Instantiating hub");
        Instantiate(HubPrefab, transform);
        isLoading = false;
        worldFinishedLoading?.Invoke();
    }

    public void LoadWorld(WorldData data)
    {
        worldStartedLoading?.Invoke();
        isLoading = true;
        ClearWorld();

        Debug.Log($"Loading world: {data.worldId}");
        isLoading = true;
        worldId = data.worldId;

        if (worldId == "1b7Tmo6Yj0")
        {
            Debug.Log("Breakpoint");
        }

        foreach (WorldObject worldObject in data.objects)
        {
            GameObject prefab = WorldObjectUtils.LoadPrefab(worldObject.objectId);
            GameObject obj = Instantiate(prefab, transform);
            worldObjects.Add(worldObject.worldObjectId, obj);

            if (worldObject.objectId[0] != '_')
            {
                LayerMaskUtils.SetLayerRecursively(obj, LayerMask.NameToLayer("WorldObject"));
            }

            obj.transform.position = worldObject.position;
            obj.transform.localRotation = Quaternion.Euler(worldObject.rotation.x, worldObject.rotation.y, worldObject.rotation.z);
            Debug.Log("Adding scale....");

            obj.transform.localScale = worldObject.scale;

            WorldObjectComponent component = obj.GetComponent<WorldObjectComponent>();
            if (component == null)
                component = obj.AddComponent<WorldObjectComponent>();
            component.worldId = worldObject.worldId;
            component.worldObjectId = worldObject.worldObjectId;
            component.objectId = worldObject.objectId;
        }
        worldFinishedLoading?.Invoke();
        isLoading = false;
        Debug.Log($"Loaded {data.objects.Count} Objects");
    }

    public void AddObject(GameObject obj)
    {
        obj.transform.parent = transform;

        WorldData addWorldData = new WorldData();
        addWorldData.type = "addWorldObjects";
        addWorldData.worldId = worldId;
        addWorldData.objects = new List<WorldObject>();

        WorldData editWorldData = new WorldData();
        editWorldData.type = "editWorldObjects";
        editWorldData.worldId = worldId;
        editWorldData.objects = new List<WorldObject>();

        try
        {
            obj.GetComponent<WorldObjectComponent>().worldId = worldId;
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }

        WorldObject worldObject = WorldObject.FromGameObject(obj);

        if (worldObject.worldObjectId == "")
        {
            addWorldData.objects.Add(worldObject);
            NetworkManager.Instance.Broadcast(JsonHelper.ToJson(addWorldData));
        }
        else
        {
            editWorldData.objects.Add(worldObject);
            NetworkManager.Instance.Broadcast(JsonHelper.ToJson(editWorldData));
        }
    }


    void OnDestroy()
    {
        NetworkManager.Instance.onWorldObjectLoad -= LoadWorld;
        NetworkManager.Instance.onGoToHub -= GoToHub;
    }
    
}
