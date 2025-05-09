using UnityEngine;

public struct ConnectionMessage{
    public string type;
    public string playerId;
    public Vector3 position;
}

public struct DisconnectionMessage{
    public string type;
    public string playerId;
}

    