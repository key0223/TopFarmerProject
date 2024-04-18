using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_Inventory_Slot : UI_Base
{
    enum GameObjects
    {
        SelectedFrame,
        UI_Inventory_Item,
    }

    private UI_Inventory_Item _invenItem;
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        GetObject((int)GameObjects.SelectedFrame).SetActive(false);

        _invenItem = GetObject((int)GameObjects.UI_Inventory_Item).GetComponent<UI_Inventory_Item>();
    }

    private void ItemSet()
    {
        Item item = Managers.Inven.Get(_invenItem.ItemDbId);
        GetObject((int)GameObjects.SelectedFrame).SetActive(item.Equipped);
    }

    public void SlotClear(bool equipped)
    {
        GetObject((int)GameObjects.SelectedFrame).SetActive(equipped);
    }
    public void SlotClear()
    {
        GetObject((int)GameObjects.SelectedFrame).SetActive(false);
    }
}
