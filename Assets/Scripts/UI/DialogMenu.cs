using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogMenu: MonoBehaviour
{
    [Header("Text Objects")]
    public GameObject DialogProgressText;
    public GameObject DialogText;
    public GameObject CharacterName;
    public GameObject CharacterImage;

    [Header("Buttons")]
    public GameObject NextButton;
    public GameObject PrevButton;
    public GameObject EndButton;

    private List<string> texts;

    private int currentTextPos = 0;

    [Header("Dialog effect")]
    public float GrowDuration = 0.05f;
    public float WaitForNextLetter = 0.01f;


    public void ShowDialog(Dialog conversation)
    {
        UIManager.Instance.SetSkillBarVisibility(false);
        StateManager.Instance.PauseGameEvent.Invoke();

        SetUp(conversation);
        ShowTextOverTime();
    }

    private void SetUp(Dialog conversation)
    {
        texts = conversation.conversation;

        CharacterImage.GetComponent<Image>().sprite = conversation.characterImage;
        CharacterName.GetComponent<TMP_Text>().text = conversation.characterName;
    }

    public void ShowTextOverTime()
    {
        if (currentTextPos < 0 || currentTextPos >= texts.Count)
            return;

        StopAllCoroutines(); // Stop the typing effect if it's still going on
        StartCoroutine(TypeText(DialogText.GetComponent<TMP_Text>(), texts[currentTextPos]));

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
        ShowTextOverTime();
    }

    public void PrevText()
    {
        if (currentTextPos <= 0)
            return;

        currentTextPos--;
        ShowTextOverTime();
    }

    public void EndDialog()
    {
        UIManager.Instance.SetOverlayVisibility(true);
        currentTextPos = 0;
        StateManager.Instance.ResumeGameEvent.Invoke();
        this.gameObject.SetActive(false);
    }

    private IEnumerator TypeText(TMP_Text textField, string fullText)
    {
        textField.text = "";
        float growDuration = GrowDuration; 
        for (int i = 0; i < fullText.Length; i++)
        {
            string currentText = fullText.Substring(0, i);
            string growingLetter = $"<size=60%>{fullText[i]}</size>"; // Start the letter at 60% of its original size
            textField.text = currentText + growingLetter;

            // Animate the growth of the character
            for (float t = 0; t < growDuration; t += Time.unscaledDeltaTime)
            {
                float progress = Mathf.Clamp01(t / growDuration);
                float sizePercent = Mathf.Lerp(60, 100, progress); // Lerp from 60% to 100%
                growingLetter = $"<size={sizePercent}%>{fullText[i]}</size>";
                textField.text = currentText + growingLetter;
                yield return null;
            }

            yield return new WaitForSeconds(WaitForNextLetter); // Delay after the letter has fully appeared
        }
    }


}