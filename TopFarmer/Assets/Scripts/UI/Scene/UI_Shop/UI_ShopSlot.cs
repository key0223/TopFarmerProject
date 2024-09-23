using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ShopSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    UI_Shop _shopUI;

    [SerializeField] Image _itemImage;
    [SerializeField] int _slotNumber = 0;

    Canvas _parentCanvas;

    public Text _text;
    public GameObject _greyedOutImageGO;
    
    ItemData _itemData;
    int _itemQuantity;

    public Image ItemImage { get { return _itemImage; } set { _itemImage = value; } }
    public ItemData ItemData 
    {
        get { return _itemData; }
        set 
        { 
            _itemData = value;
            UpdateSlot();
        } 
    }

    public int ItemQuantity { get { return _itemQuantity; } set { _itemQuantity = value; } }

    GameObject _draggedItem;

    bool _isInteractionEnable = true;

    private void Awake()
    {
        _parentCanvas = GetComponentInParent<Canvas>();
        _shopUI = GetComponentInParent<UI_Shop>();
    }

    void UpdateSlot()
    {
        if( _itemData != null )
        {
            if(_itemData.isStartingItem)
            {
                ItemImage.color = new Color(1f, 1f, 1f, 0.5f); // 반투명하게
                _isInteractionEnable = false;
            }
            else
            {
                ItemImage.color = new Color(1f, 1f, 1f, 1f); // 원래 색상
                _isInteractionEnable = true;
            }
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_itemQuantity != 0 && _isInteractionEnable)
        {
            _draggedItem = Managers.Resource.Instantiate("Object/Item/InventoryDraggedItem", _shopUI.transform);

            Image draggedItemImage = _draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = _itemImage.sprite;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_draggedItem != null && _isInteractionEnable)
        {
            _draggedItem.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggedItem != null && _isInteractionEnable)
        {
            Managers.Resource.Destroy(_draggedItem);

            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<UI_PauseMenuInventorySlot>() != null)
            {
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UI_ShopSlot>()._slotNumber;

                InventoryManager.Instance.SwapInventoryItems(Define.InventoryType.INVEN_PLAYER, _slotNumber, toSlotNumber);

                _shopUI.DestroyTextBoxGameobject();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_itemQuantity != 0 && _isInteractionEnable)
        {
            _shopUI.TextBoxUI = Managers.Resource.Instantiate("UI/Scene/InventoryTextBox", _parentCanvas.transform);
            _shopUI.TextBoxUI.transform.position = transform.position;

            UI_InventoryTextBox textBoxUI = _shopUI.TextBoxUI.GetComponent<UI_InventoryTextBox>();

            string itemTypeString = InventoryManager.Instance.GetItemTypeString(_itemData.itemType);

            string sellPrice= Managers.Data.ShopItemDict[_itemData.itemId].sellPrice.ToString();

            textBoxUI.SetTextBoxText(_itemData.itemName, itemTypeString, _itemData.itemDescription, "", "", priceText:sellPrice);

            // Set text box position
            if (_slotNumber > 23)
            {
                _shopUI.TextBoxUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                _shopUI.TextBoxUI.transform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
            }
            else
            {
                _shopUI.TextBoxUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                _shopUI.TextBoxUI.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _shopUI.DestroyTextBoxGameobject();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_itemData == null) return;

        if (_itemQuantity != 0 &&!_isInteractionEnable)
            return;


            PlayerController.Instance.PlayerCoin += Managers.Data.ShopItemDict[_itemData.itemId].sellPrice;
            InventoryManager.Instance.RemoveItem(Define.InventoryType.INVEN_PLAYER, _itemData.itemId);

    }






}
