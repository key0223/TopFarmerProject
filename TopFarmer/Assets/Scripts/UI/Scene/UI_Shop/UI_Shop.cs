using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Shop : MonoBehaviour
{
    [SerializeField] UI_ShopSlot[] _slots = null;
    [SerializeField] Sprite _blankSprite = null;

    [SerializeField] GameObject _shopItemParent= null;
    [SerializeField] Scrollbar _scrollbar = null;

    GameObject _textBoxUI;
    public GameObject TextBoxUI { get { return _textBoxUI; } set { _textBoxUI = value; } }
    GameObject _purchaseBoxUI;
    public GameObject PurchaseBoxUI { get { return _purchaseBoxUI; } set { _purchaseBoxUI = value; } }  


    private void Awake()
    {
        ClearShopItems();
    }
    private void OnEnable()
    {

        Managers.Event.UpdateInventoryEvent += PopulatePlayerInventory;

        if(InventoryManager.Instance !=null )
        {
            PopulatePlayerInventory(InventoryType.INVEN_PLAYER, InventoryManager.Instance._inventoryLists[(int)InventoryType.INVEN_PLAYER]);
        }

        ClearShopItems();
        InitializeShopItems();
    }

    private void OnDisable()
    {
        Managers.Event.UpdateInventoryEvent -= PopulatePlayerInventory;
    }
    void InitializeShopItems()
    {
       foreach(ShopItemData shopItemData in Managers.Data.ShopItemDict.Values)
        {
            if(shopItemData.purchasable)
            {
                GameObject item = Managers.Resource.Instantiate("UI/Scene/ShopProductItem",_shopItemParent.transform);
                UI_ShopItemSlot shopItemSlot = item.GetComponent<UI_ShopItemSlot>();

                shopItemSlot.SetSlot(shopItemData.itemId);

            }
        }
        _scrollbar.value = 1;
    }
    private void InitialiseInventorySlots()
    {
        // Clear inventory slots
        for (int i = 0; i < Define.PlayerMaxInvenCampacity; i++)
        {
            _slots[i]._greyedOutImageGO.SetActive(false);
            _slots[i].ItemData = null;
            _slots[i].ItemQuantity = 0;
            _slots[i].ItemImage.sprite = _blankSprite;
            _slots[i]._text.text = "";
        }

        // Grey out unavailable slots
        for (int i = InventoryManager.Instance._inventoryCapacity[(int)InventoryType.INVEN_PLAYER]; i < Define.PlayerMaxInvenCampacity; i++)
        {

            _slots[i]._greyedOutImageGO.SetActive(true);
        }
    }
    private void PopulatePlayerInventory(InventoryType inventoryType, List<InventoryItem> playerInventoryList)
    {
        if (inventoryType == InventoryType.INVEN_PLAYER)
        {
            InitialiseInventorySlots();

            // loop through all player inventory items
            for (int i = 0; i < InventoryManager.Instance._inventoryLists[(int)InventoryType.INVEN_PLAYER].Count; i++)
            {
                // Get inventory item details
                _slots[i].ItemData = Managers.Data.GetItemData(playerInventoryList[i]._itemId);
                _slots[i].ItemQuantity = playerInventoryList[i]._itemQuantity;

                if (_slots[i].ItemData != null)
                {
                    // update inventory management slot with image and quantity
                    _slots[i].ItemImage.sprite = Managers.Data.SpriteDict[_slots[i].ItemData.itemSpritePath];
                    _slots[i]._text.text = _slots[i].ItemQuantity.ToString();
                }
            }
        }
    }

    public void DestroyTextBoxGameobject()
    {
        // Destroy inventory text box if created
        if (TextBoxUI != null)
        {
            Managers.Resource.Destroy(TextBoxUI);
        }
    }

    public void DestroyPurchaseBox()
    {
        if(PurchaseBoxUI != null)
        {
            Managers.Resource.Destroy(PurchaseBoxUI);
        }
    }
    void ClearShopItems()
    {
        foreach(Transform child in _shopItemParent.transform)
        {
            Managers.Resource.Destroy(child.gameObject);
        }
    }


}
