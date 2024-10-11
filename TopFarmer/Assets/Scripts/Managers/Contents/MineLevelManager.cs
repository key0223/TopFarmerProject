using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public class MineLevelManager : MonoBehaviour
{
    [SerializeField] SO_GridProperties _soGridProperty;
    Dictionary<string, GridPropertyDetails> _gridPropertyDict;

    GameObject _cropsParentTransform;
    Tilemap _groundDecoration2;


    private void Awake()
    {
        InitialiseGridProperties();
        _cropsParentTransform = GameObject.FindGameObjectWithTag("CropsParentTransform");
        _groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
    }

    private void Start()
    {
        SpwanStones();
    }
    void InitialiseGridProperties()
    {
        Dictionary<string, GridPropertyDetails> gridPropertyDict = new Dictionary<string, GridPropertyDetails>();

        foreach (GridProperty gridProperty in _soGridProperty.gridPropertyList)
        {
            GridPropertyDetails gridPropertyDetails = new GridPropertyDetails();

            switch(gridProperty.gridBoolProperty)
            {
                case GridBoolProperty.Diggable:
                    gridPropertyDetails.isDiggable = gridProperty.gridBoolValue;
                    break;

                case GridBoolProperty.CanDropItem:
                    gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                    break;

                case GridBoolProperty.CanPlaceFurniture:
                    gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                    break;

                case GridBoolProperty.IsPath:
                    gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                    break;

                case GridBoolProperty.IsNPCObstacle:
                    gridPropertyDetails.isNPCObstacle = gridProperty.gridBoolValue;
                    break;
                case GridBoolProperty.CanSpawnMonster:
                    gridPropertyDetails.canSpawnMonster = gridProperty.gridBoolValue;
                    break;

                default:
                    break;
            }
            GridPropertiesManager.Instance.SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, gridPropertyDict);
            _gridPropertyDict = gridPropertyDict;

            GridPropertiesManager.Instance.SetGridPropertyDict(_gridPropertyDict);
        }
    }

    void SpwanStones()
    {
        foreach(var pair in _gridPropertyDict)
        {
            GridPropertyDetails detail = pair.Value;
            
            if(detail.canDropItem)
            {
                float randomValue = Random.Range(0f,1f);

                if(randomValue <=0.3f) // //30% 확률로 생성
                {
                    int randStone = Random.Range(230, 234);

                    CropData cropData = null;

                    if (Managers.Data.CropDict.TryGetValue(randStone, out cropData))
                    {
                        detail.seedItemId = randStone;
                        detail.growthDays = 0;

                        GameObject stone;

                        int[] growthStages = cropData.GetGrowthStages();
                        Sprite[] growthSprites = cropData.GetGrowthSprites();

                        int currentGrowthStage = 0;
                        int daysCounter = cropData.totalGrowthDays;

                        for (int i = growthStages.Length - 1; i >= 0; i--)
                        {
                            if (detail.growthDays >= daysCounter)
                            {
                                currentGrowthStage = i;
                                break;
                            }
                            daysCounter = daysCounter - growthStages[i];
                        }

                        Vector3 worldPosition = _groundDecoration2.CellToWorld(new Vector3Int(detail.gridX, detail.gridY, 0));
                        worldPosition = new Vector3(worldPosition.x + Define.GridCellSize / 2, worldPosition.y, worldPosition.z);

                        stone = Managers.Resource.Instantiate("Object/Item/StoneStandard", _cropsParentTransform.transform);
                        stone.GetComponentInChildren<SpriteRenderer>().sprite = growthSprites[currentGrowthStage];
                        stone.transform.position = worldPosition;
                        stone.GetComponent<Crop>()._cropGridPosition = new Vector2Int(detail.gridX, detail.gridY);
                    }
                }
               

                // 돌 프리팹을 해당 위치에 생성
                
             
            }
        }
    }
}
