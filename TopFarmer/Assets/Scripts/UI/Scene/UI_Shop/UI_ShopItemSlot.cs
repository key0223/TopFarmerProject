using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ShopItemSlot : UI_Base,IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    enum Images
    {
        ItemImage,
    }
    enum Texts
    {
        ItemNameText,
        PriceText,
    }

    [SerializeField] Canvas _parentCanvas;
    [SerializeField] UI_Shop _shopUI;

    ItemData _itemData;
    // Awake
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        
    }
    void Start()
    {
        _parentCanvas = GetComponentInParent<Canvas>();
        _shopUI = GetComponentInParent<UI_Shop>();
    }
    public void SetSlot(int itemId)
    {
        ItemData itemData = null;

        if (Managers.Data.ItemDict.TryGetValue(itemId, out itemData))
        {
            GetImage((int)Images.ItemImage).sprite = Managers.Data.SpriteDict[itemData.itemSpritePath];
            GetText((int)Texts.ItemNameText).text = Managers.Data.ItemDict[itemId].itemName;
            GetText((int)Texts.PriceText).text = Managers.Data.ShopItemDict[itemId].sellPrice.ToString();
            _itemData = itemData;
        }
    }

    #region Input

    public void OnPointerEnter(PointerEventData eventData)
    {
        _shopUI.TextBoxUI = Managers.Resource.Instantiate("UI/Scene/InventoryTextBox", _parentCanvas.transform);
        _shopUI.TextBoxUI.transform.position = transform.position;

        UI_InventoryTextBox textBoxUI = _shopUI.TextBoxUI.GetComponent<UI_InventoryTextBox>();

        string purchasePrice = Managers.Data.ShopItemDict[_itemData.itemId].purchasePrice.ToString();
        string itemTypeString = InventoryManager.Instance.GetItemTypeString(_itemData.itemType);

        textBoxUI.SetTextBoxText(_itemData.itemName, itemTypeString, _itemData.itemDescription, "", "", priceText: purchasePrice);

        _shopUI.TextBoxUI.GetComponent<RectTransform>().pivot = new Vector2(-2f, 1f);
        _shopUI.TextBoxUI.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _shopUI.DestroyTextBoxGameobject();
        _shopUI.DestroyPurchaseBox();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(PlayerController.Instance.PlayerCoin> Managers.Data.ShopItemDict[_itemData.itemId].purchasePrice)
        {
            PlayerController.Instance.PlayerCoin -= Managers.Data.ShopItemDict[_itemData.itemId].purchasePrice;
            InventoryManager.Instance.AddItem(Define.InventoryType.INVEN_PLAYER, _itemData.itemId);
            Managers.Reporter.ReportItemPurchased(_itemData,1);
        }
    }
    #endregion
}
