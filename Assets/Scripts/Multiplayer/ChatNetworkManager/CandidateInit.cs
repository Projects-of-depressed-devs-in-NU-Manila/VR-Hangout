using UnityEngine;
using System;

[Serializable]
public class CandidateInit : IJsonObject<CandidateInit>
{
    public string Candidate;
    public string SdpMid;
    public int SdpMLineIndex;

    public static CandidateInit FromJSON(string json)
    {
        return JsonUtility.FromJson<CandidateInit>(json);
    }

    public string ConvertToJSON()
    {
        return JsonUtility.ToJson(this);
    }
}
