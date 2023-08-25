using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void ContinueFromCheckpoint()
    {
        this.gameObject.SetActive(false);
        StateManager.Instance.ResumeGameEvent.Invoke();
        StateManager.Instance.RespawnPlayerEvent.Invoke();
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
