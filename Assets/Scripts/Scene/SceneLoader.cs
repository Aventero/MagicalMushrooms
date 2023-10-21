using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public static List<string> Scenes = new List<string>();

    public Image fadeImage;
    public Animator SceneAnimator;
    public float fadeDuration = 1f;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        Debug.Log("Trying to load: " + sceneName);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Pause, Fade, unpause to let the scene load.
        Time.timeScale = 0;
        SceneAnimator.Play("StartFade");
        Time.timeScale = 1;

        yield return new WaitForSeconds(1);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
                break;

            yield return null;
        }

        asyncOperation.allowSceneActivation = true;
    }
}
