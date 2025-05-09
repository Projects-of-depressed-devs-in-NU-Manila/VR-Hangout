using System;
using System.IO;
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

    private List<Action<Dictionary<string, object>, string>> recievedCallbacks;

     public async Task Connect(string url){
        ws = new ClientWebSocket();
        cts = new CancellationTokenSource();
        recievedCallbacks = new List<Action<Dictionary<string, object>, string>>();

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

    public async Task Disconnect(){
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed Socket No reason", cts.Token);
    }

    public async Task Broadcast(string json)
    {
        var encoded = Encoding.UTF8.GetBytes(json);
        var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
        await ws.SendAsync(buffer, WebSocketMessageType.Text, true, cts.Token);
    }

    public async Task StartRecieveLoop()
    {
        var buffer = new byte[2048];
        var memoryStream = new MemoryStream();

        while (ws.State == WebSocketState.Open)
        {
            memoryStream.SetLength(0); // reset the stream

            WebSocketReceiveResult result;

            do
            {
                result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
                memoryStream.Write(buffer, 0, result.Count);
            }
            while (!result.EndOfMessage);

            memoryStream.Seek(0, SeekOrigin.Begin);
            string dataStr = Encoding.UTF8.GetString(memoryStream.ToArray());

            Dictionary<string, object> dataJson = StrToDict(dataStr);

            foreach (var callback in recievedCallbacks)
            {
                callback.Invoke(dataJson, dataStr);
            }
        }

        Debug.LogError("Connection to server closed");
        Application.Quit();
    }

    private Dictionary<string, object> StrToDict(string dataStr){
        try
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(dataStr);
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to parse message: " + dataStr);
            Console.WriteLine("Error: " + ex.Message);
            throw new Exception("Error while parsing JSOn");
        }
    }


    public void addRecievedCallback(Action<Dictionary<string, object>, string> action){
        recievedCallbacks.Add(action);
    }
}