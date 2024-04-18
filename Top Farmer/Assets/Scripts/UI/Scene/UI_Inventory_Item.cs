using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;
using static UnityEditor.Progress;

public class UI_Inventory_Item : UI_Base
{
    enum Texts
    {
        ItemCountText,
    }
    enum GameObjects
    {
        SelectedFrame,
    }

    private InventoryState _state = InventoryState.Inventory;

    #region Properties
    public int SlotId { get; set; }

    public int ItemDbId { get; set; }
    public int TemplatedId { get; set; }
    public int Count { get; set; }
    public bool Equipped { get; set; }

    private Image _icon = null;
    public Image Icon { get { return _icon; } }

    private Transform _topParent;
    private Transform _prevParent;
    public Transform PrevParent { get { return _prevParent; } }
    private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup { get { return _canvasGroup; } }
    #endregion

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        GetObject((int)GameObjects.SelectedFrame).SetActive(false);

        _topParent = FindObjectOfType<Canvas>().transform;
        _icon = GetComponent<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _icon.gameObject.BindEvent(OnPointerClick, Define.UIEvent.PointerClick);
        _icon.gameObject.BindEvent(OnBeginDrag, Define.UIEvent.BeginDrag);
        _icon.gameObject.BindEvent(OnDrag, Define.UIEvent.Drag);
        _icon.gameObject.BindEvent(OnEndDrag, Define.UIEvent.EndDrag);
        _icon.gameObject.BindEvent(OnDrop, Define.UIEvent.Drop);
    }

    public void SetItem(Item item)
    {
        if (item == null)
            return;

        ItemDbId = item.ItemDbId;
        TemplatedId = item.TemplatedId;
        Count = item.Count;
        Equipped = item.Equipped;

        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(TemplatedId, out itemData);
        Sprite icon = Managers.Resource.Load<Sprite>($"{itemData.iconPath}");
        _icon.sprite = icon;
        SetFrame(Equipped);
        SetCountText(Count);
        //UI_Inventory_Slot slot = transform.parent.GetComponent<UI_Inventory_Slot>();

    }
    public void OnItemSellCancel(InteractItem interactItem)
    {
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_ToolBar toolbarUI = gameSceneUI.ToolBarUI;
        UI_Inventory invenUI = gameSceneUI.InvenUI;
        UI_Merchant merchantUI = gameSceneUI.MerchantUI;

        if (Managers.Inven.Items.ContainsKey(interactItem.itemDbId))
        {
            Item item = Managers.Inven.Get(interactItem.itemDbId);
            item.Count += interactItem.count;
            Count = item.Count;

            SetCountText(item.Count);

        }
        else
        {
            int? findSlot = Managers.Inven.GetEmptySlot();

            ItemInfo itemInfo = new ItemInfo()
            {
                itemDbId = interactItem.itemDbId,
                templatedId = interactItem.templatedId,
                count = interactItem.count,
                slot = (int)findSlot,
                equipped = false,
            };

            Item item = Item.MakeItem(itemInfo);
            Managers.Inven.Add(item);
            Count = interactItem.count;
            SetCountText(interactItem.count);

        }

        SetCountText(Count);

        _canvasGroup.alpha = 1f;

        //Managers.Npc.MerchantCtrl.Remove(interactItem);
        merchantUI.Remove(interactItem);

        merchantUI.RefreshUI();

        toolbarUI.RefreshUI();
        invenUI.RefreshUI();
    }
    public void SetInteractItem(InteractItem interactItem)
    {
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_ToolBar toolbarUI = gameSceneUI.ToolBarUI;
        UI_Inventory invenUI = gameSceneUI.InvenUI;

        Item item = Managers.Inven.Get(interactItem.itemDbId);

        if (item.Count - interactItem.count <= 0)
        {
            //item.Count = 0;

            ItemDbId = 0;
            TemplatedId = 0;
            Count = 0;
            Equipped = false;

            _canvasGroup.alpha = 0;
            SetCountText(0);

            Managers.Inven.Remove(item);
        }
        else
        {
            Count -= interactItem.count;
            item.Count -= interactItem.count;

            SetCountText(item.Count);
        }


        toolbarUI.RefreshUI();
        invenUI.RefreshUI();
    }
    public void SetItemData(int itemDbId)
    {
        Item item = Managers.Inven.Get(itemDbId);

        ItemDbId = item.ItemDbId;
        TemplatedId = item.TemplatedId;
        Count = item.Count;
        Equipped = item.Equipped;

        item.Slot = SlotId;

        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(TemplatedId, out itemData);
        Sprite icon = Managers.Resource.Load<Sprite>($"{itemData.iconPath}");
        _icon.sprite = icon;
        SetCountText(Count);
        SetFrame(item.Equipped);
        //UI_Inventory_Slot slot = transform.parent.GetComponent<UI_Inventory_Slot>();
        //slot.SlotClear(item.Equipped);

    }

    #region Mouse Interaction
    private void OnPointerClick(PointerEventData evt)
    {
        //if (Count <= 0|| CanvasGroup.alpha == 0)
        //    return;

        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_ToolBar toolbarUI = gameSceneUI.ToolBarUI;
        UI_Inventory invenUI = gameSceneUI.InvenUI;

        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(TemplatedId, out itemData);

        if (itemData == null)
            return;

        switch (invenUI.State)
        {
            case InventoryState.Inventory:
                {
                    // Temp
                    Item item = Managers.Inven.Get(ItemDbId);
                    if (item == null) return;
                    for (int i = 0; i < 9; i++)
                    {
                        Item toolbarItem = Managers.Inven.Find(e => e.Equipped);

                        if (toolbarItem == null) continue;

                        toolbarItem.Equipped = !toolbarItem.Equipped;
                    }
                    item.Equipped = !item.Equipped;
                    if (item.Equipped == true)
                        Managers.Object.Player.RefreshHoldingItem(item);
                    else
                        Managers.Object.Player.RefreshHoldingItem(null);

                    SetFrame(item.Equipped);
                }
                break;
            case InventoryState.Merchant:
                {
                    UI_ItemInfoPopup popupUI = gameSceneUI.InfoPopupUI;
                    popupUI.gameObject.SetActive(true);
                    popupUI.SetItemInfo(ItemDbId, _state);
                }
                break;
            case InventoryState.Oven:
                {
                    _state = InventoryState.Oven;
                    UI_ItemInfoPopup popupUI = gameSceneUI.InfoPopupUI;
                    popupUI.gameObject.SetActive(true);
                    popupUI.SetItemInfo(ItemDbId, _state);
                }
                break;
        }

        /*
        if (invenUI.State == InventoryState.Inventory)
        {
            // Temp
            Item item = Managers.Inven.Get(ItemDbId);
            if (item == null) return;
            for (int i = 0; i < 9; i++)
            {
                Item toolbarItem = Managers.Inven.Find(e => e.Equipped);

                if (toolbarItem == null) continue;

                toolbarItem.Equipped = !toolbarItem.Equipped;
            }
            item.Equipped = !item.Equipped;
            if (item.Equipped == true)
                Managers.Object.Player.RefreshHoldingItem(item);
            else
                Managers.Object.Player.RefreshHoldingItem(null);

            SetFrame(item.Equipped);

        }
        else if (invenUI.State == InventoryState.Merchant)
        {
            UI_ItemInfoPopup popupUI = gameSceneUI.InfoPopupUI;
            popupUI.gameObject.SetActive(true);
            popupUI.SetItemInfo(ItemDbId, _state);
        }
        */
        toolbarUI.RefreshUI();
        invenUI.RefreshUI();

    }
    private void OnBeginDrag(PointerEventData evt)
    {
        if (Count <= 0)
            return;

        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = gameSceneUI.InvenUI;

        if (invenUI.State == InventoryState.Inventory)
        {
            _prevParent = gameObject.transform.parent;
            transform.SetParent(_topParent);
            transform.SetAsLastSibling();

            _canvasGroup.alpha = 0.6f;
            _canvasGroup.blocksRaycasts = false;
        }

    }
    private void OnDrag(PointerEventData evt)
    {
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = gameSceneUI.InvenUI;

        if (invenUI.State == InventoryState.Inventory)
        {
            gameObject.transform.position = evt.position;

        }
    }
    private void OnEndDrag(PointerEventData evt)
    {
        _canvasGroup.alpha = 0f;
        if (transform.parent == _topParent)
        {
            transform.SetParent(_prevParent);
            gameObject.transform.position = _prevParent.transform.position;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1.0f;
            Debug.Log("EndDrag");
        }

        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_ToolBar toolbarUI = gameSceneUI.ToolBarUI;
        UI_Inventory invenUI = gameSceneUI.InvenUI;
        toolbarUI.RefreshUI();
        invenUI.RefreshUI();
    }

    private void OnDrop(PointerEventData evt)
    {
        if (evt.pointerDrag != null)
        {
            // 드래그 중인 아이템
            UI_Inventory_Item dragItem = evt.pointerDrag.GetComponent<UI_Inventory_Item>();
            _icon.sprite = dragItem.Icon.sprite;
            _canvasGroup.alpha = 1f;


            // 드래그한 슬롯에 데이터 세팅 
            SetItemData(dragItem.ItemDbId);

            // 이전 슬롯 초기화
            dragItem.Clear();

        }

        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_ToolBar toolbarUI = gameSceneUI.ToolBarUI;
        UI_Inventory invenUI = gameSceneUI.InvenUI;
        toolbarUI.RefreshUI();
        invenUI.RefreshUI();
    }
    #endregion

    public void SetFrame(bool equipped = false)
    {
        GetObject((int)GameObjects.SelectedFrame).SetActive(equipped);
    }
    public void SetCountText(int count)
    {
        GetText((int)Texts.ItemCountText).text = count.ToString();
        if (count <= 0)
            GetText((int)Texts.ItemCountText).text = " ";

    }
    public void Clear()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = true;
        transform.SetParent(PrevParent);
        transform.position = PrevParent.position;

        SetFrame();

    }
}
