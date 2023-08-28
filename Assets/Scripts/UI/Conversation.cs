using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Conversation", menuName = "Conversation", order = 1)]
public class Conversation : ScriptableObject
{
    public string characterName;
    public Image characterImage;
    public ConversationType conversationType;

    [TextArea]
    public List<string> conversation;

    public enum ConversationType
    {
        DIALOG, MONOLOG
    }
}
