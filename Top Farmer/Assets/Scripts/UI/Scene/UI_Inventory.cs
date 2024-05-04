using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Inventory : UI_Base
{
    enum GameObjects
    {
        ToolBar,
        Content,
    }

    enum Images
    {
        ItemIconImage,
    }
   
    enum Texts
    {
        ItemNameText,
        ItemDescriptionText,
        ItemCountText
    }
    bool _init = false;
    private InventoryState _state = InventoryState.Inventory;
    public InventoryState State
    {
        get { return _state; }
        set
        {
            _state = value;
            RefreshUI();
        }
    }

    public List<UI_Inventory_Item> Slots { get; } = new List<UI_Inventory_Item>();
    public override void Init()
    {
        Slots.Clear();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        GameObject toolBar = GetObject((int)GameObjects.ToolBar);
        foreach (Transform child in toolBar.transform)
            Destroy(child.gameObject);

        GameObject content = GetObject((int)GameObjects.Content);
        foreach (Transform child in content.transform)
            Destroy(child.gameObject);

        int slotId = 0;

        for (int i = 0; i < 8; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_Inventory_Slot", toolBar.transform);
            GameObject itemGameObject = go.transform.Find("UI_Inventory_Item").gameObject;
            UI_Inventory_Item item = Util.GetOrAddComponent<UI_Inventory_Item>(itemGameObject);
            item.SlotId += i;
            Slots.Add(item);
        }
        slotId = Slots.Count;
        
        for (int i = 0; i < 10; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_Inventory_Slot", content.transform);
            GameObject itemGameObject = go.transform.Find("UI_Inventory_Item").gameObject;
            UI_Inventory_Item item = Util.GetOrAddComponent<UI_Inventory_Item>(itemGameObject);

            if (Slots.Count>0)
            {
                item.SlotId = slotId+(i);
            }
            Slots.Add(item);
        }
        _init = true;

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        //if (Slots.Count == 0)
        //    return;
        if (_state == InventoryState.Inventory)
            GetText((int)Texts.ItemNameText).gameObject.transform.parent.parent.gameObject.SetActive(true);
        else
            GetText((int)Texts.ItemNameText).gameObject.transform.parent.parent.gameObject.SetActive(false);

        SetAlphaZero();

        List<Item> items = Managers.Inven.Items.Values.ToList();
        items.Sort((left, right) => { return left.Slot - right.Slot; });

        foreach (Item item in items)
        {
            if (item.Slot < 0 || item.Slot >= 19)
                continue;

         
            if (item.Count <= 0)
                continue;

            UI_Inventory_Item itemUI = Slots[item.Slot].transform.GetComponentInChildren<UI_Inventory_Item>();
            itemUI.CanvasGroup.alpha = 1.0f;
            //itemUI.CanvasGroup.alpha = 0f;

            Slots[item.Slot].SetItem(item);


        }
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].Equipped == false)
                continue;

            // 아이템 설명
            if (_state == InventoryState.Inventory)
            {
                GetImage((int)Images.ItemIconImage).sprite = Slots[i].Icon.sprite;

                StringData name = null;
                StringData description = null;
                Managers.Data.StringDict.TryGetValue($"itemName({Slots[i].TemplatedId})", out name);
                Managers.Data.StringDict.TryGetValue($"itemDesc({Slots[i].TemplatedId})", out description);

                GetText((int)Texts.ItemNameText).text = name.ko;
                GetText((int)Texts.ItemDescriptionText).text = description.ko;
            }
           
        }

        Managers.Inven.UpdateInventoryDatabase();
    }

    private void SetAlphaZero()
    {
        for (int i = 0; i< Slots.Count; i++)
        {
            Slots[i].CanvasGroup.alpha = 0;
            Slots[i].SetFrame(Slots[i].Equipped);
        }

    }

}
