using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public List<Quest> Quests;
    public List<Quest> ActiveQuests { get; private set; } = new List<Quest>();
    public List<Quest> CompletedQuests { get; private set; } = new List<Quest>();

    private QuestUI questUI;

    private QuestIcons questIcons;

    void Start()
    {
        questIcons = GetComponent<QuestIcons>();

        // = new List<Quest>();
        StateManager.Instance.ItemPickupEvent += this.UpdateQuestItems;
        StateManager.Instance.UsedItemEvent += this.UpdateQuestItems;

        questUI = FindObjectOfType<QuestUI>();

        foreach (Quest quest in Quests)
        {
            ActiveQuests.Add(quest);
        }
    }

    public void AddQuest(Quest quest)
    {
        Quests.Add(quest);
        ActiveQuests.Add(quest);
    }

    private void UpdateQuestItems(ItemData item)
    {
        Quest finishedQuest = Quests.Find(quest => quest.item.Name.Equals(item.Name) && !quest.isFinished);
        if (finishedQuest != null)
        {
            finishedQuest.isFinished = true;
            CompletedQuests.Add(finishedQuest);
            ActiveQuests.Remove(finishedQuest);
            questIcons.QuestCompleted(finishedQuest);
        }

        questUI.UpdateQuests(Quests);
    }
}
