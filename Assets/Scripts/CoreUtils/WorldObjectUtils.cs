using System;
using UnityEngine;

class WorldObjectUtils{

    private const string basePath = "WorldObjects/Prefabs/";

    public static GameObject LoadPrefab(string id){
        GameObject prefab = Resources.Load<GameObject>($"{basePath + id}");

        if(prefab == null){
            throw new Exception($"Cannot find World Object {basePath + id}");
        }
        return prefab;
    }
}