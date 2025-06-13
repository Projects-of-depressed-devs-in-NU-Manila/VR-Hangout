using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterWorldButton : MonoBehaviour
{
    public void OnClick()
    {
        GameObject player = GameObject.Find("Player");
        foreach (var component in player.GetComponents<Component>())
        {
            var type = component.GetType();
            var enabledProperty = type.GetProperty("enabled");
            if (enabledProperty != null && enabledProperty.PropertyType == typeof(bool))
            {
                enabledProperty.SetValue(component, true);
            }
        }
        SceneManager.LoadScene("PlacingObjectTest");
    }
}
