using System;
using UnityEngine;
using static Define;

[Serializable]
public class SoundItem 
{
    public Define.Sound _soundName;
    public AudioClip _soundClip;
    public string _soundDescription;
    [Range(0.1f, 1.5f)]
    public float _soundPitchRandomVariationMin = 0.8f;
    [Range(0.1f, 1.5f)]
    public float _soundPitchRandomVariationMax = 1.2f;
    [Range(0f, 1f)]
    public float _soundVolume = 1f;
}
