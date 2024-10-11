using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Define;

[Serializable]
public class CropData 
{
    public int seedId; // seed itemId

    public string cropPrefabPath;
    public string growthStagesString; // rawData
    public int[] growthStages;
    public int totalGrowthDays;

    public string growthStageSpritePathString; // rawData
    public Sprite[] growthStageSprites;

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

    public string cropProduceItemString; // rawData
    public int[] cropProducedItemIds;
  
    public int cropProducedMinQuantity;
    public int cropProducedMaxQuantity;

    public int daysToRegrow;

    public void DataInit()
    {
        SetGrowthStages();
        SetProduceItems();
    }

    void SetGrowthStages()
    {
        growthStagesString = growthStagesString.TrimEnd();
        string[] growthParts = growthStagesString.Split(' ');
        growthStages = new int[growthParts.Length];
        for (int i = 0; i < growthParts.Length; i++)
        {
            growthStages[i] = int.Parse(growthParts[i]);
        }
    }

    //public int[] GetGrowthStages()
    //{
    //    int[] growthStages = {growthStage1,growthStage2,growthStage3,growthStage4,growthStage5 };

    //    return growthStages;
    //}

    public Sprite[] GetGrowthSprites()
    {
        if (growthStageSprites == null)
        {
            string[] paths = growthStageSpritePathString.Split(" ");
            Sprite[] growthSprites = new Sprite[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                growthSprites[i] = Managers.Data.GetSpriteByName(paths[i]);
            }
            growthStageSprites = growthSprites;
            return growthSprites;
        }
        else 
        {
            return growthStageSprites; 
        }
       
    }
    void SetProduceItems()
    {
        cropProduceItemString = cropProduceItemString.Trim();
        string[] ids = cropProduceItemString.Split(" ");
        cropProducedItemIds = new int[ids.Length];
        for(int i = 0;i < ids.Length; i++)
        {
            cropProducedItemIds[i] = int.Parse(ids[i]);
        }
    }

    public bool CanUseToolToHarvestCrop(int toolId)
    {
        if (toolId == harvestToolItemId)
            return true;

        return false;
    }

    //public int[] GetProduceItems()
    //{
    //    int[] produceItems = { cropProducedItemId1, cropProducedItemId2, cropProducedItemId3 };

    //    return produceItems;
    //}
}
