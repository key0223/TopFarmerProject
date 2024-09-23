using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundEvent : MonoBehaviour
{
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
   
    void PlayAnimationEventSound()
    {
        SoundManager.Instance.PlaySound(Define.Sound.FOOTSTEP_HARD_GROUND);
    }
}
