using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Quitting the game! Bye!");
        Application.Quit();
    }
}
