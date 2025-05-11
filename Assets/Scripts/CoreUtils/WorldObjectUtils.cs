using System;
using UnityEngine;

class WorldObjectUtils{

    private const string basePath = "WorldObjects/";

    public static WorldObjectScriptableObject LoadScriptableObject(string id){
        WorldObjectScriptableObject SO= Resources.Load<WorldObjectScriptableObject>($"{basePath + id}");

        if(SO == null){
            throw new Exception($"Cannot find World Object Scriptable Object {basePath + id}");
        }

        return SO;
    }

}