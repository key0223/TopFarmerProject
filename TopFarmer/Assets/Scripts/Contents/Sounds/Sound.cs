using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void SetSound(SoundItem soundItem)
    {
        _audioSource.pitch = Random.Range(soundItem._soundPitchRandomVariationMin, soundItem._soundPitchRandomVariationMax);
        _audioSource.volume = soundItem._soundVolume;
        _audioSource.clip = soundItem._soundClip;

        if (_audioSource.clip != null)
        {
            _audioSource.Play();
        }
    }

    private void OnEnable()
    {
        //if(_audioSource.clip != null)
        //{
        //    _audioSource.Play();
        //}
    }

    private void OnDisable()
    {
        _audioSource.Stop();
    }
}
