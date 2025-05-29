using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class DataChannelService : WebSocketBehavior
{
    protected override void OnOpen()
    {
        Debug.Log($"{ID} connected");
        foreach (var id in Sessions.ActiveIDs)
        {
            if (id != ID)
            {
                Sessions.SendTo($"NEW_PEER!{ID}!New peer connected", id);
            }
        }
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        var message = new SignalingMessageChannel(e.Data);
        if (message.type != SignalingMessageType.OTHER)
        {
            Sessions.SendTo(e.Data, message.channelId.ToString());
        }
        else
        {
            Debug.Log($"Unknown message format: {e.Data}");
        }
    }

    protected override void OnClose(CloseEventArgs e)
    {
        Debug.Log($"{ID} disconnected");
        foreach (var id in Sessions.ActiveIDs)
        {
            Sessions.SendTo($"PEER_DISCONNECTED!{ID}!Peer disconnected", id);
        }
    }
}