using TMPro;
using UnityEngine;

public class WoldMenuUI : MonoBehaviour
{
    public TMP_InputField inputField;

    public void OnGoToWorldButtonPressed()
    {
        ChangeWorld changeWorld = new ChangeWorld();
        changeWorld.type = "changeWorld";
        changeWorld.worldId = inputField.text;
        NetworkManager.Instance.Broadcast(JsonHelper.ToJson(changeWorld));
    }

    public void GoToHub()
    {
        ChangeWorld changeWorld = new ChangeWorld();
        changeWorld.type = "goToHub";
        changeWorld.worldId = "hub";
        NetworkManager.Instance.Broadcast(JsonHelper.ToJson(changeWorld));
    }

    public void GoToPlayerWorld()
    {
        ChangeWorld changeWorld = new ChangeWorld();
        changeWorld.type = "goToPlayerWorld";
        changeWorld.worldId = "";
        NetworkManager.Instance.Broadcast(JsonHelper.ToJson(changeWorld));
    }
}
