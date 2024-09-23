using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class GridCursor : MonoBehaviour
{
    private Canvas _canvas;
    private Grid _gird;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;

    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private int _itemUseGridRadius = 0;
    public int ItemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value; } // How far frome the player it can be used

    private ItemType _selectedItemType;
    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    private bool _cursorIsEnabled = false;
    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }

    private void OnEnable()
    {
        Managers.Event.AfterSceneLoadEvent += SceneLoaded;
    }
    private void OnDisable()
    {
        Managers.Event.AfterSceneLoadEvent -= SceneLoaded;
    }

    // Start is called before the first frame update
    private void Start()
    {
        mainCamera = Camera.main;
        _canvas = GetComponentInParent<Canvas>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private Vector3Int DisplayCursor()
    {
        if (_gird != null)
        {
            // Get _gird position for cursor
            Vector3Int gridPosition = GetGridPositionForCursor();

            // Get _gird position for player
            Vector3Int playerGridPosition = GetGridPositionForPlayer();

            // Set cursor sprite
            SetCursorValidity(gridPosition, playerGridPosition);

            // Get rect transform position for cursor
            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

            return gridPosition;
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    private void SceneLoaded()
    {
        _gird = GameObject.FindObjectOfType<Grid>();
    }

    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid();

        // Check item use radius is valid
        if (Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius
            || Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            SetCursorToInvalid();
            return;
        }

        // Get selected item details
        ItemData itemData = InventoryManager.Instance.GetSelectedInventoryItemData(InventoryType.INVEN_PLAYER);

        if (itemData == null)
        {
            SetCursorToInvalid();
            return;
        }

        // Get _gird property details at cursor position
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if (gridPropertyDetails != null)
        {
            // Determine cursor validity based on inventory item selected and _gird property details
            switch (itemData.itemType)
            {
                case ItemType.ITEM_SEED:
                    if (!IsCursorValidForSeed(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.ITEM_COMODITY:

                    if (!IsCursorValidForCommodity(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.ITEM_TOOL_WATERING:
                    if (!IsCursorValidForTool(gridPropertyDetails, itemData))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.ITEM_TOOL_HOEING:

                    if (!IsCursorValidForTool(gridPropertyDetails, itemData))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.ITEM_TOOL_AXE:
                case ItemType.ITEM_TOOL_PICKAXE:
                case ItemType.ITEM_TOOL_SCYTHE:
                    if (!IsCursorValidForTool(gridPropertyDetails, itemData))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.ITEM_TOOL_COLLECTING:
                    break;
                case ItemType.NONE:
                    if (!IsCursorValidForCrop(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.COUNT:
                    break;

                default:
                    break;
            }
        }
        else
        {
            SetCursorToInvalid();
            return;
        }
    }

    /// <summary>
    /// Set the cursor to be invalid
    /// </summary>
    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }

    /// <summary>
    /// Set the cursor to be valid
    /// </summary>
    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }

    /// <summary>
    /// Test cursor validity for a commodity for the target gridPropertyDetails. Returns true if valid, false if invalid
    /// </summary>
    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    /// <summary>
    /// Set cursor validity for a seed for the target gridPropertyDetails. Returns true if valid, false if invalid
    /// </summary>
    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }
    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemData itemData)
    {
        switch (itemData.itemType)
        {
            case ItemType.ITEM_TOOL_WATERING:
                {
                    if (gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.daysSinceWatered == -1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case ItemType.ITEM_TOOL_HOEING:
                {
                    if (gridPropertyDetails.isDiggable == true && gridPropertyDetails.daysSinceDug == -1)
                    {
                        Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                        List<Item> itemList = new List<Item>();

                        HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition, Define.CursorSize, 0f);

                        bool foundReapable = false;

                        foreach (Item item in itemList)
                        {
                            if (Managers.Data.GetItemData(item.ItemId).itemType == ItemType.ITEM_REAPABLE_SCENARY)
                            {
                                foundReapable = true;
                                break;
                            }
                        }

                        if (foundReapable)
                            return false;
                        else
                            return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case ItemType.ITEM_TOOL_AXE:
                {
                    if (gridPropertyDetails.seedItemId != -1)
                    {
                        CropData cropData = null;
                        if (Managers.Data.CropDict.TryGetValue(gridPropertyDetails.seedItemId, out cropData))
                        {
                            if (gridPropertyDetails.growthDays >= cropData.totalGrowthDays)
                            {
                                if (cropData.CanUseToolToHarvestCrop(itemData.itemId))
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    return false;
                }
            case ItemType.ITEM_TOOL_COLLECTING:
                {
                    if (gridPropertyDetails.seedItemId != -1)
                    {
                        CropData cropData = null;
                        if (Managers.Data.CropDict.TryGetValue(gridPropertyDetails.seedItemId, out cropData))
                        {
                            if (gridPropertyDetails.growthDays >= cropData.totalGrowthDays)
                            {
                                if (cropData.harvestToolItemId == -1)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    return false;
                }
            default:
                return false;
        }
    }
    private bool IsCursorValidForCrop(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.seedItemId != -1)
        {
            CropData cropData = null;
            if (Managers.Data.CropDict.TryGetValue(gridPropertyDetails.seedItemId, out cropData))
            {
                if (gridPropertyDetails.growthDays >= cropData.totalGrowthDays)
                {
                    if (cropData.harvestToolItemId == null)
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }
        }

        return false;
    }
    public void DisableCursor()
    {
        cursorImage.color = Color.clear;

        CursorIsEnabled = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnabled = true;
    }

    public Vector3Int GetGridPositionForCursor()
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));  // z is how far the objects are in front of the camera - camera is at -10 so objects are (-)-10 in front = 10
        return _gird.WorldToCell(worldPosition);
    }

    public Vector3Int GetGridPositionForPlayer()
    {
        return _gird.WorldToCell(PlayerController.Instance.transform.position);
    }

    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPosition = _gird.CellToWorld(gridPosition);
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, _canvas);
    }

    public Vector3 GetWorldPositionForCursor()
    {
        return _gird.CellToWorld(GetGridPositionForCursor());
    }
}