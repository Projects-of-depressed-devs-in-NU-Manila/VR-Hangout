using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FriendListUI : MonoBehaviour
{

    public GameObject friendListingPrefab;
    public TMP_InputField friendIdInput;
    public Button addFriendButton;

    void Start()
    {

        addFriendButton.onClick.AddListener(() => {
                                                string friend_id = friendIdInput.text;
                                                StartCoroutine(AddFriendCoroutine(friend_id));
                                            });
        
    }

    void OnEnable()
    {
        StartCoroutine(GetFriendList());
    }

    IEnumerator GetFriendList()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(APIUrl.url + $"friendship?player_id={PlayerContext.Instance.playerId}"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                FriendList friendList = JsonHelper.FromJson<FriendList>(request.downloadHandler.text);

                foreach (Friend friend in friendList.friends)
                {
                    GameObject frieldListing = Instantiate(friendListingPrefab, transform);
                    frieldListing.transform.GetChild(0).GetComponent<TMP_Text>().text = friend.player_name;
                    frieldListing.transform.GetChild(1).GetComponent<Button>().onClick.AddListener( () => { StartCoroutine(RemoveFriendCoroutine(friend.player_id)); });
                }
            }
        }
    }

    IEnumerator AddFriendCoroutine(string player_id)
    {
        FriendRequest friendRequest = new FriendRequest();
        friendRequest.player_id1 = PlayerContext.Instance.playerId;
        friendRequest.player_id2 = player_id;

        string url = $"{APIUrl.url}friendship";
        string json = JsonHelper.ToJson(friendRequest);

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Error: " + request.error);
        else
            Debug.Log("Response: " + request.downloadHandler.text);
    }

    IEnumerator RemoveFriendCoroutine(string player_id)
    {
        FriendRequest friendRequest = new FriendRequest();
        friendRequest.player_id1 = PlayerContext.Instance.playerId;
        friendRequest.player_id2 = player_id;

        string url = $"{APIUrl.url}friendship";
        string json = JsonHelper.ToJson(friendRequest);

        var request = new UnityWebRequest(url, "DELETE");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Error: " + request.error);
        else
            Debug.Log("Response: " + request.downloadHandler.text);
    }

    void OnDisable()
    {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        
    }
}
