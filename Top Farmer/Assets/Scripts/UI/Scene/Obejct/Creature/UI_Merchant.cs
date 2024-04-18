using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;
using static UnityEditor.Progress;

public class UI_Merchant : UI_Base
{
    enum Texts
    {
        MessageText
    }
    enum GameObjects
    {
        SellingItemList,
        NpcImage,
        CloseBtnText,
    }
    public List<UI_Merchant_Item> Slots { get; } = new List<UI_Merchant_Item>();
    public Dictionary<int, InteractItem> Items { get; } = new Dictionary<int, InteractItem>();
    private Animator _animator;
    private bool _init = false;

    public override void Init()
    {
        Slots.Clear();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));

        GetObject((int)GameObjects.CloseBtnText).BindEvent(OnCloseBtnClick,UIEvent.PointerClick);
        _animator = GetObject((int)GameObjects.NpcImage).GetComponent<Animator>();

        GameObject sellingItemList = GetObject((int)GameObjects.SellingItemList);
        foreach (Transform child in sellingItemList.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < 15; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_Inventory_Slot", sellingItemList.transform);
            GameObject itemGameObject = go.transform.Find("UI_Inventory_Item").gameObject;
            UI_Merchant_Item item = Util.GetOrAddComponent<UI_Merchant_Item>(itemGameObject);
            item.SlotId += i;
            Slots.Add(item);
        }

        _animator.Play("Portrait_Idle");


        _init = true;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        SetAlphaZero();
    }
    private void SetAlphaZero()
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            Slots[i].CanvasGroup.alpha = 0;
            //UI_Inventory_Slot slot = Slots[i].transform.parent.GetComponent<UI_Inventory_Slot>();
            //slot.SlotClear(false);
        }

    }

    public void Add(int slotId, InteractItem item)
    {
        InteractItem findItem = Find(i => i.templatedId == item.templatedId && i.stackable);
        UI_Merchant_Item merchantItem = Slots[slotId];

        if (findItem != null)
        {
            Items[findItem.itemDbId].count += item.count;
            merchantItem.SetUI(findItem);

        }
        else
        {
            Items.Add(item.itemDbId, item);
            merchantItem.SetUI(item);
        }
    }
    public void Remove(InteractItem item)
    {
        foreach (UI_Merchant_Item merchantItem in Slots)
        {
            if (merchantItem.ItemDbId == item.itemDbId)
            {
                merchantItem.ClearUI();
            }
        }

        Items.Remove(item.itemDbId);
    }
    public InteractItem Get(int itemDbId)
    {
        InteractItem item;
        Items.TryGetValue(itemDbId, out item);

        return item;
    }

    public InteractItem Find(Func<InteractItem, bool> condition)
    {
        foreach (InteractItem item in Items.Values)
        {
            if (condition.Invoke(item))
                return item;
        }
        return null;
    }
    public int? GetEmptySlot()
    {
        for (int slot = 0; slot < Slots.Count; slot++)
        {
            if (Slots[slot].TemplatedId == 0)
                return slot;
        }

        return null;
    }

    public void SetMessageText(string text)
    {
        GetText((int)Texts.MessageText).text = text;
    }

    #region Mouse Interaction

    public void OnCloseBtnClick(PointerEventData evt)
    {
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Merchant merchantUI = gameSceneUI.MerchantUI;
        UI_Inventory invenUI = gameSceneUI.InvenUI;

        merchantUI.gameObject.SetActive(false);
        invenUI.gameObject.SetActive(false);

        invenUI.State = Define.InventoryState.Inventory;

        Managers.Inven.UpdateRedisItems();
    }

    #endregion
}
