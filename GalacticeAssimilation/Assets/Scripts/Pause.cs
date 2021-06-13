using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public GameObject PauseMenu;
    public Button ResumeButton;
    public Button ExitButton;

    private bool _isPaused = false;

    private void Start()
    {
        ResumeButton.onClick.AddListener(() =>
        {
            ResumeGame();
        });
        ExitButton.onClick.AddListener(() =>
        {
            ExitToMainMenu();
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (_isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        PauseMenu.SetActive(false);
        _isPaused = false;
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        PauseMenu.SetActive(true);
        _isPaused = true;
    }

    private void ExitToMainMenu()
    {
        ResumeGame();
        SceneManager.LoadScene(Constants.MAIN_MENU_SCENE);
    }
}
