using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitScript : MonoBehaviour
{
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ToAvatar()
    {
        SceneManager.LoadScene("Avatar");
    }

    public void ToLoginPage()
    {
        SceneManager.LoadScene("LoginPage");
    }
}
