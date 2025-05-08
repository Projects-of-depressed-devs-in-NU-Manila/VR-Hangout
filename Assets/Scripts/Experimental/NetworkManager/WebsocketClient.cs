using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using Newtonsoft.Json;
using System.Collections.Generic;

public class WebsocketClient{
    private ClientWebSocket ws;
    private CancellationTokenSource cts;

    private List<Action<Dictionary<string, object>>> recievedCallbacks;

     public async Task Connect(string url){
        ws = new ClientWebSocket();
        cts = new CancellationTokenSource();
        recievedCallbacks = new List<Action<Dictionary<string, object>>>();

        try
        {
            await ws.ConnectAsync(new Uri(url), cts.Token);
            Debug.Log("Connected to server!");

            _ = StartRecieveLoop(); // underscore just means run in the background without awaiting it 

        }
        catch (Exception e)
        {
            Debug.LogError("Connection error: " + e.Message);
            throw(e);
        }
    }

    public async Task Broadcast(Dictionary<string, object> message)
    {
        var encoded = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
        await ws.SendAsync(buffer, WebSocketMessageType.Text, true, cts.Token);
    }

    public async Task StartRecieveLoop()
    {

        var buffer = new byte[1024];
        while (ws.State == WebSocketState.Open)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
            string str = Encoding.UTF8.GetString(buffer, 0, result.Count);

            Dictionary<string, object> parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(str);

            foreach(Action<Dictionary<string, object>> callback in recievedCallbacks){
                callback.Invoke(parsed);
            }
        }
    }

    public void addRecievedCallback(Action<Dictionary<string, object>> action){
        recievedCallbacks.Add(action);
    }
}