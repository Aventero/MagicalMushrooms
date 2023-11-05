using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public GameObject QuestPrefab;
    public float horizontalPadding;

    private QuestManager questManager;
    private List<GameObject> questObjects;

    void Start()
    {
        questObjects = new List<GameObject>();
        questManager = FindObjectOfType<QuestManager>();
       // CreateQuests(questManager.Quests);
    }

    //private void CreateQuests(List<Quest> quests)
    //{
    //    int i = 0;
    //    foreach (Quest quest in quests)
    //    {
    //        GameObject questObject = Instantiate(QuestPrefab, this.transform);

    //        questObject.name = quest.Name;
    //        questObject.GetComponentInChildren<TMP_Text>().text = quest.DisplayName;

    //        RectTransform transform = questObject.GetComponent<RectTransform>();
    //        transform.localPosition = new Vector2(0, i * transform.rect.height + i * horizontalPadding) * -1;

    //        questObjects.Add(questObject);
    //        i++;
    //    }
    //}

    //public void UpdateQuests(List<Quest> quests)
    //{
    //    foreach (GameObject questObject in questObjects)
    //    {
    //        foreach (Quest quest in quests)
    //        {
    //            if (questObject.name.Equals(quest.Name))
    //            {
    //                questObject.GetComponentInChildren<Toggle>().isOn = quest.IsCompleted;
    //                break;
    //            }

    //        }
    //    }
    //}
}
