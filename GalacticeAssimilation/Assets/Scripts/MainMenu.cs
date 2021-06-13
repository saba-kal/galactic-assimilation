using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(Constants.MAIN_GAME_SCENE);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
