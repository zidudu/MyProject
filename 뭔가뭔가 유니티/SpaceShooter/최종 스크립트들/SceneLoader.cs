using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Real_MYGAME 1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
