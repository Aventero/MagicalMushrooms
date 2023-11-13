using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Quest : MonoBehaviour
{
    public enum QuestType
    {
        ReachDestination,
        Collect
    }

    public QuestType Type;

    private void Start()
    {
        QuestManager.Instance.AddQuest(this);
    }

    public void CompletedQuest()
    {
        QuestManager.Instance.RemoveQuest(this);
        Destroy(gameObject);
    }
}
