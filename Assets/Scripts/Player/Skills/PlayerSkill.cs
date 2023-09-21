using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [Header("Skill")]
    public float rechargeTime;

    public bool IsActivated { get; internal set; }

    public virtual void ShowPreview() { }
    public virtual void HidePreview() { }

    public virtual void Execute() { }
}
