using System.Collections.Generic;
using UnityEngine;

// Sends the position of the player to the server.
// Attach this to the player object
public class PlayerTransformSyncer : MonoBehaviour
{
    private float sendInterval = 1f / 10;
    private float sendTimer = 0;

    private Vector3 lastSentPosition; 
    private float positionThreshold = 0.1f;

    void Update()
    {
        sendTimer += Time.deltaTime;

        if (sendTimer >= sendInterval){
            if (Vector3.Distance(transform.position, lastSentPosition) > positionThreshold){
                PlayerMoveMessage message = new PlayerMoveMessage()
                {
                    type = "playerMove",
                    playerId = PlayerContext.Instance.playerId,
                    position = transform.position,
                    rotation = transform.rotation.eulerAngles
                };

                NetworkManager.Instance.Broadcast(JsonHelper.ToJson(message));
                lastSentPosition = transform.position;
            }
        }
    }
}
