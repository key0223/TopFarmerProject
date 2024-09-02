using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_InventorySlot : UI_Base, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    enum Texts
    {
        ItemQuantityText,
    }
    enum Images
    {
        ItemSpriteImage,
        HightlightImage,
    }
    [SerializeField] int _slotNumber = 0;
    [SerializeField] GameObject _itemPrefab;
    [SerializeField] Sprite _blankSprite = null;

    Camera _mainCamera;
    Canvas _parentCanvas;
    Transform _parentItem;
    public  GameObject _draggedItem;
    
    Cursor _cursor;
    GridCursor _gridCursor;

    UI_InventoryBar _inventoryBar = null;

    ItemData _itemData;
    int _itemQuantity = 0;
    bool _isSelected = false;

    public ItemData ItemData { get { return _itemData; } set { _itemData = value; } }
    public int ItemQuantity { get { return _itemQuantity; } set { _itemQuantity = value; } }
    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;
            SetHightlightImage();
        }
    }

    // Awake
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        _parentCanvas = GetComponentInParent<Canvas>();
        _inventoryBar = GetComponentInParent<UI_InventoryBar>();
    }
    void OnEnable()
    {
        Managers.Event.AfterSceneLoadEvent += SceneLoaded;
        Managers.Event.RemoveSelectedItemFromInventoryEvent += RemoveSelectedItemFromInventory;

        Managers.Event.DropSelectedItemEvent += DropSelctedItemAtMousePosition;

    }
    void OnDisable()
    {
        Managers.Event.AfterSceneLoadEvent -= SceneLoaded;
        Managers.Event.RemoveSelectedItemFromInventoryEvent -= RemoveSelectedItemFromInventory;
        Managers.Event.DropSelectedItemEvent -= DropSelctedItemAtMousePosition;
    }
    void Start()
    {
        _mainCamera = Camera.main;
        _gridCursor = FindObjectOfType<GridCursor>();
        _cursor = FindObjectOfType<Cursor>();
    }
    void ClearCursors()
    {
        _gridCursor.DisableCursor();
        _cursor.DisableCursor();

        _gridCursor.SelectedItemType = ItemType.NONE;
        _cursor.SelectedItemType = ItemType.NONE;
    }


    public void SetSlot(int itemId)
    {
        ItemData itemData = null;
        if (Managers.Data.ItemDict.TryGetValue(itemId, out itemData))
        {
            GetImage((int)Images.ItemSpriteImage).sprite = Managers.Data.SpriteDict[itemData.itemSpritePath];
            GetText((int)Texts.ItemQuantityText).text = ItemQuantity.ToString();
            this.ItemData = itemData;
        }
    }
    public void SlotClear()
    {
        GetImage((int)Images.ItemSpriteImage).sprite = _blankSprite;
        GetText((int)Texts.ItemQuantityText).text = "";
        _itemData = null;
        _itemQuantity = 0;
    }
    private void SetSelectedItem()
    {
        // Clear currently highlighted items
        _inventoryBar.ClearHighlightedOnInventorySlots();

        // Highlight item on inventory bar
        _isSelected = true;

        // Set highlighted inventory slots
        _inventoryBar.SetHighlightedInventorySlots();

        #region Cursor

        _gridCursor.ItemUseGridRadius = (int)ItemData.itemUseGridRadius;
        _cursor.ItemUseRadius = ItemData.itemUseRadius;

        if(ItemData.itemUseGridRadius > 0)
        {
            _gridCursor.EnableCursor();
        }
        else
        {
            _gridCursor.DisableCursor();
        }

        if (ItemData.itemUseRadius > 0f)
        {
            _cursor.EnableCursor();
        }
        else
        {
            _cursor.DisableCursor();
        }

        _gridCursor.SelectedItemType = ItemData.itemType;
        _cursor.SelectedItemType = ItemData.itemType;

        #endregion

        // Set item selected in inventory
        InventoryManager.Instance.SetSelectedInventoryItem(InventoryType.INVEN_PLAYER, _itemData.itemId);


        if (_itemData.canBeCarried == true)
        {
            // Show player carrying item
            PlayerController.Instance.ShowCarriedItem(_itemData.itemId);
        }
        else
        {
            // Show player carrying nothing
            PlayerController.Instance.ClearCarriedItem();
        }
    }

    public void ClearSelectedItem()
    {
        ClearCursors();

        // Clear currently highlighted items
        _inventoryBar.ClearHighlightedOnInventorySlots();
        _isSelected = false;

        // Set no item selected in inventory
        InventoryManager.Instance.ClearSelectedInvetoryItem(InventoryType.INVEN_PLAYER);

        // Clear player carrying item
        //Player.Instance.ClearCarriedItem();
    }
    public void SetHightlightImage()
    {
        GetImage((int)Images.HightlightImage).color = IsSelected ? new Color(1f,1f,1f,1f) : new Color(0f,0f,0f,0f);
    }

    private void DropSelctedItemAtMousePosition()
    {
        if (_itemData != null && _isSelected)
        {
            if (_gridCursor.CursorPositionIsValid)
            {
                Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_mainCamera.transform.position.z));

                // Create item from prefab at mouse position
                GameObject itemGameObject = Instantiate(_itemPrefab, new Vector3(worldPosition.x, worldPosition.y - GridCellSize / 2f, worldPosition.z), Quaternion.identity, _parentItem);
                Item item = itemGameObject.GetComponent<Item>();
                item.ItemId = _itemData.itemId;

                // Remove item from players inventory
                InventoryManager.Instance.RemoveItem(InventoryType.INVEN_PLAYER, item.ItemId);

                // if no more of item then clear selected
                if (InventoryManager.Instance.FindItemInInventory(InventoryType.INVEN_PLAYER, item.ItemId) == -1)
                {
                    ClearSelectedItem();

                }
            }
        }
    }

    private void RemoveSelectedItemFromInventory()
    {
        if(_itemData != null && _isSelected)
        {
            int itemId = ItemData.itemId;

            InventoryManager.Instance.RemoveItem(InventoryType.INVEN_PLAYER,itemId);

            if(InventoryManager.Instance.FindItemInInventory(InventoryType.INVEN_PLAYER, itemId) == -1)
            {
                ClearSelectedItem();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
       if(_itemData != null)
        {
            PlayerController.Instance.DisablePlayerInputAndResetMovement();
            _draggedItem = Managers.Resource.Instantiate("Object/Item/InventoryDraggedItem",_inventoryBar.transform);

            Image draggedItemImage = _draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = GetImage((int)Images.ItemSpriteImage).sprite;

            SetSelectedItem();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_draggedItem != null)
        {
            _draggedItem.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggedItem != null)
        {
            Managers.Resource.Destroy(_draggedItem);

            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<UI_InventorySlot>() != null)
            {
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UI_InventorySlot>()._slotNumber;

                InventoryManager.Instance.SwapInventoryItems(InventoryType.INVEN_PLAYER, _slotNumber, toSlotNumber);

                DestroyTextBox();
                ClearSelectedItem();

            }
            else
            {
                if (_itemData.canBeDropped)
                {
                    int stackSize = _itemQuantity;
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        for (int i = 0; i < stackSize; i++)
                        {
                            DropSelctedItemAtMousePosition();
                        }
                    }
                    else
                    {
                        DropSelctedItemAtMousePosition();

                    }
                }
            }
            PlayerController.Instance.EnablePlayerInput();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       if(_itemQuantity != 0)
        {
            GameObject textBoxGo = Managers.Resource.Instantiate("UI/Scene/InventoryTextBox");
            textBoxGo.transform.SetParent(_parentCanvas.transform, false);
            _inventoryBar.InventoryTextBoxGameObject = textBoxGo;

            UI_InventoryTextBox textBoxUI = textBoxGo.GetComponent<UI_InventoryTextBox>();

            string itemTypeString = InventoryManager.Instance.GetItemTypeString(_itemData.itemType);

            textBoxUI.SetTextBoxText(_itemData.itemName, itemTypeString,_itemData.itemDescription,"","",priceText:"");

            // Set text box position according to inventory bar position
            if (_inventoryBar.IsInventoryBarPositionBottom)
            {
                textBoxGo.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                textBoxGo.transform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
            }
            else
            {
                textBoxGo.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                textBoxGo.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }

        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // if left click
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // if inventory slot currently selected then deselect
            if (_isSelected == true)
            {
                ClearSelectedItem();
            }
            else
            {
                if (_itemQuantity > 0)
                {
                    SetSelectedItem();
                }
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
       DestroyTextBox();
    }

    private void DestroyTextBox()
    {
        if (_inventoryBar.InventoryTextBoxGameObject != null)
        {
            Managers.Resource.Destroy(_inventoryBar.InventoryTextBoxGameObject);
        }
    }
    public void SceneLoaded()
    {
        _parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }
}
