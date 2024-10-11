using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static Define;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    private Transform _cropParentTransform;

    private Tilemap _groundDecoration1;
    private Tilemap _groundDecoration2;

    private bool _isFirstTimeSecenLoaded = true;

    private Grid _grid;
    private Dictionary<string, GridPropertyDetails> _gridPropertyDict;
    [SerializeField] SO_GridProperties[] _soGridPropertiesArray = null;
    [SerializeField] Tile[] _dugGround = null;
    [SerializeField] Tile[] _wateredGround = null;


    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        //ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        ISaveableUniqueID = "GridPropertiesManagers";
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISaveableRegister();
        Managers.Event.AfterSceneLoadEvent += AfterSceneLoaded;
        Managers.Event.DayPassedRegistered += DayPassed;

    }

    private void OnDisable()
    {
        ISaveableDeregister();

        Managers.Event.AfterSceneLoadEvent -= AfterSceneLoaded;
        Managers.Event.DayPassedRegistered -= DayPassed;
    }

    private void Start()
    {
        InitialiseGridProperties();
    }

    private void ClearDisplayGroundDecorations()
    {
        _groundDecoration1.ClearAllTiles();
        _groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayAllPlantedCrops()
    {
        // Destory all crops in scene
        Crop[] cropArray;
        cropArray = FindObjectsOfType<Crop>();

        foreach(Crop crop in cropArray)
        {
            Managers.Resource.Destroy(crop.gameObject);
        }    
    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecorations();

        ClearDisplayAllPlantedCrops();
    }

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        if(gridPropertyDetails.daysSinceDug >-1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }

    public void DisplayWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        // Watered
        if(gridPropertyDetails.daysSinceWatered >-1)
        {
            ConnectWaterGround(gridPropertyDetails);
        }
    }

   public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        if(gridPropertyDetails.seedItemId >-1)
        {
            CropData cropData = null;

            if (Managers.Data.CropDict.TryGetValue(gridPropertyDetails.seedItemId, out cropData))
            {
                GameObject cropObject;

                 int currentGrowthStage = 0;
                int daysCounter = cropData.totalGrowthDays;

                for (int i = cropData.growthStages.Length -1; i>=0; i--)
                {
                    if(gridPropertyDetails.growthDays>= daysCounter)
                    {
                        currentGrowthStage = i;
                        break;
                    }
                    daysCounter = daysCounter - cropData.growthStages[i];
                }

                Vector3 worldPosition = _groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
                worldPosition = new Vector3(worldPosition.x + Define.GridCellSize / 2, worldPosition.y, worldPosition.z);

                Sprite[] sprites = cropData.GetGrowthSprites();

                if (currentGrowthStage == 4)
                {
                    switch (gridPropertyDetails.seedItemId)
                    {
                        case 224:
                            cropObject = Managers.Resource.Instantiate(OakTreePrefabPath, _cropParentTransform);
                            break;
                        case 225:
                            cropObject = Managers.Resource.Instantiate(MapleTreePrefabPath, _cropParentTransform);
                            break;
                        case 226:
                            cropObject = Managers.Resource.Instantiate(PineTreePrefabPath, _cropParentTransform);
                            break;
                        default:
                            cropObject = Managers.Resource.Instantiate(CropStandardPrefabPath, _cropParentTransform);
                            cropObject.GetComponentInChildren<SpriteRenderer>().sprite = sprites[currentGrowthStage];
                            break;
                    }
                }
                else
                {
                    cropObject = Managers.Resource.Instantiate(CropStandardPrefabPath, _cropParentTransform);
                    cropObject.GetComponentInChildren<SpriteRenderer>().sprite = sprites[currentGrowthStage];
                }

                cropObject.transform.position = worldPosition;
                cropObject.GetComponent<Crop>()._cropGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            }    
        }
    }


    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        // Select tile based on surrounding dug tiles

        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        _groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);

        // Set 4 tiles if dug surrounding current tile - up, down, left, right now that this central tile has been dug

        GridPropertyDetails adjacentGridPropertyDetails;

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            _groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), dugTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile2 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            _groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), dugTile2);
        }
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile3 = SetDugTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            _groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), dugTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile4 = SetDugTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            _groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), dugTile4);
        }
    }

    private void ConnectWaterGround(GridPropertyDetails gridPropertyDetails)
    {
        // Select tile based on surrounding watered tiles

        Tile wateredTile0 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        _groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), wateredTile0);

        // Set 4 tiles if watered surrounding current tile - up, down, left, right now that this central tile has been watered

        GridPropertyDetails adjacentGridPropertyDetails;

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile1 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            _groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), wateredTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile2 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            _groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), wateredTile2);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile3 = SetWateredTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            _groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), wateredTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile4 = SetWateredTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            _groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), wateredTile4);
        }
    }
    private Tile SetDugTile(int xGrid, int yGrid)
    {
        //Get whether surrounding tiles (up,down,left, and right) are dug or not

        bool upDug = IsGridSquareDug(xGrid, yGrid + 1);
        bool downDug = IsGridSquareDug(xGrid, yGrid - 1);
        bool leftDug = IsGridSquareDug(xGrid - 1, yGrid);
        bool rightDug = IsGridSquareDug(xGrid + 1, yGrid);

        #region Set appropriate tile based on whether surrounding tiles are dug or not

        if (!upDug && !downDug && !rightDug && !leftDug)
        {
            return _dugGround[0];
        }
        else if (!upDug && downDug && rightDug && !leftDug)
        {
            return _dugGround[1];
        }
        else if (!upDug && downDug && rightDug && leftDug)
        {
            return _dugGround[2];
        }
        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return _dugGround[3];
        }
        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return _dugGround[4];
        }
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return _dugGround[5];
        }
        else if (upDug && downDug && rightDug && leftDug)
        {
            return _dugGround[6];
        }
        else if (upDug && downDug && !rightDug && leftDug)
        {
            return _dugGround[7];
        }
        else if (upDug && downDug && !rightDug && !leftDug)
        {
            return _dugGround[8];
        }
        else if (upDug && !downDug && rightDug && !leftDug)
        {
            return _dugGround[9];
        }
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return _dugGround[10];
        }
        else if (upDug && !downDug && !rightDug && leftDug)
        {
            return _dugGround[11];
        }
        else if (upDug && !downDug && !rightDug && !leftDug)
        {
            return _dugGround[12];
        }
        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return _dugGround[13];
        }
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return _dugGround[14];
        }
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return _dugGround[15];
        }

        return null;

        #endregion Set appropriate tile based on whether surrounding tiles are dug or not
    }
    private Tile SetWateredTile(int xGrid, int yGrid)
    {
        // Get whether surrounding tiles (up,down,left, and right) are watered or not

        bool upWatered = IsGridSquareWatered(xGrid, yGrid + 1);
        bool downWatered = IsGridSquareWatered(xGrid, yGrid - 1);
        bool leftWatered = IsGridSquareWatered(xGrid - 1, yGrid);
        bool rightWatered = IsGridSquareWatered(xGrid + 1, yGrid);

        #region Set appropriate tile based on whether surrounding tiles are watered or not

        if (!upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return _wateredGround[0];
        }
        else if (!upWatered && downWatered && rightWatered && !leftWatered)
        {
            return _wateredGround[1];
        }
        else if (!upWatered && downWatered && rightWatered && leftWatered)
        {
            return _wateredGround[2];
        }
        else if (!upWatered && downWatered && !rightWatered && leftWatered)
        {
            return _wateredGround[3];
        }
        else if (!upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return _wateredGround[4];
        }
        else if (upWatered && downWatered && rightWatered && !leftWatered)
        {
            return _wateredGround[5];
        }
        else if (upWatered && downWatered && rightWatered && leftWatered)
        {
            return _wateredGround[6];
        }
        else if (upWatered && downWatered && !rightWatered && leftWatered)
        {
            return _wateredGround[7];
        }
        else if (upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return _wateredGround[8];
        }
        else if (upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return _wateredGround[9];
        }
        else if (upWatered && !downWatered && rightWatered && leftWatered)
        {
            return _wateredGround[10];
        }
        else if (upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return _wateredGround[11];
        }
        else if (upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return _wateredGround[12];
        }
        else if (!upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return _wateredGround[13];
        }
        else if (!upWatered && !downWatered && rightWatered && leftWatered)
        {
            return _wateredGround[14];
        }
        else if (!upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return _wateredGround[15];
        }

        return null;

        #endregion Set appropriate tile based on whether surrounding tiles are watered or not
    }

    private bool IsGridSquareDug(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daysSinceDug > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsGridSquareWatered(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.daysSinceWatered > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DisplayGridPropertyDetalils()
    {

        foreach (KeyValuePair<string, GridPropertyDetails> item in _gridPropertyDict)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;
            DisplayDugGround(gridPropertyDetails);
            DisplayWateredGround(gridPropertyDetails);
            DisplayPlantedCrop(gridPropertyDetails);
        }
    }
    /// <summary>
    /// This initialises the _grid property dictionary with the values from the SO_GridProperties assets and stores the values for each scene in
    /// GameObjectSave sceneData
    /// </summary>
    private void InitialiseGridProperties()
    {
        // Loop through all gridproperties in the array
        foreach (SO_GridProperties so_GridProperties in _soGridPropertiesArray)
        {
            // Create dictionary of _grid property details
            Dictionary<string, GridPropertyDetails> _gridPropertyDict = new Dictionary<string, GridPropertyDetails>();

            // Populate _grid property dictionary - Iterate through all the _grid properties in the so gridproperties list
            foreach (GridProperty gridProperty in so_GridProperties.gridPropertyList)
            {
                GridPropertyDetails gridPropertyDetails;

                gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, _gridPropertyDict);

                if (gridPropertyDetails == null)
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }

                switch (gridProperty.gridBoolProperty)
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

                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, _gridPropertyDict);
            }

            // Create scene save for this gameobject
            SceneSave sceneSave = new SceneSave();

            // Add _grid property dictionary to scene save data
            sceneSave._griPropertyDetailDict = _gridPropertyDict;

            // If starting scene set the _gridPropertyDict member variable to the current iteration
            if (so_GridProperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this._gridPropertyDict = _gridPropertyDict;
            }

            sceneSave._boolDictionary = new Dictionary<string, bool>();
            sceneSave._boolDictionary.Add("isFirstTimeSceneLoaded", true);


            // Add scene save to game object scene data
            GameObjectSave.sceneData.Add(so_GridProperties.sceneName.ToString(), sceneSave);
        }
    }
    private void AfterSceneLoaded()
    {
        if(GameObject.FindGameObjectWithTag(Tags.CropsParentTransform)!= null)
        {
            _cropParentTransform = GameObject.FindGameObjectWithTag(Tags.CropsParentTransform).transform;
        }
        else
        {
            _cropParentTransform=null;
        }

        // Get Grid
        _grid = GameObject.FindObjectOfType<Grid>();

        // Get Tilemaps

        _groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        _groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
    }

    /// <summary>
    /// Returns the gridPropertyDetails at the gridlocation for the supplied dictionary, or null if no properties exist at that location.
    /// </summary>
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> _gridPropertyDict)
    {
        // Construct key from coordinate
        string key = "x" + gridX + "y" + gridY;

        GridPropertyDetails gridPropertyDetails;

        // Check if _grid property details exist forcoordinate and retrieve
        if (!_gridPropertyDict.TryGetValue(key, out gridPropertyDetails))
        {
            // if not found
            return null;
        }
        else
        {
            return gridPropertyDetails;
        }
    }

    public bool GetGridDimensions(Define.Scene sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin )
    {
        gridDimensions = Vector2Int.zero;
        gridOrigin = Vector2Int.zero;

        foreach(SO_GridProperties so_GridProperties in _soGridPropertiesArray)
        {
            if(so_GridProperties.sceneName == sceneName)
            {
                gridDimensions.x = so_GridProperties.gridWidth;
                gridDimensions.y = so_GridProperties.gridHeight;

                gridOrigin.x = so_GridProperties.originX;
                gridOrigin.y = so_GridProperties.originY;

                return true;
            }
        }

        return false;
    }    

    public Crop GetCropObjectAtGridLocation(GridPropertyDetails gridPropertyDetails)
    {
        Vector3 worldPosition = _grid.GetCellCenterWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(worldPosition);

        Crop crop = null;
        for (int i = 0; i < collider2DArray.Length; i++)
        {
            crop = collider2DArray[i].gameObject.GetComponentInParent<Crop>();
            if (crop != null && crop._cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;

            crop = collider2DArray[i].gameObject.GetComponentInChildren<Crop>();
            if (crop != null && crop._cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
        }
        return crop;
    }

    
    /// <summary>
    /// Get the _grid property details for the tile at (gridX,gridY).  If no _grid property details exist null is returned and can assume that all _grid property details values are null or false
    /// </summary>
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, _gridPropertyDict);
    }
    public void SetGridPropertyDict(Dictionary<string, GridPropertyDetails> gridPropertyDict)
    {
        this._gridPropertyDict = gridPropertyDict;

    }
    public void ISaveableRegister()
    {
      Managers.Save.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
       Managers.Save.iSaveableObjectList.Remove(this);
    }
    public GameObjectSave ISaveableSave()
    {
        // Store current scene data
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave._gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        // Remove sceneSave for scene
        GameObjectSave.sceneData.Remove(sceneName);

        // Create sceneSave for scene
        SceneSave sceneSave = new SceneSave();

        // create & add dict _grid property details dictionary
        sceneSave._griPropertyDetailDict = _gridPropertyDict;

        sceneSave._boolDictionary = new Dictionary<string, bool>();
        sceneSave._boolDictionary.Add("isFirstTimeSceneLoaded", _isFirstTimeSecenLoaded);

        // Add scene save to game object scene data
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // Get sceneSave for scene - it exists since we created it in initialise
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            // get _grid property details dictionary - it exists since we created it in initialise
            if (sceneSave._griPropertyDetailDict != null)
            {
                _gridPropertyDict = sceneSave._griPropertyDetailDict;
            }

            if(sceneSave._boolDictionary != null && sceneSave._boolDictionary.TryGetValue("isFirstTimeSceneLoaded", out bool storedIsFirstTimeSceneLoaded))
            {
                _isFirstTimeSecenLoaded = storedIsFirstTimeSceneLoaded;
            }

            if (_isFirstTimeSecenLoaded)
                Managers.Event.InstantiateCrops();


            // if grid properties exist
            if(_gridPropertyDict.Count>0)
            {
                ClearDisplayGridPropertyDetails();
                DisplayGridPropertyDetalils();

            }

            if (_isFirstTimeSecenLoaded == true)
                _isFirstTimeSecenLoaded = false;

        }
    }


    /// <summary>
    /// Set the _grid property details to gridPropertyDetails for the tile at (gridX,gridY) for current scene
    /// </summary>
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, _gridPropertyDict);
    }

    /// <summary>
    /// Set the _grid property details to gridPropertyDetails for the tile at (gridX,gridY) for the gridpropertyDictionary.
    /// </summary>
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> _gridPropertyDict)
    {
        // Construct key from coordinate
        string key = "x" + gridX + "y" + gridY;

        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        // Set value
        _gridPropertyDict[key] = gridPropertyDetails;
    }

    private void DayPassed()
    {
        ClearDisplayGridPropertyDetails();

        foreach(SO_GridProperties so_GridProperties in _soGridPropertiesArray)
        {
            if(GameObjectSave.sceneData.TryGetValue(so_GridProperties.sceneName.ToString(), out SceneSave sceneSave))
            {
                if(sceneSave._griPropertyDetailDict != null)
                {
                    for(int i  = sceneSave._griPropertyDetailDict.Count -1; i>=0; i--)
                    {
                        KeyValuePair<string, GridPropertyDetails> item = sceneSave._griPropertyDetailDict.ElementAt(i);

                        GridPropertyDetails gridPropertyDetails = item.Value;

                        #region Update all grid properties to reflect the advance in the day

                        // If a crop is planted
                        if (gridPropertyDetails.growthDays > -1)
                        {
                            gridPropertyDetails.growthDays += 1;
                        }

                        // If ground is watered, then clear water
                        if (gridPropertyDetails.daysSinceWatered > -1)
                        {
                            gridPropertyDetails.daysSinceWatered = -1;
                        }

                        // Set gridpropertydetails
                        SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave._griPropertyDetailDict);

                        #endregion Update all grid properties to reflect the advance in the day
                    }
                }
            }
        }
        
        // Update changed values
        DisplayGridPropertyDetalils();
    }


}
