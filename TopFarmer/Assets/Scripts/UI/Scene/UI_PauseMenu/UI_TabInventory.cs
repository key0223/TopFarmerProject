using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_TabInventory : MonoBehaviour
{
    [SerializeField] UI_PauseMenuInventorySlot[] _pausemenuInventorySlot = null;


    [SerializeField] Sprite _blankSprite = null;

    private GameObject _textBoxUI;
    public GameObject TextBoxUI {  get { return _textBoxUI; } set { _textBoxUI = value; } }


    private void OnEnable()
    {
        Managers.Event.UpdateInventoryEvent += PopulatePlayerInventory;

        if(InventoryManager.Instance !=null)
        {
            PopulatePlayerInventory(InventoryType.INVEN_PLAYER, InventoryManager.Instance._inventoryLists[(int)InventoryType.INVEN_PLAYER]);
        }

    }
    private void InitialiseInventoryManagementSlots()
    {
        // Clear inventory slots
        for (int i = 0; i < Define.PlayerMaxInvenCampacity; i++)
        {
            _pausemenuInventorySlot[i]._greyedOutImageGO.SetActive(false);
            _pausemenuInventorySlot[i]._itemData= null;
            _pausemenuInventorySlot[i]._itemQuantity = 0;
            _pausemenuInventorySlot[i]._inventoryManagementSlotImage.sprite = _blankSprite;
            _pausemenuInventorySlot[i]._text.text = "";
        }

        // Grey out unavailable slots
        for (int i = InventoryManager.Instance._inventoryCapacity[(int)InventoryType.INVEN_PLAYER]; i < Define.PlayerMaxInvenCampacity; i++)
        {
            _pausemenuInventorySlot[i]._greyedOutImageGO.SetActive(true);
        }
    }
    private void PopulatePlayerInventory(InventoryType inventoryType, List<InventoryItem> inventory)
    {
        if (inventoryType == InventoryType.INVEN_PLAYER)
        {
            InitialiseInventoryManagementSlots();

            // loop through all player inventory items
            for (int i = 0; i < InventoryManager.Instance._inventoryLists[(int)InventoryType.INVEN_PLAYER].Count; i++)
            {
                // Get inventory item details
                _pausemenuInventorySlot[i]._itemData = Managers.Data.GetItemData(inventory[i]._itemId);
                _pausemenuInventorySlot[i]._itemQuantity = inventory[i]._itemQuantity;

                if (_pausemenuInventorySlot[i]._itemData != null)
                {
                    // update inventory management slot with image and quantity
                    _pausemenuInventorySlot[i]._inventoryManagementSlotImage.sprite = Managers.Data.SpriteDict[_pausemenuInventorySlot[i]._itemData.itemSpritePath];
                    _pausemenuInventorySlot[i]._text.text = _pausemenuInventorySlot[i]._itemQuantity.ToString();
                }
            }
        }
    }
    public void DestroyCurrentlyDraggedItems()
    {
        // loop through all player inventory items
        for (int i = 0; i <InventoryManager.Instance._inventoryLists[(int)InventoryType.INVEN_PLAYER].Count; i++)
        {
            if (_pausemenuInventorySlot[i]._draggedItem != null)
            {
                Managers.Resource.Destroy(_pausemenuInventorySlot[i]._draggedItem);
            }

        }
    }
    public void DestroyInventoryTextBoxGameobject()
    {
        // Destroy inventory text box if created
        if (TextBoxUI != null)
        {
            Managers.Resource.Destroy(TextBoxUI);
        }
    }

}
