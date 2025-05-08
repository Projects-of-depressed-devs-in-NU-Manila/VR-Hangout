using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance {get; private set;}

    private WebsocketClient ws;

    void Awake()
    {
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ws = new WebsocketClient();
        _ = ws.Connect("ws://localhost:8000/game/ws?user_id=123"); // underscore just means run in the background without awaiting it
        ws.addRecievedCallback(onDataRecieved);
        
    }

    void onDataRecieved(Dictionary<string, object> data){
        Debug.Log(data["type"]);
        Debug.Log(DictToStr(data));
    }

    string DictToStr(Dictionary<string, object> dict){
        return JsonConvert.SerializeObject(dict); 

    }
}
