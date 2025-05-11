using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance {get; private set;}

    public float tickInterval = 5f; 

    public event Action<ConnectionMessage> onOtherPlayerConnect;
    public event Action<DisconnectionMessage> onOtherPlayerDisconnect;
    public event Action<PlayerMoveMessage> onOtherPlayerMove;
    public event Action<WorldData> onWorldDataRecieved;

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
        _ = ws.Connect($"ws://localhost:8000/game/ws?player_id={PlayerContext.Instance.playerId}"); // underscore just means run in the background without awaiting it
        ws.addRecievedCallback(OnDataRecieved);
        
    }

    void OnDestroy()
    {
        _ = ws.Disconnect();
    }

    public void Broadcast(string json){
        _ = ws.Broadcast(json);
    }

    void OnDataRecieved(Dictionary<string, object> dataJson, string dataStr){
        string type = (string)dataJson["type"];

        try{
            switch(type){
                case "playerConnect":
                    onOtherPlayerConnect?.Invoke(JsonHelper.FromJson<ConnectionMessage>(dataStr));
                    break;
                case "playerDisconnect":
                    onOtherPlayerDisconnect?.Invoke(JsonHelper.FromJson<DisconnectionMessage>(dataStr));
                    break;
                case "playerMove":
                    onOtherPlayerMove?.Invoke(JsonHelper.FromJson<PlayerMoveMessage>(dataStr));
                    break;
                case "loadWorld":
                    onWorldDataRecieved?.Invoke(JsonHelper.FromJson<WorldData>(dataStr));
                    break;
            }
        } catch (Exception e){
            Debug.Log(e);
        }
    }
}
