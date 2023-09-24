using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [Header("Skill")]
    public float RechargeTime;
    public string TooltipText;

    public bool IsActivated { get; internal set; }

    public virtual void ShowPreview() { }
    public virtual void HidePreview() { }

    public virtual bool Execute() { return false; }
}
