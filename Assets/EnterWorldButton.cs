using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterWorldButton : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("PlacingObjectsTest");
    }
}
