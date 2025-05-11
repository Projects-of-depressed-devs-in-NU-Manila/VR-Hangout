
using System.Collections.Generic;
using UnityEngine;

public class WorldObject
{
    public string worldObjectId { get; set; }
    public string worldId { get; set; }
    public string objectId { get; set; }
    public Vector3 position { get; set; }
    public Vector3 rotation { get; set; }
    public Vector3 scale { get; set; }
}

public class WorldData
{
    public string type { get; set; }
    public string worldId;
    public List<WorldObject> objects { get; set; }
}

