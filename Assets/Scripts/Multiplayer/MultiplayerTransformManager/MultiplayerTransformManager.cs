using System;
using System.Collections.Generic;
using UnityEngine;


// Uses NetworkManager's events to listen and wait for movement of other players.
// This script syncs the other players position. 
public class MultiplayerMovementManager : MonoBehaviour
{
    [SerializeField] private GameObject characterPrefab;
    public static MultiplayerMovementManager Instance;

    private Dictionary<string, GameObject> players;
    private Dictionary<string, Animator> animators;

    public event Action<string, GameObject> onPlayerSpawn;
    public event Action<string> onPlayerDespawn;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        players = new Dictionary<string, GameObject>();
        animators = new Dictionary<string, Animator>();

        NetworkManager.Instance.onOtherPlayerConnect += OnPlayerConnect;
        NetworkManager.Instance.onOtherPlayerDisconnect += OnPlayerDisconnect;
        NetworkManager.Instance.onOtherPlayerMove += OnPlayerMove;
        NetworkManager.Instance.OnAnimationSync += OnPlayerAnimationSync;
    }

    void OnPlayerConnect(ConnectionMessage message){
        GameObject player = Instantiate(characterPrefab, transform);
        player.transform.GetChild(1).Find(message.avatar_name).gameObject.SetActive(true);
        players.Add(message.playerId, player);
        animators.Add(message.playerId, player.GetComponent<Animator>());
        onPlayerSpawn(message.playerId, player);
    }

    void OnPlayerDisconnect(DisconnectionMessage message){
        Destroy(players[message.playerId]);
        players.Remove(message.playerId);
        animators.Remove(message.playerId);
        onPlayerDespawn(message.playerId);
    }

    void OnPlayerMove(PlayerMoveMessage message){
        players[message.playerId].transform.position = message.position;
        players[message.playerId].transform.rotation = Quaternion.Euler(message.rotation);
    }

    void OnPlayerAnimationSync(PlayerAnimation animation)
    {
        Animator animator = animators[animation.player_id];
        animator.SetBool("isWalking", animation.isWalking);
        animator.SetBool("isRunning", animation.isRunning);
        animator.SetBool("isFishing", animation.isFishing);

    }

    void OnDestroy()
    {
        NetworkManager.Instance.onOtherPlayerConnect -= OnPlayerConnect;
        NetworkManager.Instance.onOtherPlayerDisconnect -= OnPlayerDisconnect;
        NetworkManager.Instance.onOtherPlayerMove -= OnPlayerMove;
    }
}
