using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToNextCamera : MonoBehaviour
{
    public GameObject DollyTracks;
    public float CartSpeed = 1.0f;

    private readonly List<GameObject> tracks = new();
    private GameObject currentTrack = null;
    private int currentTrackNumber;

    private float pathLength;
    private float cartPos;
    private int priority = 11;

    private CinemachineDollyCart currentCart;
    private CinemachinePathBase currentPath;

    void Start()
    {   
        for(int i = 0; i < DollyTracks.transform.childCount; i++)
            tracks.Add(DollyTracks.transform.GetChild(i).gameObject);
        
        currentTrackNumber = 0;
        priority = 11;
        SwitchCamera();
    }

    private void SwitchCamera()
    {
        if (currentTrackNumber > tracks.Count - 1)
            currentTrackNumber = 0;

        currentTrack = tracks[currentTrackNumber];

        currentCart = currentTrack.GetComponentInChildren<CinemachineDollyCart>();
        currentCart.m_Position = 0;

        currentPath = currentTrack.GetComponent<CinemachinePathBase>();
        currentTrack.GetComponentInChildren<CinemachineVirtualCamera>().Priority = priority;

        currentCart.m_Speed = CartSpeed;

        currentTrackNumber++;
        priority++;
    }

    void Update()
    {
        cartPos = currentCart.m_Position;
        pathLength = currentPath.PathLength;

        if (cartPos >= pathLength)
            SwitchCamera();
    }
}
