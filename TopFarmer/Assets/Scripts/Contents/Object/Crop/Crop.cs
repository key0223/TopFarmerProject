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

        if(cropData.isHarvestEffect) 
        {

        }
        int requireHarvestAction = cropData.requireHarvestAction;
        if (requireHarvestAction == -1) return;

        _harvestActionCount += 1;

        if (_harvestActionCount >= requireHarvestAction)
        {
            HarvestAnimation(itemData, dir, animator);
            HarvestCrop(cropData, gridPropertyDetails, animator);
        }
        else
        {
            BeforeHarvestAnimation(itemData,dir,animator);
        }
    }

    void BeforeHarvestAnimation(ItemData itemData, MoveDir dir, Animator animator)
    {
        if (itemData.itemType == ItemType.ITEM_TOOL_AXE)
        {
            switch (dir)
            {
                case MoveDir.Up:
                    animator.Play($"TREEWOBBLE_RIGHT");
                    animator.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                case MoveDir.Down:
                    animator.Play($"TREEWOBBLE_RIGHT");
                    animator.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                case MoveDir.Left:
                    animator.Play($"TREEWOBBLE_RIGHT");
                    animator.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                case MoveDir.Right:
                    animator.Play($"TREEWOBBLE_RIGHT");
                    animator.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                case MoveDir.None:
                    break;
            }
        }

    }
    void HarvestAnimation(ItemData itemData,MoveDir dir, Animator animator)
    {
        if(itemData.itemType == ItemType.ITEM_TOOL_AXE)
        {
            switch (dir)
            {
                case MoveDir.Up:
                    animator.Play($"TREEFALLING_RIGHT");
                    animator.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                case MoveDir.Down:
                    animator.Play($"TREEFALLING_RIGHT");
                    animator.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                case MoveDir.Left:
                    animator.Play($"TREEFALLING_RIGHT");
                    animator.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                case MoveDir.Right:
                    animator.Play($"TREEFALLING_RIGHT");
                    animator.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                case MoveDir.None:

                    break;
            }
        }
        else if(itemData.itemType == ItemType.ITEM_TOOL_COLLECTING)
        {
            switch (dir)
            {
                case MoveDir.Up:
                    animator.Play($"HARVEST_CROP_BACK");
                    _harvestedSpriteRenderer.flipX = false;
                    break;
                case MoveDir.Down:
                    animator.Play($"HARVEST_CROP_FRONT");
                    _harvestedSpriteRenderer.flipX = false;
                    break;
                case MoveDir.Left:
                    animator.Play($"HARVEST_CROP_RIGHT");
                    _harvestedSpriteRenderer.flipX = true;
                    break;
                case MoveDir.Right:
                    animator.Play($"HARVEST_CROP_RIGHT");
                    _harvestedSpriteRenderer.flipX = false;
                    break;
                case MoveDir.None:
                    break;
            }
        }
    }
    private void HarvestCrop(CropData cropData, GridPropertyDetails gridPropertyDetails, Animator animator)
    {

        if (cropData.isHarvestedAnimation && animator != null)
        {
            Sprite harvestSprite = null;
            if (Managers.Data.SpriteDict.TryGetValue(cropData.harvestedSpritePath, out harvestSprite))
            {
                if(_harvestedSpriteRenderer != null)
                {
                    _harvestedSpriteRenderer.sprite = harvestSprite;
                }
            }
        }

        if(cropData.harvestSound != Define.Sound.NONE)
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
            HarvestAction(cropData, gridPropertyDetails,animator);
        }
    }

    private void HarvestAction(CropData cropData, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        SpawnHarvestItems(cropData);

        if(cropData.harvedtedTransformItemId >0)
        {
            CreateHarvestedTransformCrop(cropData, gridPropertyDetails);
        }
        StartCoroutine(WaitAndDestroy(animator));
        //Managers.Resource.Destroy(gameObject);
    }

    private void SpawnHarvestItems(CropData cropData)
    {
        int[] produceItems = cropData.GetProduceItems();

        for (int i = 0; i < produceItems.Length; i++)
        {
            if (produceItems[i] == -1)
                continue;

            int cropToProduce = Random.Range(cropData.cropProducedMinQuantity, cropData.cropProducedMaxQuantity);

            for (int j = 0; j < cropToProduce; j++)
            {
                Vector3 spawnPosition;

                if (cropData.spawnCropProduceAtPlayerPosition)
                {
                    InventoryManager.Instance.AddItem(Define.InventoryType.INVEN_PLAYER, produceItems[i]);
                }
                else
                {
                    spawnPosition = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f), 0f);
                    SceneItemsManager.Instance.InstantiateSceneItems(produceItems[i], spawnPosition);
                }
            }
        }
    }

    IEnumerator ProcessHarvestrActionAfterAnimation(CropData cropData, GridPropertyDetails gridPropertyDetails, Animator animator)
    {

        AnimatorClipInfo[] currentClip = animator.GetCurrentAnimatorClipInfo(0);

        yield return new WaitForSeconds(currentClip[0].clip.length);


        HarvestAction(cropData, gridPropertyDetails, animator);
    }
    IEnumerator WaitAndDestroy(Animator animator)
    {
        AnimatorClipInfo[] currentClip = animator.GetCurrentAnimatorClipInfo(0);

        yield return new WaitForSeconds(currentClip[0].clip.length);

        Managers.Resource.Destroy(gameObject);
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

}
