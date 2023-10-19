using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoaderFinder : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        Debug.Log("I GOT SIGNALED!");
        SceneLoader sceneLoader = FindObjectOfType<SceneLoader>();
        sceneLoader.LoadScene(sceneName);
    }

}
