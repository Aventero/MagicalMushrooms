using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Hovering over: " + EventSystem.current.IsPointerOverGameObject());
    }

    public void OnButtonClick()
    {
        Debug.Log("Clicked On Button!");
    }
}
