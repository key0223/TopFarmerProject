using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class VFXManager
{
    #region Monster
    public void OnMonsterDeath(MonsterType monsterType, Vector3 position)
    {
        switch (monsterType)
        {
            case MonsterType.MONSTER_SLIME:
                {
                    GameObject effect = Managers.Resource.Instantiate("Effect/SlimeDeathEffect");
                    effect.transform.position = position;
                    DestroyItSelf destroy = effect.GetOrAddComponent<DestroyItSelf>();
                    destroy.Destory(1f);
                }

                break;
            case MonsterType.MONSTER_BUG:
                {
                    GameObject effect = Managers.Resource.Instantiate("Effect/BugDeathEffect");
                    effect.transform.position = position;
                    DestroyItSelf destroy = effect.GetOrAddComponent<DestroyItSelf>();
                    destroy.Destory(1f);
                }
                break;
        }

    }

    public void OnMonsterDeath(MonsterType monsterType, Vector3 position, Color color)
    {
        switch (monsterType)
        {
            case MonsterType.MONSTER_SLIME:
                GameObject effect = Managers.Resource.Instantiate("Effect/SlimeDeathEffect");
                effect.transform.position = position;

                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.startColor = color;

                // TODO: Effect destroy
                DestroyItSelf destroy = effect.GetOrAddComponent<DestroyItSelf>();
                destroy.Destory(1f);
                break;
        }


    }
    #endregion

    #region Crop
    public void OnHarvestCrop(HarvestEffectType effectType, Vector3 position)
    {
        switch (effectType)
        {
            case HarvestEffectType.EFFECT_BREAKING_STONE:
                {
                    GameObject effect = Managers.Resource.Instantiate("Effect/BreakingStone");
                    effect.transform.position = position;
                    DestroyItSelf destroy = effect.GetOrAddComponent<DestroyItSelf>();
                    destroy.Destory(1f);
                }
                break;
         
        }

    }
    #endregion


}
