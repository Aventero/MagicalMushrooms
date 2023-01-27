using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public List<Quest> quests;

    private QuestUI questUI;

    void Start()
    {
        // = new List<Quest>();
        StateManager.Instance.ItemPickupEvent += this.UpdateQuestItems;
        StateManager.Instance.UsedItemEvent += this.UpdateQuestItems;

        questUI = FindObjectOfType<QuestUI>();
    }

    public void AddQuest(Quest quest)
    {
        quests.Add(quest);
    }

    private void UpdateQuestItems(ItemData item)
    {
        Quest finishedQuest = quests.Find(quest => quest.item.Name.Equals(item.Name) && !quest.isFinished);
        if (finishedQuest != null)
            finishedQuest.isFinished = true;

        questUI.UpdateQuests(quests);
    }
}
