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
    private string currentAnimationName;

    private void Start()
    {
    }

    public void UpdateAnimationStates()
    {

    }

    public void PlayEyeClose()
    {
        eyeAnimatorInUI.Play("EyeLidClose");
    }

    public void PlayEyeOpen(float timeNeeded)
    {
        PlayAnimationForDuration("EyeLidOpen", timeNeeded);
    }

    public void PlayEyeAttackMode(float timeNeeded)
    {
        PlayAnimationForDuration("EyeAttackMode", timeNeeded);
    }

    public void PlayPupilExpand(float timeNeeded, bool playInReverse)
    {
        if (playInReverse)
            ReverseAnimationFromCurrentPosition("EyePupilExpand", timeNeeded);
        else
        {
            PlayAnimationForDuration("EyePupilExpand", timeNeeded);
        }
    }

    // Check if a specific animation is currently playing
    public bool IsAnimationPlaying(string animationName)
    {
        AnimatorStateInfo stateInfo = eyeAnimatorInUI.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName);
    }

    // Plays animation for x seconds
    // Continutes an animation if its already playing for the x seconds
    public void PlayAnimationForDuration(string animationName, float targetDuration)
    {
        AnimationClip clip = FindClipByName(animationName);
        if (clip == null)
        {
            Debug.LogWarning($"Animation {animationName} not found!");
            return;
        }

        float playbackSpeed = clip.length / targetDuration;

        float startingNormalizedTime = 0f;  // Default to start of animation
        if (IsAnimationPlaying(animationName))
        {
            AnimatorStateInfo stateInfo = eyeAnimatorInUI.GetCurrentAnimatorStateInfo(0);
            startingNormalizedTime = stateInfo.normalizedTime % 1;  // Current position in animation
        }

        eyeAnimatorInUI.SetFloat("PlaybackSpeed", playbackSpeed);
        eyeAnimatorInUI.Play(animationName, 0, startingNormalizedTime);
        currentAnimationName = animationName;
    }

    public void ReverseAnimationFromCurrentPosition(string animationName, float targetDuration)
    {
        AnimationClip clip = FindClipByName(animationName);
        if (clip == null)
        {
            Debug.LogWarning($"Animation {animationName} not found!");
            return;
        }

        if (currentAnimationName != animationName)
            return;

        AnimatorStateInfo stateInfo = eyeAnimatorInUI.GetCurrentAnimatorStateInfo(0);
        float currentNormalizedTime = stateInfo.normalizedTime % 1;  // Ensure it's between 0 and 1

        float playbackSpeed = -(clip.length / targetDuration);  // Negative for reversing

        // Adjust for how Unity treats negative playback speed
        currentNormalizedTime -= playbackSpeed * Time.deltaTime / clip.length;
        if (currentNormalizedTime < 0)
            currentNormalizedTime += 1;  // Ensure it's between 0 and 1

        eyeAnimatorInUI.SetFloat("PlaybackSpeed", playbackSpeed);
        eyeAnimatorInUI.Play(animationName, 0, currentNormalizedTime);
    }

    private AnimationClip FindClipByName(string clipName)
    {
        AnimationClip[] clips = eyeAnimatorInUI.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
            {
                return clip;
            }
        }
        return null;
    }
}
