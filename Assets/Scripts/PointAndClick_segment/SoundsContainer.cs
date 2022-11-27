using System.Collections;
using System.Collections.Generic;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

public class SoundsContainer : MonoBehaviour
{
    [SerializeField] private AudioClip[] AudioSources;
    private SoundService soundService;

    private void Awake()
    {
        soundService = ServiceLocator.Get<SoundService>();
    }

    public void PlaySound(int soundIndex)
    {
        if (AudioSources[soundIndex] != null)
        {
            soundService.Sounds.PlayOneShot(AudioSources[soundIndex]);
        }
    }
}
