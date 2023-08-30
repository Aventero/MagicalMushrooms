using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MonologManager : MonoBehaviour
{
    private Monolog[] monologs;

    private readonly List<WitchMonolog> witchMonologs = new();
    private readonly List<GuideMonolog> guideMonologs = new();


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

    private void SendMonologToUI(Monolog monolog)
    {
        // TODO: Display conversation on screen
        int random = UnityEngine.Random.Range(0, monolog.conversation.Count);
        Debug.Log(monolog.conversation[random]);
    }

    public void DisplayMonolog(WitchType witchType)
    {
        foreach (WitchMonolog witchMonolog in witchMonologs)
        {
            if(witchType.Equals(witchMonolog.witchType))
            {
                SendMonologToUI(witchMonolog);
            }
        }
    }

    public void DisplayMonolog(GuideType guideType)
    {
        foreach (GuideMonolog guideMonolog in guideMonologs)
        {
            if (guideType.Equals(guideMonolog.guideType))
            {
                SendMonologToUI(guideMonolog);
            }
        }
    }
}
