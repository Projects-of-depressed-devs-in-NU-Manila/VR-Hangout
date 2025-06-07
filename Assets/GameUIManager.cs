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

            // send chat to network

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
}
