using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    [Header("Text Objects")]
    public GameObject DialogProgressText;
    public GameObject DialogText;
    public GameObject DialogName;

    [Header("Buttons")]
    public GameObject NextButton;
    public GameObject PrevButton;
    public GameObject EndButton;

    private List<string> texts;

    private int currentTextPos = 0;

    public void ShowDialog(Conversation conversation)
    {
        StateManager.Instance.PauseGameEvent.Invoke();

        SetUp(conversation);
        UpdateText();
    }

    private void SetUp(Conversation conversation)
    {
        texts = conversation.conversation;

        DialogName.GetComponent<TMP_Text>().text = conversation.characterName;
    }

    public void UpdateText()
    {
        if (currentTextPos < 0 || currentTextPos >= texts.Count)
            return;
        
        DialogText.GetComponent<TMP_Text>().text = texts[currentTextPos];
        DialogProgressText.GetComponent<TMP_Text>().text = (currentTextPos + 1) + "/" + texts.Count;

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
        else if (currentTextPos == texts.Count - 1)
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
        if (currentTextPos >= texts.Count)
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
        currentTextPos = 0;
        StateManager.Instance.ResumeGameEvent.Invoke();
        this.gameObject.SetActive(false);
    }
}