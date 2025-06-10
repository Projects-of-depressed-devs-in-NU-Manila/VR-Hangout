using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class VoiceNetworkManager : MonoBehaviour
{
    public static VoiceNetworkManager Instance {get; private set;}

    public Dictionary<string, PacketDecoder> decoders; 

    public float tickInterval = 5f; 

    private VoiceWebsocketClient ws;
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
        NetworkManager.Instance.onConnect += ConnectToVoice;
    }

    void ConnectToVoice()
    {
        decoders = new Dictionary<string, PacketDecoder>();
        ws = new VoiceWebsocketClient();
        _ = ws.Connect($"ws://localhost:8000/game/voice?player_id={PlayerContext.Instance.playerId}"); // underscore just means run in the background without awaiting it
        ws.addRecievedCallback(OnDataRecieved);
        MultiplayerMovementManager.Instance.onPlayerSpawn += onPlayerSpawn;
    }

    void onPlayerSpawn(string player_id, GameObject gameObject)
    {
        PacketDecoder decoder = gameObject.GetComponent<PacketDecoder>();
        decoders.Add(player_id, decoder);
    }

    void OnDestroy()
    {
        _ = ws?.Disconnect();
    }

    public void Broadcast(byte[] data, int length){
        if(ws != null)
            _ = ws.Broadcast(data, length);
    }

    void OnDataRecieved(VoicePacket packet){
        decoders[packet.playerId].QueuePacket(packet);
    }
}
