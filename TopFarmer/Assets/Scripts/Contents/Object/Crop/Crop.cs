using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class Crop : MonoBehaviour
{
    [Tooltip("This should be populated from child gameobject")]
    [SerializeField] SpriteRenderer _harvestedSpriteRenderer;

    private int _harvestActionCount = 0;

    [HideInInspector]
    public Vector2Int _cropGridPosition;

    public void ProcessToolAction(ItemData itemData, MoveDir dir)
    {
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(_cropGridPosition.x, _cropGridPosition.y);

        if (gridPropertyDetails == null)
            return;

        ItemData seedItemData = Managers.Data.ItemDict[gridPropertyDetails.seedItemId];
        if (seedItemData == null) return;

        CropData cropData = Managers.Data.CropDict[seedItemData.itemId];
        if (cropData == null) return;

        Animator animator = GetComponentInChildren<Animator>();

        int requireHarvestAction = cropData.requireHarvestAction;
        if (requireHarvestAction == -1) return;

        _harvestActionCount += 1;

        if (_harvestActionCount >= requireHarvestAction)
        {
            HarvestCrop(cropData, gridPropertyDetails, animator);
        }
    }
    private void HarvestCrop(CropData cropData, GridPropertyDetails gridPropertyDetails, Animator animator)
    {

        if (cropData.isHarvestedAnimation && animator != null)
        {
            Sprite harvestSprite = null;
            if (Managers.Data.SpriteDict.TryGetValue(cropData.harvestedSpritePath, out harvestSprite))
            {
                if (_harvestedSpriteRenderer != null)
                {
                    _harvestedSpriteRenderer.sprite = harvestSprite;
                }
            }
        }

        if (cropData.harvestSound != Define.Sound.NONE)
        {
            SoundManager.Instance.PlaySound(cropData.harvestSound);
        }


        // reset the grid properties
        gridPropertyDetails.seedItemId = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        if (cropData.hideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
        if (cropData.disableCropColliderBeforeHarvestedAnimation)
        {
            // Disable any box colliders
            Collider2D[] collider2Ds = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider2D in collider2Ds)
            {
                collider2D.enabled = false;
            }
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        if (cropData.isHarvestedAnimation && animator != null)
        {
            StartCoroutine(ProcessHarvestrActionAfterAnimation(cropData, gridPropertyDetails, animator));
        }
        else
        {
            HarvestAction(cropData, gridPropertyDetails);
        }
    }

    private void HarvestAction(CropData cropData, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestItems(cropData);

        if (cropData.harvedtedTransformItemId > 0)
        {
            CreateHarvestedTransformCrop(cropData, gridPropertyDetails);
        }

        if(cropData.isHarvestEffect)
        {
            Managers.VFX.OnHarvestCrop(cropData.harvestEffectType, transform.position);
        }
        Managers.Resource.Destroy(gameObject);
    }
    private void SpawnHarvestItems(CropData cropData)
    {

        for (int i = 0; i < cropData.cropProducedItemIds.Length; i++)
        {
            if (cropData.cropProducedItemIds[i] == -1)
                continue;

            int cropToProduce;

            if (cropData.cropProducedMinQuantity == cropData.cropProducedMaxQuantity ||
                cropData.cropProducedMaxQuantity < cropData.cropProducedMinQuantity)
            {
                cropToProduce = cropData.cropProducedMinQuantity;
            }
            else
            {
                cropToProduce = Random.Range(cropData.cropProducedMinQuantity, cropData.cropProducedMaxQuantity +1);
            }

            for (int j = 0; j < cropToProduce; j++)
            {
                Vector3 spawnPosition;

                if (cropData.spawnCropProduceAtPlayerPosition)
                {
                    InventoryManager.Instance.AddItem(Define.InventoryType.INVEN_PLAYER, cropData.cropProducedItemIds[i]);
                }
                else
                {
                    spawnPosition = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f), 0f);
                    SceneItemsManager.Instance.InstantiateSceneItems(cropData.cropProducedItemIds[i], spawnPosition);
                }
            }
        }
    }
    private void CreateHarvestedTransformCrop(CropData cropData, GridPropertyDetails gridPropertyDetails)
    {
        // Update crop in grid properties
        gridPropertyDetails.seedItemId = cropData.harvedtedTransformItemId;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // Display planted crop
        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }

    IEnumerator ProcessHarvestrActionAfterAnimation(CropData cropData, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        if (animator != null)
        {
            AnimatorClipInfo[] currentClip = animator.GetCurrentAnimatorClipInfo(0);

            if( currentClip.Length>0)
            {
                yield return new WaitForSeconds(currentClip[0].clip.length);
            }
        }

        HarvestAction(cropData, gridPropertyDetails);
    }
}
