using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class UI_ToolBar : UI_Base
{
    enum GameObjects
    {
        Slots,
    }

    enum Texts
    {
        MoneyValueText
    }
    bool _init = false;

    public List<UI_Inventory_Item> Slots { get; } = new List<UI_Inventory_Item>();
    public override void Init()
    {
        Slots.Clear();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));

        GetText((int)Texts.MoneyValueText).text = Managers.Object.PlayerInfo.Coin.ToString();

        GameObject slots = GetObject((int)GameObjects.Slots);
        foreach(Transform child in slots.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < 8; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_Inventory_Slot", slots.transform);
            GameObject itemGameObject = go.transform.Find("UI_Inventory_Item").gameObject;
            UI_Inventory_Item item = Util.GetOrAddComponent<UI_Inventory_Item>(itemGameObject);
            item.SlotId += i;
            Slots.Add(item);
        }

        _init = true;
        RefreshUI();
    }

    public void RefreshUI()
    {

        if (_init == false)
            return;

        // 우선 다 투명하게 만든다.
        SetAlphaZero();

        
        List<Item> items = Managers.Inven.Items.Values.ToList();
        items.Sort((left, right) => { return left.Slot - right.Slot; });
        
        foreach(Item item in items)
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

        GetText((int)Texts.MoneyValueText).text = Managers.Object.Player.Info.Coin.ToString();
    }

  
    private void SetAlphaZero()
    {
       for (int i = 0; i<Slots.Count; i++)
        {
            Slots[i].CanvasGroup.alpha = 0;
            Slots[i].SetFrame(Slots[i].Equipped);
        }
    }
}
