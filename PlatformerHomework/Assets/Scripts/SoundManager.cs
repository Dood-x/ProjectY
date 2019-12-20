using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource audios;

    public AudioClip leap;
    public AudioClip gotHit;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        audios = GetComponent<AudioSource>();
    }

    public void PlayGotHit()
    {
        audios.clip = gotHit;
        audios.Play();
    }

}
