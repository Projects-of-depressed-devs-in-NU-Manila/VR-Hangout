using System;
using System.Collections.Generic;
using UnityEngine;


// Uses NetworkManager's events to listen and wait for movement of other players.
// This script syncs the other players position. 
public class MultiplayerMovementManager : MonoBehaviour
{
    [SerializeField] private GameObject characterPrefab;

    private Dictionary<string, GameObject> players;

    void Start()
    {
        players = new Dictionary<string, GameObject>();

        NetworkManager.Instance.onOtherPlayerConnect += OnPlayerConnect;
        NetworkManager.Instance.onOtherPlayerDisconnect += OnPlayerDisconnect;
        NetworkManager.Instance.onOtherPlayerMove += OnPlayerMove;
    }

    void OnPlayerConnect(ConnectionMessage message){
        GameObject player = Instantiate(characterPrefab, transform);
        players.Add(message.playerId, player);

    }

    void OnPlayerDisconnect(DisconnectionMessage message){
        Destroy(players[message.playerId], 1f);
        players.Remove(message.playerId);
    }

    void OnPlayerMove(PlayerMoveMessage message){
        players[message.playerId].transform.position = message.position;
    }

    void OnDestroy()
    {
        NetworkManager.Instance.onOtherPlayerConnect -= OnPlayerConnect;
        NetworkManager.Instance.onOtherPlayerDisconnect -= OnPlayerDisconnect;
        NetworkManager.Instance.onOtherPlayerMove -= OnPlayerMove;
    }
}
