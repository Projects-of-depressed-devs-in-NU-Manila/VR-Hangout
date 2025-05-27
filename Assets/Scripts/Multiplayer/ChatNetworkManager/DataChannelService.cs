using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class DataChannelService : WebSocketBehavior
{

    protected override void OnOpen()
    {
        Debug.Log("Server has Started!");
    }
    protected override void OnMessage(MessageEventArgs e)
    {
        Debug.Log(ID + ": DataChannel server has message " + e.Data);

        foreach(var id in Sessions.ActiveIDs)
        {
            if (id != ID)
            {
                Sessions.SendTo(e.Data, id);
            }
        }
    }
}
