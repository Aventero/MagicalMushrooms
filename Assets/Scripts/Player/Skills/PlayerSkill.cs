using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [Header("Skill")]
    public float RechargeTime;
    public string TooltipText;
    public int SkillCost;

    [HideInInspector]
    public bool activated;
    [HideInInspector]
    public bool isRecharging;

    public bool IsActivated { get; internal set; }
    public bool IsMouseHeld { get; set; } = false;
    public virtual bool CanBeHeld() { return false; }

    public virtual void ShowPreview() { }
    public virtual void HidePreview() { }
    public virtual bool Execute() { return false; }

}
