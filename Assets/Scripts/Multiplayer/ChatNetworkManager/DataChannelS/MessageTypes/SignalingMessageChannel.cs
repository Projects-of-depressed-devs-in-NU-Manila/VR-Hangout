using UnityEngine;
using System;

public class SignalingMessageChannel
{
    public readonly SignalingMessageType type;
    public readonly string channelId;
    public readonly string message;

    public SignalingMessageChannel(string messageStr)
    {
        var messageArray = messageStr.Split('!');

        if (messageArray.Length < 3)
        {
            type = SignalingMessageType.OTHER;
            channelId = "";
            message = messageStr;
        }
        else if (Enum.TryParse(messageArray[0], out SignalingMessageType resultType))
        {
            type = resultType;
            channelId = messageArray[1];
            message = messageArray[2];
        }
        else
        {
            type = SignalingMessageType.OTHER;
            channelId = "";
            message = messageStr;
        }
    }
}