using System;
using UnityEngine;

[Serializable]
public class SessionDescription : IJsonObject<SessionDescription>
{
    public string SessionType;
    public string Sdp;


    public static SessionDescription FromJSON(string json)
    {
        return JsonUtility.FromJson<SessionDescription>(json);
    }
    public string ConvertToJSON()
    {
        return JsonUtility.ToJson(this);
    }
}