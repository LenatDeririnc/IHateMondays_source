using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHub : MonoBehaviour
{
    [SerializeField] private SoundsContainer _soundsContainer;

    public void PlaySound(int index)
    {
        _soundsContainer.PlaySound(index);
    }

    public void StopSound()
    {
        _soundsContainer.StopPlay();
    }
}
