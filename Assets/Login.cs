using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;
    public TMP_InputField UsernameSignuoInput;
    public TMP_InputField PasswordSignupInput;

    public Button LoginButton;
    public Button SignupButton;

    void Start()
    {
        LoginButton.onClick.AddListener(() => { StartCoroutine(SigninCoroutine()); });
        SignupButton.onClick.AddListener(() => { StartCoroutine(SignupCoroutine()); });
    }

    IEnumerator SigninCoroutine()
    {
        AuthRequest authRequest = new AuthRequest();
        authRequest.username = UsernameInput.text;
        authRequest.password = PasswordInput.text;

        string url = $"{APIUrl.url}auth/signin";
        string json = JsonHelper.ToJson(authRequest);

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Error: " + request.error);
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);

            AuthResponse response = JsonHelper.FromJson<AuthResponse>(request.downloadHandler.text);
            PlayerContext.Instance.playerId = response.player_id;
            PlayerContext.Instance.playerName = response.username;

            SceneManager.LoadScene("MainMenu");
        }
    }

    IEnumerator SignupCoroutine()
    {
        AuthRequest authRequest = new AuthRequest();
        authRequest.username = UsernameInput.text;
        authRequest.password = PasswordInput.text;

        string url = $"{APIUrl.url}auth/signup";
        string json = JsonHelper.ToJson(authRequest);

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Error: " + request.error);
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            AuthResponse response = JsonHelper.FromJson<AuthResponse>(request.downloadHandler.text);
            PlayerContext.Instance.playerId = response.player_id;
            PlayerContext.Instance.playerName = response.username;
        }
    }
}
