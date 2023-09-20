using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monolog", menuName = "Conversation/Monolog/GuideMonolog", order = 1)]
public class GuideMonolog : Monolog
{
    public GuideType guideType;
}

public enum GuideType
{
    FOUND_ITEM
}