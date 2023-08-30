using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Dialog", menuName = "Conversation/Dialog", order = 1)]
public class Dialog : ScriptableObject
{
    public string characterName;
    public Image characterImage;

    [TextArea]
    public List<string> conversation;
}
