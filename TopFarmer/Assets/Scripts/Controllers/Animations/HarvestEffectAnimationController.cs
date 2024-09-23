using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class HarvestEffectAnimationController : MonoBehaviour
{
    SpriteRenderer _sprite;
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    public void StartAnimation(Vector3 effectPosition, HarvestEffectType effectType)
    {
        string animationName = GetToolAnimationName(effectType);

        _animator.Play($"{animationName}_EFFECT");
        gameObject.transform.position = effectPosition;
        StartCoroutine(CoDisableEffect());
    }

    IEnumerator CoDisableEffect()
    {
        yield return new WaitForSeconds(0.6F);

        Managers.Resource.Destroy(gameObject);

    }
    string GetToolAnimationName(HarvestEffectType effectType)
    {
        string animationName = "";

        switch (effectType)
        {
            case HarvestEffectType.NONE:
                animationName = "NONE";
                break;
            case HarvestEffectType.EFFECT_REAPING:
                animationName = "REAPING";
                break;
            case HarvestEffectType.EFFECT_WEED:
                animationName = "WEED";
                break;
            

        }

        return animationName;
    }
}
