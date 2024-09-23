using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class Cursor : MonoBehaviour
{
    private Canvas _canvas;
    private Camera _mainCamera;

    [SerializeField] private Image _cursorImage = null;
    [SerializeField] private RectTransform _cursorRectTransform = null;


    [SerializeField] private Sprite _greenCursorSprite = null;
    [SerializeField] private Sprite _transparentCursorSprite = null;
    [SerializeField] private GridCursor _gridCursor = null;

    private bool _cursorIsEnabled = false;
    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private ItemType _selectedItemType;
    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    private float _itemUseRadius = 0f;
    public float ItemUseRadius { get => _itemUseRadius; set => _itemUseRadius = value; }

    private void Start()
    {
        _mainCamera = Camera.main;
        _canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {

        //DisplayCursor();
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private void DisplayCursor()
    {
        // Get position for cursor
        Vector3 cursorWorldPosition = GetWorldPositionForCursor();

        // Set cursor sprite
        SetCursorValidity(cursorWorldPosition, PlayerController.Instance.GetPlayerCenterPosition());

        // Get rect transform position for cursor
        _cursorRectTransform.position = GetRectTransformPositionForCursor();
    }

    private void SetCursorValidity(Vector3 cursorPosition, Vector3 playerPosition)
    {
        SetCursorToValid();

        // Check use radius corners

        if (
            cursorPosition.x > (playerPosition.x + ItemUseRadius / 2f) && cursorPosition.y > (playerPosition.y + ItemUseRadius / 2f)
            ||
            cursorPosition.x < (playerPosition.x - ItemUseRadius / 2f) && cursorPosition.y > (playerPosition.y + ItemUseRadius / 2f)
            ||
            cursorPosition.x < (playerPosition.x - ItemUseRadius / 2f) && cursorPosition.y < (playerPosition.y - ItemUseRadius / 2f)
            ||
            cursorPosition.x > (playerPosition.x + ItemUseRadius / 2f) && cursorPosition.y < (playerPosition.y - ItemUseRadius / 2f)
            )

        {
            SetCursorToInvalid();
            return;
        }

        // Check item use radius is valid
        if (Mathf.Abs(cursorPosition.x - playerPosition.x) > ItemUseRadius
            || Mathf.Abs(cursorPosition.y - playerPosition.y) > ItemUseRadius)
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

        // Determine cursor validity based on inventory item selected and what object the cursor is over
        switch (itemData.itemType)
        {
            case ItemType.ITEM_TOOL_WATERING:
            case ItemType.ITEM_TOOL_AXE:
            case ItemType.ITEM_TOOL_PICKAXE:
            case ItemType.ITEM_TOOL_HOEING:
            case ItemType.ITEM_TOOL_SCYTHE:
                if (!SetCursorValidityTool(cursorPosition, playerPosition, itemData))
                {
                    SetCursorToInvalid();
                    return;
                }
                break;
            case ItemType.ITEM_TOOL_COLLECTING:
                if (!SetCursorValidityTool(cursorPosition, playerPosition, itemData))
                {
                    SetCursorToInvalid();
                    return;
                }
                break;

            case ItemType.NONE:
                break;

            case ItemType.COUNT:
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Set the cursor to be valid
    /// </summary>
    private void SetCursorToValid()
    {
        _cursorImage.sprite = _greenCursorSprite;
        CursorPositionIsValid = true;

        _gridCursor.DisableCursor();
    }

    /// <summary>
    /// Set the cursor to be invalid
    /// </summary>
    private void SetCursorToInvalid()
    {
        _cursorImage.sprite = _transparentCursorSprite;
        CursorPositionIsValid = false;

        _gridCursor.EnableCursor();
    }

    /// <summary>
    /// Sets the cursor as either valid or invalid for the tool for the target. Returns true if valid or false if invalid
    /// </summary>
    private bool SetCursorValidityTool(Vector3 cursorPosition, Vector3 playerPosition, ItemData itemData)
    {
        // Switch on tool
        switch (itemData.itemType)
        {
            case ItemType.ITEM_TOOL_SCYTHE:
                return SetCursorValidityReapingTool(cursorPosition, playerPosition, itemData);

            default:
                return false;
        }
    }

    private bool SetCursorValidityReapingTool(Vector3 cursorPosition, Vector3 playerPosition, ItemData equippedItemDetails)
    {
        List<Item> itemList = new List<Item>();

        if (HelperMethods.GetComponentsAtCursorLocation<Item>(out itemList, cursorPosition))
        {
            if (itemList.Count != 0)
            {
                foreach (Item item in itemList)
                {
                    if(Managers.Data.GetItemData(item.ItemId).itemType == ItemType.ITEM_REAPABLE_SCENARY)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public void DisableCursor()
    {
        _cursorImage.color = new Color(1f, 1f, 1f, 0f);
        CursorIsEnabled = false;
    }

    public void EnableCursor()
    {
        _cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnabled = true;
    }

    public Vector3 GetWorldPositionForCursor()
    {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);

        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(screenPosition);

        return worldPosition;
    }

    public Vector2 GetRectTransformPositionForCursor()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        return RectTransformUtility.PixelAdjustPoint(screenPosition, _cursorRectTransform, _canvas);
    }
}
