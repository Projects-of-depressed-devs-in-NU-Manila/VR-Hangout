
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct WorldObject
{
    public string worldObjectId { get; set; }
    public string worldId { get; set; }
    public string objectId { get; set; }
    public Vector3 position { get; set; }
    public Vector3 rotation { get; set; }
    public Vector3 scale { get; set; }

    public static WorldObject FromGameObject(GameObject obj)
    {
        WorldObjectComponent component = obj.GetComponent<WorldObjectComponent>();
        WorldObject worldObject = new WorldObject();
        worldObject.objectId = component.objectId;
        worldObject.worldObjectId = component.worldObjectId;
        worldObject.worldId = component.worldId;

        worldObject.position = obj.transform.position;
        worldObject.rotation = obj.transform.eulerAngles;
        worldObject.scale = obj.transform.lossyScale;
        return worldObject;
    }    
}

public struct WorldData
{
    public string type { get; set; }
    public string worldId;
    public List<WorldObject> objects { get; set; }
}


