using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class WitchUIAnimation : MonoBehaviour
{
    [Header("Used to Play animations in UI")]
    public bool playAnimations;
    public Animator eyeAnimatorInUI;


    private void Start()
    {
    }

    public void UpdateAnimationStates()
    {
        if (eyeAnimatorInUI.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("EyeLidOpen"))
        {

        }
    }

    public void PlayEyeClose()
    {
        eyeAnimatorInUI.Play("EyeLidClose");
    }

    public void PlayEyeOpen(float timeNeeded)
    {
        PlayAnimationForDuration("EyeLidOpen", timeNeeded);
    }

    public void PlayerPupilExpand(float timeNeeded)
    {
        PlayAnimationForDuration("EyePupilExpand", timeNeeded);
    }

    private void PlayAnimationForDuration(string animationName, float targetDuration)
    {
        // Get the original duration of the animation
        AnimationClip[] clips = eyeAnimatorInUI.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animationName)
            {
                // Calculate the speed based on target duration
                float playbackSpeed = clip.length / targetDuration;
                eyeAnimatorInUI.SetFloat("PlaybackSpeed", playbackSpeed);
                eyeAnimatorInUI.Play(animationName);
                return; 
            }
        }
    }
}
