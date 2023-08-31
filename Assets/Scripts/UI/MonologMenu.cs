using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class MonologMenu : MonoBehaviour
{
    public TMP_Text TextArea;
    public TMP_Text CharacterName;
    public TMP_Text CharacterImage;

    public GameObject MonologWindow;

    private GameObject target;
    private RectTransform rect;

    private bool activeMonolog = false;

    private void Start()
    {
        rect = this.GetComponent<RectTransform>();
    }

    public void ShowMonolog(Monolog monolog, GameObject target)
    {
        int randomNumber = Random.Range(0, monolog.conversation.Count);
        TextArea.text = monolog.conversation[randomNumber];

        CharacterName.text = monolog.characterName;
        // TODO: Set the Image

        this.target = target;
        
        activeMonolog = true;

        //StartCoroutine(HideMonolog(5)); 
    }

    private void Update()
    {
        if (!activeMonolog)
            return;

        Vector3 targetPosition = target.transform.position;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetPosition);

        Vector2 rectSize = rect.sizeDelta;

        float offsetX = rectSize.x / 2;
        float offsetY = rectSize.y / 2;

        if (screenPosition.z < 0)
        {
            MonologWindow.SetActive(false);
            return;
        }
        else
        {
            MonologWindow.SetActive(true);
        }

        float screenPosX = Mathf.Clamp(screenPosition.x, offsetX, Screen.width - offsetX);
        float screenPosY = Mathf.Clamp(screenPosition.y, offsetY, Screen.height - offsetY);

        rect.position = new Vector3(screenPosX, screenPosY, 0);
    }

    private IEnumerator HideMonolog(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        activeMonolog = false;
        this.gameObject.SetActive(false);
    }
}
