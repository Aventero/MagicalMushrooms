using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestIcons : MonoBehaviour
{
    // Find a active quest
    // Get the position
    // Translate it to screen coords
    // show the icon on the sceen
    public GameObject OverlayCanvas;
    public QuestManager QuestManager;
    public Sprite QuestSprite;
    private Dictionary<Quest, GameObject> questSprites = new Dictionary<Quest, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
    }

    private GameObject CreateIcon(GameObject questSprite)
    {
        // Transform
        RectTransform rectTransform = questSprite.AddComponent<RectTransform>();
        rectTransform.SetParent(OverlayCanvas.transform);
        rectTransform.localScale = Vector3.one;
        rectTransform.anchoredPosition = new Vector2(0f, 0f);
        rectTransform.sizeDelta = new Vector2(25f, 25f);

        // Image
        Image image = questSprite.AddComponent<Image>();
        image.sprite = QuestSprite;
        image.transform.SetParent(OverlayCanvas.transform);
        questSprite.SetActive(false);

        return questSprite;
    }

    public void QuestCompleted(Quest quest)
    {
        if (questSprites.TryGetValue(quest, out GameObject questIcon))
        {
            questSprites.Remove(quest);
            Destroy(questIcon);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Quest quest in QuestManager.InCompletedQuests)
        {
            if (questSprites.TryGetValue(quest, out GameObject questIcon))
            {
                Vector3 position = quest.transform.position;
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);

                // Icon is behind the camera so dont draw it
                // z is the distance from camera and player, so if its negative its behind
                if (screenPosition.z < 0)
                {
                    questIcon.SetActive(false);
                    continue;
                }

                questIcon.SetActive(true);
                questIcon.GetComponent<Image>().rectTransform.position = new Vector3(screenPosition.x, screenPosition.y, 0f);
            }
            else
            {
                // Create the Sprite
                questSprites.Add(quest, CreateIcon(new GameObject("QuestIcon")));
            }
        }
    }
}
