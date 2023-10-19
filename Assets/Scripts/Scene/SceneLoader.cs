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
    public float fadeDuration = 1f;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Pause, Fade, unpause to let the scene load.
        Time.timeScale = 0;
        yield return StartCoroutine(FadeImageTo(1f));
        Time.timeScale = 1;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
                break;

            yield return null;
        }

        asyncOperation.allowSceneActivation = true;

        // Wait for the new scene to actually load
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == sceneName);

        Time.timeScale = 0;
        yield return StartCoroutine(FadeImageTo(0f));
        Time.timeScale = 1;
    }

    private IEnumerator FadeImageTo(float targetAlpha)
    {
        Time.timeScale = 0;
        Color color = fadeImage.color;
        float startAlpha = color.a;

        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            float normalizedTime = t / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, normalizedTime);
            fadeImage.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;
    }
}
