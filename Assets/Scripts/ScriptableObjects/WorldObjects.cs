using UnityEngine;

[CreateAssetMenu(fileName = "WorldObjects", menuName = "Scriptable Objects/WorldObjects")]
public class WorldObjects : ScriptableObject
{
    public string objectId;
    public string objectName; 
    public GameObject prefab;
    public int price;
    public Vector3 minScale;
    public Vector3 maxScale;
}
