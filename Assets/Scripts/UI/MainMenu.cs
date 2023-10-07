using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Awake()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Continue()
    {
        NewGame();
    }

    public void NewGame()
    {
        SetCursor();
        SceneManager.LoadScene(1);
    }

    private void SetCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Quit()
    {
        Debug.Log("Quitting the game! Bye!");
        Application.Quit();
    }
}
