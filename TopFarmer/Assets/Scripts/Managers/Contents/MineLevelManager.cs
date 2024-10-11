using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

[System.Serializable]
public class StoneProbability
{
    public int _stoneId;
    public float _probability;
}
public class MineLevelManager : MonoBehaviour
{
    [SerializeField] SO_GridProperties _soGridProperty;
    Dictionary<string, GridPropertyDetails> _gridPropertyDict;

    GameObject _cropsParentTransform;
    Tilemap _groundDecoration2;

    [SerializeField] List<StoneProbability> _stoneProbabilities;
    float _totalProbability;
    private void Awake()
    {
        InitialiseGridProperties();
        _cropsParentTransform = GameObject.FindGameObjectWithTag("CropsParentTransform");
        _groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
    }

    private void Start()
    {
        _totalProbability = CalculateTotalProbability();
        SpwanStones();
    }
    void InitialiseGridProperties()
    {
        Dictionary<string, GridPropertyDetails> gridPropertyDict = new Dictionary<string, GridPropertyDetails>();

        foreach (GridProperty gridProperty in _soGridProperty.gridPropertyList)
        {
            GridPropertyDetails gridPropertyDetails;

            gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDict);

            if(gridPropertyDetails == null)
            {
                gridPropertyDetails = new GridPropertyDetails();
            }

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
        }
        _gridPropertyDict = gridPropertyDict;
        GridPropertiesManager.Instance.SetGridPropertyDict(_gridPropertyDict);
    }

    [ContextMenu("Respwan")]
    public void ReSpwan()
    {
        for (int i = _cropsParentTransform.transform.childCount - 1; i >= 0; i--)
        {
            // 자식 오브젝트를 삭제
            GameObject child = _cropsParentTransform.transform.GetChild(i).gameObject;
            Destroy(child);
        }

        SpwanStones();
    }
    void SpwanStones()
    {
        foreach(var pair in _gridPropertyDict)
        {
            GridPropertyDetails detail = pair.Value;
            
            if(detail.canDropItem)
            {
                float randomValue = Random.Range(0f,1f);

                if(randomValue <=0.7f) // 60% 확률로 생성
                {
                    int randStone = GetRandomStone();

                    CropData cropData = null;

                    if (Managers.Data.CropDict.TryGetValue(randStone, out cropData))
                    {
                        detail.seedItemId = randStone;
                        detail.growthDays = 0;

                        GameObject stone;

                        int currentGrowthStage = CalculateCurrentGrowthStage(cropData, detail.growthDays);


                        Vector3 worldPosition = _groundDecoration2.CellToWorld(new Vector3Int(detail.gridX, detail.gridY, 0));
                        worldPosition = new Vector3(worldPosition.x + Define.GridCellSize / 2, worldPosition.y, worldPosition.z);

                        Sprite[] sprites = cropData.GetGrowthSprites();
                        stone = Managers.Resource.Instantiate("Object/Item/StoneStandard", _cropsParentTransform.transform);
                        stone.GetComponentInChildren<SpriteRenderer>().sprite = sprites[currentGrowthStage];
                        stone.transform.position = worldPosition;
                        stone.GetComponent<Crop>()._cropGridPosition = new Vector2Int(detail.gridX, detail.gridY);
                    }
                }
            }
        }
    }

    int CalculateCurrentGrowthStage(CropData cropData, int growthDays)
    {
        int currentGrowthStage = 0;
        int daysCounter = cropData.totalGrowthDays;

        for (int i = cropData.growthStages.Length - 1; i >= 0; i--)
        {
            if (growthDays >= daysCounter)
            {
                currentGrowthStage = i;
                break;
            }
            daysCounter -= cropData.growthStages[i];
        }

        return currentGrowthStage;
    }
    int GetRandomStone()
    {
        float randomValue = Random.Range(0f, _totalProbability);
        float cumulativeProbability = 0f;

        foreach (var stoneProb in _stoneProbabilities)
        {
            cumulativeProbability += stoneProb._probability;

            if (randomValue <= cumulativeProbability)
            {
                return stoneProb._stoneId;
            }
        }

        return _stoneProbabilities[0]._stoneId;
    }
    float CalculateTotalProbability()
    {
        float total = 0f;
        foreach (var stoneProb in _stoneProbabilities)
        {
            total += stoneProb._probability;
        }
        return total;
    }
   
}
