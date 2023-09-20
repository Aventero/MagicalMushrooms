using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void ContinueFromCheckpoint()
    {
        this.gameObject.SetActive(false);
        StateManager.Instance.RespawnPlayerEvent.Invoke();
        StateManager.Instance.ResumeGameEvent.Invoke();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
