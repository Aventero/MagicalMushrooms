using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Quest : MonoBehaviour
{
    public string Name;
    public string DisplayName;
    public bool IsCompleted { get; set; }
    
    public enum Type
    {
        Interact,
        ReachDestination,
        Charge
    }

    public Type QuestType;

    private void Start()
    {
        QuestManager.Instance.AddQuest(this);
    }

    public void MarkAsCompleted()
    {
        IsCompleted = true;
    }

}
