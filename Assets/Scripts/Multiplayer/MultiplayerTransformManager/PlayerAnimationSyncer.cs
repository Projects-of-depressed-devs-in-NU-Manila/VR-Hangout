using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationSyncer : MonoBehaviour
{
    private float sendInterval = 1f / 10;
    private float sendTimer = 0;

    private float positionThreshold = 0.1f;
    public bool isWalking;
    public bool isRunning;
    public bool isFishing;

    public bool lastIsWalking;
    public bool lastIsRunning;
    public bool lastIsFishing;

    void Update()
    {
        sendTimer += Time.deltaTime;

        if (sendTimer >= sendInterval)
        {
            if (isWalking == lastIsWalking && isRunning == lastIsRunning && isFishing == lastIsFishing)
            {
                sendTimer = 0f;
                return;
            }

            PlayerAnimation animation = new PlayerAnimation();
            animation.player_id = PlayerContext.Instance.playerId;
            animation.type = "animationSync";
            animation.isWalking = isWalking;
            animation.isRunning = isRunning;
            animation.isFishing = isFishing;

            NetworkManager.Instance.Broadcast(JsonHelper.ToJson(animation));


            lastIsWalking = isWalking;
            lastIsRunning = isRunning;
            lastIsFishing = isFishing;

            sendTimer = 0f;
        }
    }
}
