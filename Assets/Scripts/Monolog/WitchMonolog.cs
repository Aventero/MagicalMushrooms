using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Monolog", menuName = "Conversation/Monolog/WitchMonolog", order = 1)]
public class WitchMonolog : Monolog
{
    public WitchType witchType;
}

public enum WitchType
{
    ANGRY,
    VANISHED,
    SEARCHING
}