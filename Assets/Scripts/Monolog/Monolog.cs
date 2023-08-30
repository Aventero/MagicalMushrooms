using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Monolog", menuName = "Conversation/Monolog", order = 1)]
public class Monolog : ScriptableObject
{
    public string characterName;
    public Image characterImage;

    [TextArea]
    public List<string> conversation;
}