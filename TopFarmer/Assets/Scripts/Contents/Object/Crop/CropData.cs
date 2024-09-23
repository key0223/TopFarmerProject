using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[Serializable]
public class CropData 
{
    public int seedId; // seed itemId

    public string cropPrefabPath;

    public int growthStage1;
    public int growthStage2;
    public int growthStage3;
    public int growthStage4;
    public int growthStage5;

    public int totalGrowthDays;

    public string growthStage1SpritePath;
    public string growthStage2SpritePath;
    public string growthStage3SpritePath;
    public string growthStage4SpritePath;
    public string growthStage5SpritePath;

    public string fullyGrownSpritePath;
    public string fullyGrownHarvestedSpritePath;

    public Season season;

    public string harvestedSpritePath;

    public int harvedtedTransformItemId;

    public bool hideCropBeforeHarvestedAnimation;
    public bool disableCropColliderBeforeHarvestedAnimation;
    public bool isHarvestedAnimation;
    public bool isHarvestEffect = false;
    public bool spawnCropProduceAtPlayerPosition;

    public HarvestEffectType harvestEffectType;
    public Define.Sound harvestSound;

    public int harvestToolItemId;
    public int requireHarvestAction;

    public int cropProducedItemId1;
    public int cropProducedItemId2;
    public int cropProducedItemId3;

    public int cropProducedMinQuantity;
    public int cropProducedMaxQuantity;

    public int daysToRegrow;


    public int[] GetGrowthStages()
    {
        int[] growthStages = {growthStage1,growthStage2,growthStage3,growthStage4,growthStage5 };

        return growthStages;
    }

    public Sprite[] GetGrowthSprites()
    {
        Sprite[] growthSprites = {
                    Managers.Data.GetSpriteByName(growthStage1SpritePath),
                    Managers.Data.GetSpriteByName(growthStage2SpritePath),
                    Managers.Data.GetSpriteByName(growthStage3SpritePath),
                    Managers.Data.GetSpriteByName(growthStage4SpritePath),
                    Managers.Data.GetSpriteByName(growthStage5SpritePath)
                };

        return growthSprites;
    }

    public bool CanUseToolToHarvestCrop(int toolId)
    {
        if (toolId == harvestToolItemId)
            return true;

        return false;
    }

    public int[] GetProduceItems()
    {
        int[] produceItems = { cropProducedItemId1, cropProducedItemId2, cropProducedItemId3 };

        return produceItems;
    }
}
