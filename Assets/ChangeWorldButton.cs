using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChangeWorldButton : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button button;

    void Start()
    {
        button.onClick.AddListener(onChangeWorldButtonClicked);
    }

    void onChangeWorldButtonClicked() {
        ChangeWorld changeWorld = new ChangeWorld();
        changeWorld.type = "changeWorld";
        changeWorld.worldId = inputField.text;
        NetworkManager.Instance.Broadcast(JsonHelper.ToJson(changeWorld));
    }

}
