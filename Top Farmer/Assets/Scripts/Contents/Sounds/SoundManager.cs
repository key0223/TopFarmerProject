using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Experimental.AI;
using UnityEngine.SceneManagement;

public class SoundManager : SingletonMonobehaviour<SoundManager>
{

    [Header("Audio Sources")]
    [SerializeField] AudioSource _ambientSoundAudioSource = null;
    [SerializeField] AudioSource _gameMusicAudioSource = null;

    [Header("Audio Mixers")]
    [SerializeField] AudioMixer _gameAudioMixer = null;

    [Header("Audio Snapshots")]
    [SerializeField] AudioMixerSnapshot _gameMusicSnapshot = null;
    [SerializeField] AudioMixerSnapshot _gameAmbientSnapshot = null;


    [Header("Other")]
    [SerializeField] SO_SoundList _soSoundList = null;

    [SerializeField] SO_SceneSoundsList _soSceneSoundList = null;
    [SerializeField] float _defaultSceneMusicPlayTimeSeconds = 120f;
    [SerializeField] float _sceneMusicStartMinSecs = 20f;
    [SerializeField] float _sceneMusicStartMaxSecs = 40f;
    [SerializeField] float _musicTransitionSecs = 8f;


    Dictionary<Define.Sound, SoundItem> _soundDictionary;
    Dictionary<Define.Scene, SceneSoundItem> _sceneSoundDictionay;

    Coroutine _coPlaySceneSound;

    protected override void Awake()
    {
        base.Awake();

        _soundDictionary = new Dictionary<Define.Sound, SoundItem>();
        foreach(SoundItem soundItem in _soSoundList.soundDetails)
        {
            _soundDictionary.Add(soundItem._soundName, soundItem);
        }

        _sceneSoundDictionay = new Dictionary<Define.Scene, SceneSoundItem>();
        foreach(SceneSoundItem sceneSoundItem in _soSceneSoundList.sceneSoundsDatas)
        {
            _sceneSoundDictionay.Add(sceneSoundItem._sceneName, sceneSoundItem);
        }
    }
    private void OnEnable()
    {
        Managers.Event.AfterSceneLoadEvent += PlaySceneSound;

    }
    private void OnDisable()
    {
        Managers.Event.AfterSceneLoadEvent -= PlaySceneSound;
    }
    public void PlaySound(Define.Sound soundName)
    {
        if(_soundDictionary.TryGetValue(soundName , out SoundItem soundItem))
        {
            GameObject soundGameObject = Managers.Resource.Instantiate("Sound/Sound");

            Sound sound = soundGameObject.GetComponent<Sound>();

            sound.SetSound(soundItem);
            //soundGameObject.SetActive(true);
            StartCoroutine(CoDisableSound(soundGameObject, soundItem._soundClip.length));
        }
    }

    IEnumerator CoDisableSound(GameObject soundGameObject, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        Managers.Resource.Destroy(soundGameObject);
    }

    void PlaySceneSound()
    {
        SoundItem musicSoundItem = null;
        SoundItem ambientSoundItem = null;

        float musicPlayTime = _defaultSceneMusicPlayTimeSeconds;

        if(Enum.TryParse<Define.Scene>(SceneManager.GetActiveScene().name,true, out Define.Scene currentSceneName))
        {
            if (_sceneSoundDictionay.TryGetValue(currentSceneName, out SceneSoundItem sceneSoundItem))
            {
                _soundDictionary.TryGetValue(sceneSoundItem._musicForScene, out musicSoundItem);
                _soundDictionary.TryGetValue(sceneSoundItem._ambientSoundForScene, out ambientSoundItem);
            }
            else
            {
                return;
            }

            if(_coPlaySceneSound != null)
            {
                StopCoroutine(_coPlaySceneSound);
            }

            _coPlaySceneSound = StartCoroutine(CoPlaySceneSound(musicPlayTime, musicSoundItem, ambientSoundItem));
        }

    }

    IEnumerator CoPlaySceneSound(float musicPlaySeconds, SoundItem musicSoundItem, SoundItem ambientSoundItem)
    {
        if(musicSoundItem != null && ambientSoundItem != null)
        {
            PlayAmbientSoundClip(ambientSoundItem, 0f);

            yield return new WaitForSeconds(UnityEngine.Random.Range(_sceneMusicStartMinSecs, _sceneMusicStartMaxSecs));

            PlayMusicSoundClip(musicSoundItem, _musicTransitionSecs);

            yield return new WaitForSeconds(musicPlaySeconds);

            PlayAmbientSoundClip(ambientSoundItem, _musicTransitionSecs);

            
        }

    }

    void PlayAmbientSoundClip(SoundItem ambientSoundItem, float transitionTimeSeconds)
    {
        // Set Volume
        _gameAudioMixer.SetFloat("AmbientVolume", ConvertSoundVolumeDecimalFractionToDecibels(ambientSoundItem._soundVolume));

        // Set Clip & Play
        _ambientSoundAudioSource.clip = ambientSoundItem._soundClip;
        _ambientSoundAudioSource.Play();

        // Transition to ambient
        _gameAmbientSnapshot.TransitionTo(transitionTimeSeconds);

    }

    private void PlayMusicSoundClip(SoundItem musicSoundItem, float transitionTimeSeconds)
    {
        // Set Volume
        _gameAudioMixer.SetFloat("MusicVolume", ConvertSoundVolumeDecimalFractionToDecibels(musicSoundItem._soundVolume));

        // Set clip & play
        _gameMusicAudioSource.clip = musicSoundItem._soundClip;
        _gameMusicAudioSource.Play();

        // Transition to music snapshot
        _gameMusicSnapshot.TransitionTo(transitionTimeSeconds);
    }

    /// <summary>
    /// Convert volumeDecimalFraction to -80 to +20 decibel range
    /// </summary>
    private float ConvertSoundVolumeDecimalFractionToDecibels(float volumeDecimalFraction)
    {
        // Convert volume from decimal fraction to -80 to +20 decibel range

        return (volumeDecimalFraction * 100f - 80f);
    }



}
