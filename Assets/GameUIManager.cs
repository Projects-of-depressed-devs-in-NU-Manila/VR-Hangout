using System;
using System.Runtime.ExceptionServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;
    public bool isEnabled = true;

    public event Action<bool> shouldCursorLock;
    public event Action<bool> shouldDisableKeyboard;

    public GameObject chatPrefab;
    public GameObject chatHistoryPanel;

    public GameObject icons;
    public GameObject worldChat;
    public GameObject equippedItem;
    public GameObject friendsMenu;
    public GameObject worldMenu;
    public GameObject inventory;
    public GameObject settings;

    public TMP_InputField friendIdInput;
    public TMP_InputField WorldIDInput;
    public TMP_InputField chatInput;

    public bool isTyping = false;
    public void setIsTyping(bool value)
    {
        isTyping = value;
    }

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
        NetworkManager.Instance.OnChatRecieved += OnChatRecieved; 

        shouldDisableKeyboard += setIsTyping;
        icons.SetActive(true);
        worldChat.SetActive(true);
        equippedItem.SetActive(true);
        friendsMenu.SetActive(false);
        worldMenu.SetActive(false);
        inventory.SetActive(false);
        settings.SetActive(false);
    }

    void Update()
    {
        if (!isEnabled)
            return;

        if (Input.GetKeyDown(KeyCode.Return))
            handleEnter();
        if (Input.GetKeyDown(KeyCode.Escape))
            handleEscape();

        if (isTyping)
            return;

        if (EventSystem.current.currentSelectedGameObject == friendIdInput)
            return;

        if (EventSystem.current.currentSelectedGameObject == WorldIDInput)
            return;

        if (Input.GetKeyDown(KeyCode.P))
            handleP();
        if (Input.GetKeyDown(KeyCode.I))
            handleI();
        if (Input.GetKeyDown(KeyCode.M))
            handleM();
    }

    void handleEnter()
    {
        if (EventSystem.current.currentSelectedGameObject == chatInput.gameObject)
        {
            Debug.Log("Chat off");

            Chat chat = new Chat();
            chat.type = "chat";
            chat.player_id = PlayerContext.Instance.playerId;
            chat.player_name = PlayerContext.Instance.playerName;
            chat.message = chatInput.text;
            NetworkManager.Instance.Broadcast(JsonHelper.ToJson(chat));


            shouldDisableKeyboard?.Invoke(false);
            chatInput.text = "";
            chatInput.DeactivateInputField(true);
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            Debug.Log("Chat on");
            shouldDisableKeyboard?.Invoke(true);
            chatInput.Select();
            chatInput.ActivateInputField();
        }
    }

    void handleEscape()
    {
        hideAllPopups();

        if (settings.activeInHierarchy)
        {
            shouldCursorLock?.Invoke(true);
            icons.SetActive(true);
            worldChat.SetActive(true);
            settings.SetActive(false);
        }
        else
        {
            shouldCursorLock?.Invoke(false);
            shouldDisableKeyboard?.Invoke(false);
            icons.SetActive(false);
            worldChat.SetActive(false);
            settings.SetActive(true);
        }
    }

    void handleP()
    {
        if (friendsMenu.activeInHierarchy)
        {


            shouldCursorLock?.Invoke(true);
            friendsMenu.SetActive(false);
        }
        else
        {
            hideAllPopups();
            shouldCursorLock?.Invoke(false);
            shouldDisableKeyboard(true);
            friendIdInput.Select();
            friendIdInput.ActivateInputField();
            Debug.Log("test");
            friendsMenu.SetActive(true);
        }
    }

    void handleI()
    {
        if (inventory.activeInHierarchy)
        {
            shouldCursorLock?.Invoke(true);
            inventory.SetActive(false);
        }
        else
        {
            hideAllPopups();
            shouldCursorLock?.Invoke(false);
            inventory.SetActive(true);
        }
    }

    void handleM()
    {
        if (inventory.activeInHierarchy)
        {

            shouldCursorLock?.Invoke(true);
            worldMenu.SetActive(false);
        }
        else
        {
            hideAllPopups();
            shouldCursorLock?.Invoke(false);
            shouldDisableKeyboard(true);
            WorldIDInput.Select();
            WorldIDInput.ActivateInputField();
            worldMenu.SetActive(true);
        }
    }

    void hideAllPopups()
    {
        worldMenu.SetActive(false);
        inventory.SetActive(false);
        friendsMenu.SetActive(false);
    }

    public void OnChatRecieved(Chat chat)
    {
        GameObject chat1 = Instantiate(chatPrefab, chatHistoryPanel.transform);
        chat1.GetComponent<TMP_Text>().text = $"{chat.player_name}: {chat.message}";
    }
}
