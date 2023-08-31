using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MonologManager : MonoBehaviour
{
    private Monolog[] monologs;

    private readonly List<WitchMonolog> witchMonologs = new();
    private readonly List<GuideMonolog> guideMonologs = new();

    public GameObject Witch;

    public static MonologManager Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        // Get all monologues within the "Assets/Resources/Monolog" folder
        monologs = Resources.LoadAll<Monolog>("Monolog");
        SetupMonolog();
    }

    private void SetupMonolog()
    {
        foreach (Monolog monolog in monologs)
        {
            switch(monolog)
            {
                case WitchMonolog witch:
                    witchMonologs.Add(witch);
                    break;
                case GuideMonolog guide:
                    guideMonologs.Add(guide);
                    break;
            }
        }
    }

    private void SendMonologToUI(Monolog monolog, GameObject target)
    {
        UIManager.Instance.ShowMonolog(monolog, target);
    }

    public void DisplayMonolog(WitchType witchType)
    {
        foreach (WitchMonolog witchMonolog in witchMonologs)
        {
            if(witchType.Equals(witchMonolog.witchType))
            {
                SendMonologToUI(witchMonolog, Witch);
            }
        }
    }

    //public void DisplayMonolog(GuideType guideType)
    //{
    //    foreach (GuideMonolog guideMonolog in guideMonologs)
    //    {
    //        if (guideType.Equals(guideMonolog.guideType))
    //        {
    //            GameObject Guide = GameObject.FindGameObjectWithTag("Guide");
    //            if (Guide != null)
    //                SendMonologToUI(guideMonolog, Guide);
    //        }
    //    }
    //}
}
