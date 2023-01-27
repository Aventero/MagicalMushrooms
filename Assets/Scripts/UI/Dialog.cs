using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    [Header("Text Objects")]
    public GameObject DialogProgressText;
    public GameObject DialogText;

    [Header("Buttons")]
    public GameObject NextButton;
    public GameObject PrevButton;
    public GameObject EndButton;

    [Header("Text Objects")]
    [TextArea]
    public List<string> Texts;

    private int currentTextPos = 0;

    public void ShowDialog()
    {
        Debug.Log("Showing");
        StateManager.Instance.PauseGameEvent();
        UpdateText();
    }

    public void UpdateText()
    {
        if (currentTextPos < 0 || currentTextPos >= Texts.Count)
            return;
        
        Debug.Log("Show new Text");
        DialogText.GetComponent<TMP_Text>().text = Texts[currentTextPos];
        DialogProgressText.GetComponent<TMP_Text>().text = (currentTextPos + 1) + "/" + Texts.Count;

        UpdateButtons();
    }

    public void UpdateButtons()
    {
        if (currentTextPos == 0)
        {
            // First Entry
            NextButton.SetActive(true);
            PrevButton.SetActive(false);
            EndButton.SetActive(false);
        }
        else if (currentTextPos == Texts.Count - 1)
        {
            // Middle
            NextButton.SetActive(false);
            PrevButton.SetActive(true);
            EndButton.SetActive(true);
        }
        else
        {
            // Last Entry
            NextButton.SetActive(true);
            PrevButton.SetActive(true);
            EndButton.SetActive(false);
        }
    }

    public void NextText()
    {
        if (currentTextPos >= Texts.Count)
            return;

        currentTextPos++;
        UpdateText();
    }

    public void PrevText()
    {
        if (currentTextPos <= 0)
            return;

        currentTextPos--;
        UpdateText();
    }

    public void EndDialog()
    {
        StateManager.Instance.ResumeGameEvent();
        this.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}