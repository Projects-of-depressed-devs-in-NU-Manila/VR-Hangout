using System.Collections.Generic;
using UnityEngine;

public class PlayerTransformSyncer : MonoBehaviour
{
    private float sendInterval = 1f / NetworkManager.Instance.tickInterval;
    private float sendTimer = 0;

    private Vector3 lastSentPosition; 
    private float positionThreshold = 0.1f;

    void Update()
    {
        sendTimer += Time.deltaTime;

        if (sendTimer >= sendInterval){
            if (Vector3.Distance(transform.position, lastSentPosition) > positionThreshold){
                PlayerMoveMessage message = new PlayerMoveMessage(){
                    type = "playerMove", 
                    playerId=PlayerContext.Instance.playerId, 
                    position=transform.position
                };

                NetworkManager.Instance.Broadcast(JsonHelper.ToJson(message));
            }
        }
    }
}
