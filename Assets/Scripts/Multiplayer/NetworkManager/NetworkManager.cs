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
    public event Action<WorldData> onWorldObjectLoad;
    public event Action<WorldData> onWorldObjectAdded;
    public event Action<WorldData> onWorldObjectEditted;
    public event Action<Chat> OnChatRecieved;
    public event Action<PlayerAnimation> OnAnimationSync;
    public event Action onGoToHub;
    public event Action onConnect;

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

    async void Start()
    {
        ws = new WebsocketClient();
        await ws.Connect($"ws://vrhangout.cottonbuds.dev/game/ws?player_id={PlayerContext.Instance.playerId}"); // underscore just means run in the background without awaiting it
        ws.addRecievedCallback(OnDataRecieved);
        onConnect?.Invoke();
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
        try
        {
            switch (type)
            {
                case "playerConnect":
                    onOtherPlayerConnect?.Invoke(JsonHelper.FromJson<ConnectionMessage>(dataStr));
                    break;
                case "playerDisconnect":
                    onOtherPlayerDisconnect?.Invoke(JsonHelper.FromJson<DisconnectionMessage>(dataStr));
                    break;
                case "playerMove":
                    onOtherPlayerMove?.Invoke(JsonHelper.FromJson<PlayerMoveMessage>(dataStr));
                    break;
                case "loadWorldObjects":
                    onWorldObjectLoad?.Invoke(JsonHelper.FromJson<WorldData>(dataStr));
                    break;
                case "addWorldObjects":
                    onWorldObjectAdded?.Invoke(JsonHelper.FromJson<WorldData>(dataStr));
                    break;
                case "editWorldObjects":
                    onWorldObjectEditted?.Invoke(JsonHelper.FromJson<WorldData>(dataStr));
                    break;
                case "goToHub":
                    Debug.Log("Sending go to hub events");
                    onGoToHub?.Invoke();
                    break;
                case "chat":
                    OnChatRecieved?.Invoke(JsonHelper.FromJson<Chat>(dataStr));
                    break;
                case "animationSync":
                    OnAnimationSync?.Invoke(JsonHelper.FromJson<PlayerAnimation>(dataStr));
                    break;

            }
        }
        catch (Exception e)
        {
            Debug.Log("Error:" + e);
        }
    }
}
