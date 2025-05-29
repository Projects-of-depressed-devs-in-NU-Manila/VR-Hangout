using UnityEngine;
using WebSocketSharp.Server;
using System.Net;
using System.Net.Sockets;

public class DataChannelServer : MonoBehaviour
{
    private WebSocketServer wssv;
    private string serverIpv4Address;
    private int serverPort = 8082;

    private void Awake()
    {
        serverIpv4Address = GetLocalIPAddress();
        Debug.Log($"Starting server on {serverIpv4Address}:{serverPort}");

        wssv = new WebSocketServer($"ws://0.0.0.0:{serverPort}");
        wssv.Log.Level = WebSocketSharp.LogLevel.Debug;
        
        wssv.AddWebSocketService<DataChannelService>($"/{nameof(DataChannelService)}");
        
        try
        {
            wssv.Start();
            Debug.Log("Server started successfully");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to start server: {ex}");
        }
    }
    
    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "127.0.0.1";
    }

    private void OnDestroy()
    {
        wssv.Stop();
    }
}
