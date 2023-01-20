using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    TMPro.TMP_Text FrameRate;
    float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        FrameRate = GetComponent<TMPro.TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= 1f)
        {
            FrameRate.SetText("FR: " + (int)(1f / Time.deltaTime));
            timer = 0f;
        }
        timer += Time.deltaTime;
    }
}
