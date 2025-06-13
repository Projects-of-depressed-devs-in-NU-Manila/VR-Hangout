using System;
using UnityEngine;

class CharacterUtils{

    private const string basePath = "Characters/";

    public static GameObject LoadPrefab(string id){
        GameObject prefab = Resources.Load<GameObject>($"{basePath + id}");

        if(prefab == null){
            throw new Exception($"Cannot find Character {basePath + id}");
        }
        return prefab;
    }
}