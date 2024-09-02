using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_PauseMenuInventorySlot : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image _inventoryManagementSlotImage;
    public Text _text;
    public GameObject _greyedOutImageGO;

    UI_TabInventory _inventoryTab;

    [HideInInspector] public ItemData _itemData;
    [HideInInspector] public int _itemQuantity;

    [SerializeField] int _slotNumber = 0;

    Canvas _parentCanvas;
    Vector3 _startingPosition;

    public GameObject _draggedItem;

    void Awake()
    {
        _parentCanvas = GetComponentInParent<Canvas>();
        _inventoryTab = GetComponentInParent<UI_TabInventory>();    
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_itemQuantity != 0)
        {
            _draggedItem = Managers.Resource.Instantiate("Object/Item/InventoryDraggedItem", _inventoryTab.transform);

            Image draggedItemImage = _draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = _inventoryManagementSlotImage.sprite;
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

            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<UI_PauseMenuInventorySlot>() != null)
            {
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UI_PauseMenuInventorySlot>()._slotNumber;

                InventoryManager.Instance.SwapInventoryItems(Define.InventoryType.INVEN_PLAYER, _slotNumber, toSlotNumber);

                _inventoryTab.DestroyInventoryTextBoxGameobject();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_itemQuantity != 0)
        {
            _inventoryTab.TextBoxUI = Managers.Resource.Instantiate("UI/Scene/InventoryTextBox", _parentCanvas.transform);
            _inventoryTab.TextBoxUI.transform.position = transform.position;

            UI_InventoryTextBox textBoxUI = _inventoryTab.TextBoxUI.GetComponent<UI_InventoryTextBox>();

            string itemTypeString = InventoryManager.Instance.GetItemTypeString(_itemData.itemType);

            textBoxUI.SetTextBoxText(_itemData.itemName, itemTypeString, _itemData.itemDescription, "", "", priceText:"");

            // Set text box position
            if (_slotNumber > 23)
            {
                _inventoryTab.TextBoxUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                _inventoryTab.TextBoxUI.transform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
            }
            else
            {
                _inventoryTab.TextBoxUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                _inventoryTab.TextBoxUI.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _inventoryTab.DestroyInventoryTextBoxGameobject();
    }
}
