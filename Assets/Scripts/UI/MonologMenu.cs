using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonologMenu : MonoBehaviour
{
    public TMP_Text TextArea;
    public TMP_Text CharacterName;

    public GameObject CharacterImage;
    public GameObject MonologWindow;

    private GameObject target;
    private RectTransform rect;
    private Image image;

    private Vector2 offset;

    private bool activeMonolog = false;

    private void Start()
    {
        rect = this.GetComponent<RectTransform>();
        offset = rect.sizeDelta / new Vector2(2, 2);

        image = CharacterImage.GetComponent<Image>();
    }

    public void ShowMonolog(Monolog monolog, GameObject target)
    {
        int randomNumber = Random.Range(0, monolog.conversation.Count);
        TextArea.text = monolog.conversation[randomNumber];

        CharacterName.text = monolog.characterName;
        image.sprite = monolog.characterSprite;

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

        if (screenPosition.z < 0)
        {
            MonologWindow.SetActive(false);
            return;
        }
        else
        {
            MonologWindow.SetActive(true);
        }

        float screenPosX = Mathf.Clamp(screenPosition.x, offset.x, Screen.width - offset.x);
        float screenPosY = Mathf.Clamp(screenPosition.y, offset.y, Screen.height - offset.y);

        rect.position = new Vector3(screenPosX, screenPosY, 0);
    }

    private IEnumerator HideMonolog(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        activeMonolog = false;
        this.gameObject.SetActive(false);
    }
}
