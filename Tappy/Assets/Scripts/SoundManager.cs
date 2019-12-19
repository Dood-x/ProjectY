using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> pussycatDollsClips;
    public AudioSource audioSource;

    public bool muted;

    public void PlayHushHush()
    {
        audioSource.clip = pussycatDollsClips[0];
        audioSource.Play();
    }
    public void PlayBeep()
    {
        audioSource.clip = pussycatDollsClips[3];
        audioSource.Play();
    }
    public void PlayDontCha()
    {
        audioSource.clip = pussycatDollsClips[1];
        audioSource.Play();
    }
    public void PlayButtons()
    {
        audioSource.clip = pussycatDollsClips[2];
        audioSource.Play();
    }
}
