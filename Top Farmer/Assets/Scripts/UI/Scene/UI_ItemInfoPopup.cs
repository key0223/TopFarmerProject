using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_ItemInfoPopup : UI_Scene
{
    enum Texts
    {
        ItemNameText,
        ItemDescriptionText,
        ItemCountText,
        InteractionBtnText,

    }
    enum Buttons
    {
        CloseBtn,
        InteractionBtn,
    }

    private InventoryState _state = InventoryState.Inventory;

    private Slider _countSlider;

    private int _itemDbId;
    private int _templatedId;
    private int _count;
    private int _invenSlotId;
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.CloseBtn).gameObject.BindEvent(OnCloseBtn);
        GetButton((int)Buttons.InteractionBtn).gameObject.BindEvent(OnIntercationBtn);
        _countSlider = GetComponentInChildren<Slider>();
        _countSlider.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
    }
    /// <summary>
    /// �κ��丮���� �������� �������� �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="itemDbId"></param>
    /// <param name="state"></param>
    public void SetItemInfo(int itemDbId, InventoryState state)
    {
        _state = state;

        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Merchant merchantUI = gameSceneUI.MerchantUI;
        UI_Oven ovenUI = gameSceneUI.OvenUI;

        switch (_state)
        {
            case InventoryState.Inventory:
                GetText((int)Texts.InteractionBtnText).text = "�Ǹ�";
                {
                    Item item = Managers.Inven.Get(itemDbId);

                    _itemDbId = itemDbId;
                    _templatedId = item.TemplatedId;
                    _count = item.Count;
                    _invenSlotId = item.Slot;
                }
                break;
            case InventoryState.Merchant:
                GetText((int)Texts.InteractionBtnText).text = "���";
                {
                    InteractItem item = merchantUI.Get(itemDbId);
                    int maxCount = merchantUI.Get(itemDbId).count;

                    //interactItem item = Managers.Npc.MerchantCtrl.Get(itemDbId);
                    //int maxCount = item.count;

                    _countSlider.maxValue = maxCount;

                    _itemDbId = itemDbId;
                    _templatedId = item.templatedId;
                    _count = item.count;
                    _invenSlotId = item.slot;
                }
                break;
                case InventoryState.Oven:
                GetText((int)Texts.InteractionBtnText).text = "����";
                {
                    Item item = Managers.Inven.Get(itemDbId);

                    _itemDbId = itemDbId;
                    _templatedId = item.TemplatedId;
                    _count = item.Count;
                    _invenSlotId = item.Slot;
                }
                break;

        }

        string itemName = Managers.Data.StringDict[$"itemName({_templatedId})"].ko;
        string itemDescription = Managers.Data.StringDict[$"itemDesc({_templatedId})"].ko;
        GetText((int)Texts.ItemNameText).text = itemName;
        GetText((int)Texts.ItemDescriptionText).text = itemDescription;

        _countSlider.minValue = 0;
        _countSlider.maxValue = _count;

    }

    public void OnSliderValueChange()
    {
        GetText((int)Texts.ItemCountText).text = _countSlider.value.ToString();
    }

    private void OnCloseBtn(PointerEventData evt)
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// �˾�â���� ��ư�� ������ �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="evt"></param>
    private void OnIntercationBtn(PointerEventData evt)
    {
        if (_countSlider.value <= 0)
            return;
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Merchant merchantUI = gameSceneUI.MerchantUI;
        UI_Inventory invenUI = gameSceneUI.InvenUI;
        UI_Oven ovenUI = gameSceneUI.OvenUI;

        switch (_state)
        {
            case InventoryState.Inventory:
                {
                    int? slotId;

                    InteractItem findItem = merchantUI.Find(i => i.templatedId == _templatedId && i.stackable);

                    //interactItem findItem = Managers.Npc.MerchantCtrl.Find(i => i.templatedId == _templatedId && i.stackable);
                    if (findItem == null)
                    {
                        slotId = merchantUI.GetEmptySlot();
                        //slotId = Managers.Npc.MerchantCtrl.GetEmptySlot();
                    }
                    else
                        slotId = findItem.slot;

                    Item item = Managers.Inven.Get(_itemDbId);

                    InteractItem interactItem = new InteractItem()
                    {
                        itemDbId = item.ItemDbId,
                        templatedId = item.TemplatedId,
                        count = (int)_countSlider.value,
                        slot = (int)slotId,
                        stackable = item.Stackable,
                    };

                    //Managers.Npc.MerchantCtrl.Add((int)slotId, interactItem);
                    merchantUI.Add((int)slotId, interactItem);
                    invenUI.Slots[_invenSlotId].SetInteractItem(interactItem);

                }
                break;
            case InventoryState.Merchant:
                {
                    InteractItem findItem = merchantUI.Get(_itemDbId);

                    //interactItem findItem = Managers.Npc.MerchantCtrl.Get(_itemDbId);
                    if (_countSlider.value == findItem.count)
                    {
                        invenUI.Slots[_invenSlotId].OnItemSellCancel(findItem);
                    }
                    else
                    {
                        InteractItem interactItem = new InteractItem()
                        {
                            itemDbId = findItem.itemDbId,
                            templatedId = findItem.templatedId,
                            count = (int)_countSlider.value,
                            slot = _invenSlotId,
                            stackable = findItem.stackable,
                        };
                        invenUI.Slots[_invenSlotId].OnItemSellCancel(interactItem);
                    }
                }
                break;
            case InventoryState.Oven:
                {
                    Debug.Log("UI_Popup Oven");
                    int? stoveId = ovenUI.GetEmptyStove();
                    if(stoveId != null)
                    {
                        Item item = Managers.Inven.Get(_itemDbId);

                        InteractItem interactItem = new InteractItem()
                        {
                            itemDbId = item.ItemDbId,
                            templatedId = item.TemplatedId,
                            count = (int)_countSlider.value,
                            stackable = item.Stackable,
                        };

                        ovenUI.Stoves[(int)stoveId].SetInteractItem((int)stoveId,interactItem);
                        invenUI.Slots[_invenSlotId].SetInteractItem(interactItem);
                    }
                }
                break;
        }

        gameObject.SetActive(false);
        _countSlider.value = 0;
    }
}
