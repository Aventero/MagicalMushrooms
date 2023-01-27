using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string Name;
    public string DisplayName;
    public bool isFinished;

    public enum Type
    {
        PickupItem,
        UseItem,
        Location
    }

    public Type QuestType;

    public ItemData item;
}
