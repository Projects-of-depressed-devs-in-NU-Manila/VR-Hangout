using System.Collections;
using System.Collections.Generic;
using Unity.WebRTC;
using UnityEngine;
using WebSocketSharp;

public class WebRTCVoiceChat : MonoBehaviour
{
    [Header("Server Configuration")]
    [SerializeField] private string serverIp = "192.168.1.30";
    [SerializeField] private int serverPort = 8082;
    [SerializeField] private bool autoConnect = true;

    [Header("Audio Configuration")]
    [SerializeField] private int sampleRate = 48000;
    [SerializeField] private int bufferSize = 1024;
    [SerializeField] private bool echoCancellation = true;
    [SerializeField] private bool noiseSuppression = true;
    [SerializeField] private bool automaticGainControl = true;

    [Header("UI Configuration")]
    [SerializeField] private bool showVoiceUI = true;
    [SerializeField] private Vector2 voiceUIPosition = new Vector2(10, 10);
    [SerializeField] private Vector2 voiceUISize = new Vector2(300, 150);

    private Dictionary<string, RTCPeerConnection> peerConnections = new Dictionary<string, RTCPeerConnection>();
    private Dictionary<string, AudioStreamTrack> audioTracks = new Dictionary<string, AudioStreamTrack>();
    private WebSocket signalingSocket;

    private string clientId;
    private AudioStreamTrack localAudioTrack;
    private bool isMuted = false;
    private bool isConnected = false;

    private void Start()
    {
        if (autoConnect)
        {
            ConnectToServer();
        }
    }

    public void ConnectToServer()
    {
        clientId = System.Guid.NewGuid().ToString();
        Debug.Log($"Connecting to signaling server as client {clientId}...");

        signalingSocket = new WebSocket($"ws://{serverIp}:{serverPort}/{nameof(DataChannelService)}");
        
        // Setup event handlers
        signalingSocket.OnOpen += OnSignalingConnected;
        signalingSocket.OnMessage += OnSignalingMessage;
        signalingSocket.OnClose += OnSignalingDisconnected;
        signalingSocket.OnError += OnSignalingError;
        
        signalingSocket.Connect();
    }

    private void OnSignalingConnected(object sender, System.EventArgs e)
    {
        Debug.Log("Connected to signaling server");
        isConnected = true;
        StartLocalAudio();
        signalingSocket.Send($"NEW_PEER!{clientId}!Connected");
    }

    private void StartLocalAudio()
    {
        var audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        localAudioTrack = new AudioStreamTrack(audioSource);
        localAudioTrack.Loopback = false; 
    }

    private void OnSignalingMessage(object sender, MessageEventArgs e)
    {
        try
        {
            var message = new SignalingMessageChannel(e.Data);
            Debug.Log($"Received message of type {message.type} from {message.channelId}");

            switch (message.type)
            {
                case SignalingMessageType.NEW_PEER:
                    if (message.channelId != clientId && !peerConnections.ContainsKey(message.channelId))
                    {
                        CreatePeerConnection(message.channelId);
                    }
                    break;

                case SignalingMessageType.PEER_DISCONNECTED:
                    if (peerConnections.ContainsKey(message.channelId))
                    {
                        CleanupPeer(message.channelId);
                    }
                    break;

                case SignalingMessageType.OFFER:
                    HandleOffer(message.channelId, message.message);
                    break;

                case SignalingMessageType.ANSWER:
                    HandleAnswer(message.channelId, message.message);
                    break;

                case SignalingMessageType.CANDIDATE:
                    HandleCandidate(message.channelId, message.message);
                    break;

                default:
                    Debug.Log($"Unknown message: {e.Data}");
                    break;
            }
        }
        catch (System.Exception er)
        {
            Debug.LogError($"Failed to process message: {e.Data}\nError: {er}");
        }
    }

