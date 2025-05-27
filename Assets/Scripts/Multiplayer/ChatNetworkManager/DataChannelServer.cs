using UnityEngine;
using WebSocketSharp.Server;
using System.Net;
using System.Net.Sockets;

public class DataChannelServer : MonoBehaviour
{
    private WebSocketServer wssv;
    private string serverIpv4Address;
    private int serverPort = 8080;

    private void Awake()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                serverIpv4Address = ip.ToString();
                break;
            }
        }

        //Debug.Log("Server IP: " + serverIpv4Address);

        wssv = new WebSocketServer($"ws://{serverIpv4Address}:{serverPort}");
        wssv.AddWebSocketService<DataChannelService>($"/{nameof(DataChannelService)}");
        wssv.Start();
    }
}
