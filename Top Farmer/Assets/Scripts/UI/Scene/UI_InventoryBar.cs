using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_InventoryBar : MonoBehaviour
{
    [SerializeField] UI_InventorySlot[] _inventorySlots = null;
    GameObject _inventoryTextBoxGameObject;

    RectTransform _rect;
    GameObject _inventoryBarDraggedItem;
    bool _isInventoryBarPositionBottom = true;

    public GameObject InventoryTextBoxGameObject { get { return _inventoryTextBoxGameObject; } set { _inventoryTextBoxGameObject = value; } }
    public bool IsInventoryBarPositionBottom { get { return _isInventoryBarPositionBottom; } set { _isInventoryBarPositionBottom = value; } }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Managers.Event.UpdateInventoryEvent += UpdateInventory;
    }
    private void OnDisable()
    {
        Managers.Event.UpdateInventoryEvent -= UpdateInventory;

    }
    private void Update()
    {
        SwitchInventoryBarPosition();
    }

    void UpdateInventory(InventoryType inventoryType, List<InventoryItem> inventory)
    {
        if (inventoryType == InventoryType.INVEN_PLAYER)
        {
            ClearInventorySlots();

            if (_inventorySlots.Length > 0 && inventory.Count > 0)
            {
                // loop through inventory slots and update with corresonding inventory list item
                for (int i = 0; i < _inventorySlots.Length; i++)
                {
                    if (i < inventory.Count)
                    {
                        int itemCode = inventory[i]._itemId;

                        // ItemDetails itemDetails = InventoryManager.Instance.itemDetails.Find(x=> x.itemCode == itemCode);
                        ItemData itemData = Managers.Data.ItemDict[itemCode];

                        // add images and details to inventory item slot
                        _inventorySlots[i].ItemQuantity = inventory[i]._itemQuantity;
                        _inventorySlots[i].SetSlot(itemCode);


                        SetHighlightedInventorySlots(i);
                    }
                }
            }
        }

    }
    private void ClearInventorySlots()
    {
        if (_inventorySlots.Length > 0)
        {
            // loop through inventory slots and update with blank sprite
            for (int i = 0; i < _inventorySlots.Length; i++)
            {
                _inventorySlots[i].SlotClear();

            }
        }
    }
    public void SetHighlightedInventorySlots()
    {
        if (_inventorySlots.Length > 0)
        {

            // loop through inventory slots and clear highlight sprites
            for (int i = 0; i < _inventorySlots.Length; i++)
            {
                SetHighlightedInventorySlots(i);
            }
        }
    }
    public void SetHighlightedInventorySlots(int itemPosition)
    {
        if (_inventorySlots.Length > 0 && _inventorySlots[itemPosition].ItemData != null)
        {
            if (_inventorySlots[itemPosition].IsSelected)
            {
                _inventorySlots[itemPosition].SetHightlightImage();

                // Update inventory to show item as selected
                InventoryManager.Instance.SetSelectedInventoryItem(InventoryType.INVEN_PLAYER, _inventorySlots[itemPosition].ItemData.itemId);
            }
        }
    }
    public void ClearHighlightedOnInventorySlots()
    {
        if (_inventorySlots.Length > 0)
        {
            // loop through inventory slots and clear highlight sprites
            for (int i = 0; i < _inventorySlots.Length; i++)
            {
                if (_inventorySlots[i].IsSelected)
                {
                    _inventorySlots[i].IsSelected = false;
                    _inventorySlots[i].SetHightlightImage();

                    // Update inventory to show item as not selected
                    //InventoryManager.Instance.ClearSelectedInvetoryItem(InventoryLocation.INVEN_LOCATION_PLAYER);
                }
            }
        }
    }
    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewportPosition = PlayerController.Instance.GetPlayerViewportPosition();

        if (playerViewportPosition.y > 0.3f && IsInventoryBarPositionBottom == false)
        {

            // transform.position = new Vector3(transform.position.x, 7.5f,0f); //  this waw changed to control the recttransform see below
            _rect.pivot = new Vector2(0.5f, 0f);
            _rect.anchorMin = new Vector2(0.5f, 0f);
            _rect.anchorMax = new Vector2(0.5f, 0f);
            _rect.anchoredPosition = new Vector2(0f, 2.5f);

            IsInventoryBarPositionBottom = true;
        }
        else if (playerViewportPosition.y <= 0.3f && IsInventoryBarPositionBottom == true)
        {
            // transform.position = new Vector3(transform.position.x,_mainCamera.pixelHeight - 120f,0f); //  this waw changed to control the recttransform see below
            _rect.pivot = new Vector2(0.5f, 1f);
            _rect.anchorMin = new Vector2(0.5f, 1f);
            _rect.anchorMax = new Vector2(0.5f, 1f);
            _rect.anchoredPosition = new Vector2(0f, -2.5f);

            IsInventoryBarPositionBottom = false;
        }
    }

    public void DestoryCurrentlyDraggedItems()
    {
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            if (_inventorySlots[i]._draggedItem != null)
            {
                Managers.Resource.Destroy(_inventorySlots[i]._draggedItem);
            }
        }
    }
    public void ClearCurrentlySelectedItems()
    {
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            _inventorySlots[i].ClearSelectedItem();
        }
    }
}

