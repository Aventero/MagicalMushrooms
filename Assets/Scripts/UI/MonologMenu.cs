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

    public GameObject target;

    private GameObject player;
    private bool activeMonolog = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
        Debug.Log(screenPosition);
        this.GetComponent<RectTransform>().position = new Vector3(screenPosition.x, screenPosition.y, screenPosition.z);
    }

    private IEnumerator HideMonolog(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        activeMonolog = false;
        this.gameObject.SetActive(false);
    }
}
