using Unity.WebRTC;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WebRTCManager : MonoBehaviour
{
    public static WebRTCManager Instance { get; private set; }

    private RTCPeerConnection peerConnection;
    private string remotePlayerId;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartConnection(string targetPlayerId)
    {
        remotePlayerId = targetPlayerId;
        peerConnection = new RTCPeerConnection();

        peerConnection.OnIceCandidate = candidate =>
        {
            var msg = new Dictionary<string, object> {
                {"type", "webrtc-ice-candidate"},
                {"to", remotePlayerId},
                {"from", PlayerContext.Instance.playerId},
                {"candidate", candidate.ToString()}
            };
            NetworkManager.Instance.Broadcast(JsonHelper.ToJson(msg));
        };

        StartCoroutine(CreateAndSendOffer());
    }

    private IEnumerator CreateAndSendOffer()
    {
        var offerOp = peerConnection.CreateOffer();
        yield return offerOp;

        var offer = offerOp.Desc;
        var setLocalOp = peerConnection.SetLocalDescription(ref offer);
        yield return new WaitUntil(() => setLocalOp.IsDone);

        if (setLocalOp.IsError)
        {
            Debug.LogError("Failed to set local description: " + setLocalOp.Error.message);
            yield break;
        }

        var msgOffer = new Dictionary<string, object> {
            {"type", "webrtc-offer"},
            {"to", remotePlayerId},
            {"from", PlayerContext.Instance.playerId},
            {"sdp", offer.sdp}
        };
        NetworkManager.Instance.Broadcast(JsonHelper.ToJson(msgOffer));
    }

    public void OnOfferReceived(Dictionary<string, object> dataJson)
    {
        remotePlayerId = (string)dataJson["from"];
        peerConnection = new RTCPeerConnection();

        peerConnection.OnIceCandidate = candidate =>
        {
            var msg = new Dictionary<string, object> {
                {"type", "webrtc-ice-candidate"},
                {"to", remotePlayerId},
                {"from", PlayerContext.Instance.playerId},
                {"candidate", candidate.ToString()}
            };
            NetworkManager.Instance.Broadcast(JsonHelper.ToJson(msg));
        };

        StartCoroutine(HandleOffer(dataJson["sdp"].ToString()));
    }

    private IEnumerator HandleOffer(string sdp)
    {
        var desc = new RTCSessionDescription
        {
            type = RTCSdpType.Offer,
            sdp = sdp
        };

        var setRemoteOp = peerConnection.SetRemoteDescription(ref desc);
        yield return new WaitUntil(() => setRemoteOp.IsDone);

        if (setRemoteOp.IsError)
        {
            Debug.LogError("SetRemoteDescription failed: " + setRemoteOp.Error.message);
            yield break;
        }

        var answerOp = peerConnection.CreateAnswer();
        yield return answerOp;

        var answer = answerOp.Desc;
        var setLocalOp = peerConnection.SetLocalDescription(ref answer);
        yield return new WaitUntil(() => setLocalOp.IsDone);

        if (setLocalOp.IsError)
        {
            Debug.LogError("SetLocalDescription failed: " + setLocalOp.Error.message);
            yield break;
        }

        var msgAnswer = new Dictionary<string, object> {
            {"type", "webrtc-answer"},
            {"to", remotePlayerId},
            {"from", PlayerContext.Instance.playerId},
            {"sdp", answer.sdp}
        };
        NetworkManager.Instance.Broadcast(JsonHelper.ToJson(msgAnswer));
    }

    public void OnAnswerReceived(Dictionary<string, object> dataJson)
    {
        var desc = new RTCSessionDescription
        {
            type = RTCSdpType.Answer,
            sdp = dataJson["sdp"].ToString()
        };

        StartCoroutine(SetRemoteAnswer(desc));
    }

    private IEnumerator SetRemoteAnswer(RTCSessionDescription desc)
    {
        var setRemoteOp = peerConnection.SetRemoteDescription(ref desc);
        yield return new WaitUntil(() => setRemoteOp.IsDone);

        if (setRemoteOp.IsError)
        {
            Debug.LogError("Failed to set remote answer: " + setRemoteOp.Error.message);
        }
        else
        {
            Debug.Log("Remote answer set successfully.");
        }
    }

    public void OnIceCandidateReceived(Dictionary<string, object> dataJson)
    {
        var candidateStr = dataJson["candidate"].ToString();
        var candidate = new RTCIceCandidate(new RTCIceCandidateInit { candidate = candidateStr });
        peerConnection.AddIceCandidate(candidate);
    }
}
