using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Continue()
    {
        NewGame();
    }

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Debug.Log("Quitting the game! Bye!");
        Application.Quit();
    }
}