    private void CreatePeerConnection(string peerId)
    {
        if (peerConnections.ContainsKey(peerId)) return;

        Debug.Log($"Creating peer connection with {peerId}");

        var config = new RTCConfiguration
        {
            iceServers = new[]
            {
                new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } },
                // Add TURN server if needed for NAT traversal
                // new RTCIceServer { 
                //     urls = new[] { "turn:your-turn-server.com" },
                //     username = "user",
                //     credential = "pass"
                // }
            },
            iceTransportPolicy = RTCIceTransportPolicy.All
        };

        var connection = new RTCPeerConnection(ref config);
        peerConnections[peerId] = connection;

        if (localAudioTrack != null)
        {
            connection.AddTrack(localAudioTrack);
        }

        connection.OnIceCandidate = candidate =>
        {
            var candidateInit = new CandidateInit()
            {
                SdpMid = candidate.SdpMid,
                SdpMLineIndex = candidate.SdpMLineIndex ?? 0,
                Candidate = candidate.Candidate
            };
            signalingSocket.Send($"CANDIDATE!{peerId}!{candidateInit.ConvertToJSON()}");
        };

        connection.OnIceConnectionChange = state =>
        {
            Debug.Log($"ICE connection with {peerId} changed to {state}");
            if (state == RTCIceConnectionState.Disconnected ||
                state == RTCIceConnectionState.Failed)
            {
                CleanupPeer(peerId);
            }
        };

        connection.OnTrack = e =>
        {
            if (e.Track is AudioStreamTrack remoteAudioTrack)
            {
                Debug.Log($"Received audio track from {peerId}");

                var go = new GameObject($"RemoteAudio_{peerId}");
                go.transform.SetParent(transform);
                var audioSource = go.AddComponent<AudioSource>();

                audioSource.SetTrack(remoteAudioTrack);
                audioSource.loop = true;
                audioSource.Play();

                audioTracks[peerId] = remoteAudioTrack;
            }
        };

        StartCoroutine(CreateOffer(peerId, connection));
        
        connection.OnConnectionStateChange = state =>
        {
            Debug.Log($"Connection with {peerId} changed to {state}");
            if (state == RTCPeerConnectionState.Failed || 
                state == RTCPeerConnectionState.Disconnected)
            {
                CleanupPeer(peerId);
            }
        };
    }

    private IEnumerator CreateOffer(string peerId, RTCPeerConnection connection)
    {
        var offer = connection.CreateOffer();
        yield return offer;
        
        if (offer.IsError)
        {
            Debug.LogError($"Offer creation error: {offer.Error.message}");
            yield break;
        }
        
        var desc = offer.Desc;
        var op = connection.SetLocalDescription(ref desc);
        yield return op;
        
        if (op.IsError)
        {
            Debug.LogError($"Local description set error: {op.Error.message}");
            yield break;
        }
        
        var sessionDesc = new SessionDescription()
        {
            SessionType = desc.type.ToString(),
            Sdp = desc.sdp
        };
        
        signalingSocket.Send($"OFFER!{peerId}!{sessionDesc.ConvertToJSON()}");
    }

    private void HandleOffer(string peerId, string offerJson)
    {
        Debug.Log($"Received offer from {peerId}");
        
        if (!peerConnections.ContainsKey(peerId))
        {
            CreatePeerConnection(peerId);
        }
        
        var connection = peerConnections[peerId];
        var offer = SessionDescription.FromJSON(offerJson);
        var desc = new RTCSessionDescription
        {
            type = RTCSdpType.Offer,
            sdp = offer.Sdp
        };
        
        StartCoroutine(HandleOfferCoroutine(peerId, connection, desc));
    }

    private IEnumerator HandleOfferCoroutine(string peerId, RTCPeerConnection connection, RTCSessionDescription desc)
    {
        var op = connection.SetRemoteDescription(ref desc);
        yield return op;
        
        if (op.IsError)
        {
            Debug.LogError($"Remote description set error: {op.Error.message}");
            yield break;
        }
        
        var answer = connection.CreateAnswer();
        yield return answer;
        
        if (answer.IsError)
        {
            Debug.LogError($"Answer creation error: {answer.Error.message}");
            yield break;
        }
        
        var answerDesc = answer.Desc;
        op = connection.SetLocalDescription(ref answerDesc);
        yield return op;
        
        if (op.IsError)
        {
            Debug.LogError($"Local description set error: {op.Error.message}");
            yield break;
        }
        
        var sessionDesc = new SessionDescription()
        {
            SessionType = answerDesc.type.ToString(),
            Sdp = answerDesc.sdp
        };
        
        signalingSocket.Send($"ANSWER!{peerId}!{sessionDesc.ConvertToJSON()}");
    }

    private void HandleAnswer(string peerId, string answerJson)
    {
        Debug.Log($"Received answer from {peerId}");
        
        if (!peerConnections.ContainsKey(peerId)) return;
        
        var answer = SessionDescription.FromJSON(answerJson);
        var desc = new RTCSessionDescription
        {
            type = RTCSdpType.Answer,
            sdp = answer.Sdp
        };
        
        StartCoroutine(SetRemoteDescription(peerConnections[peerId], desc));
    }

    private IEnumerator SetRemoteDescription(RTCPeerConnection connection, RTCSessionDescription desc)
    {
        var op = connection.SetRemoteDescription(ref desc);
        yield return op;
        
        if (op.IsError)
        {
            Debug.LogError($"Remote description set error: {op.Error.message}");
        }
    }

    private void HandleCandidate(string peerId, string candidateJson)
    {
        if (!peerConnections.ContainsKey(peerId)) return;
        
        var candidateInit = CandidateInit.FromJSON(candidateJson);
        var init = new RTCIceCandidateInit
        {
            candidate = candidateInit.Candidate,
            sdpMid = candidateInit.SdpMid,
            sdpMLineIndex = candidateInit.SdpMLineIndex
        };
        
        var candidate = new RTCIceCandidate(init);
        peerConnections[peerId].AddIceCandidate(candidate);
    }

    private void CleanupPeer(string peerId)
    {
        Debug.Log($"Cleaning up peer {peerId}");
        
        if (audioTracks.TryGetValue(peerId, out var track))
        {
            track.Dispose();
            audioTracks.Remove(peerId);
            
            // Destroy the GameObject that was created for this peer's audio
            var go = GameObject.Find($"RemoteAudio_{peerId}");
            if (go != null)
            {
                Destroy(go);
            }
        }
        
        if (peerConnections.TryGetValue(peerId, out var connection))
        {
            connection.Close();
            peerConnections.Remove(peerId);
        }
    }

    private void OnSignalingDisconnected(object sender, CloseEventArgs e)
    {
        Debug.Log($"Signaling server disconnected: {e.Reason}");
        isConnected = false;
        CleanupAllPeers();
    }

    private void OnSignalingError(object sender, ErrorEventArgs e)
    {
        Debug.LogError($"Signaling error: {e.Message}");
        if (e.Exception != null)
        {
            Debug.LogError($"Exception details: {e.Exception.ToString()}");
        }
    }

    private void CleanupAllPeers()
    {
        foreach (var peerId in new List<string>(peerConnections.Keys))
        {
            CleanupPeer(peerId);
        }
    }

    private void OnDestroy()
    {
        if (localAudioTrack != null)
        {
            localAudioTrack.Dispose();
        }
        
        if (signalingSocket != null && signalingSocket.IsAlive)
        {
            signalingSocket.Close();
        }
        
        CleanupAllPeers();
    }

    private void OnGUI()
    {
        if (!showVoiceUI) return;
        
        GUILayout.BeginArea(new Rect(voiceUIPosition.x, voiceUIPosition.y, voiceUISize.x, voiceUISize.y));
        GUILayout.Label("Voice Chat Controls");
        
        if (GUILayout.Button(isMuted ? "Unmute" : "Mute"))
        {
            ToggleMute();
        }
        
        GUILayout.Label($"Status: {(isConnected ? "Connected" : "Disconnected")}");
        GUILayout.Label($"Peers: {peerConnections.Count}");
        
        GUILayout.EndArea();
    }

    private void ToggleMute()
    {
        isMuted = !isMuted;
        if (localAudioTrack != null)
        {
            localAudioTrack.Enabled = !isMuted;
        }
    }
}