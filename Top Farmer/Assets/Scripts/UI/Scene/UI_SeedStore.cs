using Data;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;
using static UnityEditor.Progress;

public class UI_SeedStore : UI_Base
{
    enum Buttons
    {
        SeedStoreBtn,
        UI_SeedStoreItem_0,
        UI_SeedStoreItem_1,
        UI_SeedStoreItem_2,
        PurchaseBtn,
        CancelBtn,
    }
    enum GameObjects
    {
        ItemGrid,
        CheckPurchase,
    }
    enum Texts
    {
        PurchaseText,
        ItemNameText,
    }

    private SeedStoreState _state = SeedStoreState.Available;

    private int _selectedItemId;
    private int _selectedItemCount;
    private int _selectedItemCost;

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.SeedStoreBtn).gameObject.BindEvent(OnSeedStoreBtnClicked);
        for (int i = 1; i < 4; i++)
        {
            GetButton(i).gameObject.BindEvent(OnStoreItemBtnClicked);
        }
        GetButton((int)Buttons.CancelBtn).gameObject.BindEvent(OnCancelBtnClicked);
        GetButton((int)Buttons.PurchaseBtn).gameObject.BindEvent(OnPurchaseBtnClicked);


        Managers.Time.DayPassedRegistered -= OnNextDay;
        Managers.Time.DayPassedRegistered += OnNextDay;

        HideStore();

    }
    
    private void OnNextDay()
    {
        _state = SeedStoreState.Available;
        GetButton((int)Buttons.SeedStoreBtn).interactable = true;

        for (int i = 1; i < 4; i++)
        {
            UI_SeedStore_Item item = GetButton(i).GetComponent<UI_SeedStore_Item>();
            item.OnCanceled();
            item.Initialized = false;
        }

        HideStore();

    }

    private void OnSeedStoreBtnClicked(PointerEventData evt)
    {
        if (_state != SeedStoreState.Available)
            return;

        GetObject((int)GameObjects.ItemGrid).SetActive(true);
        GetButton((int)Buttons.CancelBtn).gameObject.SetActive(true);
        _state = SeedStoreState.ItemGrid;

        // SeedStore Item Setting
        SetRandomItem();
    }
    private void OnStoreItemBtnClicked(PointerEventData evt)
    {
        GetButton((int)Buttons.PurchaseBtn).gameObject.SetActive(true);
        UI_SeedStore_Item clickedItem = evt.pointerClick.gameObject.GetComponent<UI_SeedStore_Item>();
        if (clickedItem == null)
            return;
        _selectedItemId = clickedItem.TemplatedId;
        _selectedItemCount = clickedItem.Count;
        _selectedItemCost = clickedItem.Cost;

        for (int i = 1; i < 4; i++)
        {
            UI_SeedStore_Item item = GetButton(i).GetComponent<UI_SeedStore_Item>();
            item.Selected = (clickedItem == item);
        }
    }

    private void OnCancelBtnClicked(PointerEventData evt)
    {
        switch (_state)
        {
            case SeedStoreState.ItemGrid:
                for (int i = 1; i < 4; i++)
                {
                    UI_SeedStore_Item item = GetButton(i).GetComponent<UI_SeedStore_Item>();
                    item.OnCanceled();
                }
                HideStore();
                _state = SeedStoreState.Available;
                break;
            case SeedStoreState.CheckPurchase:
                GetObject((int)GameObjects.CheckPurchase).SetActive(false);
                _state = SeedStoreState.ItemGrid;
                break;
        }
    }
    private void OnPurchaseBtnClicked(PointerEventData evt)
    {
        switch (_state)
        {
            case SeedStoreState.ItemGrid:
                GetObject((int)GameObjects.CheckPurchase).SetActive(true);
                _state = SeedStoreState.CheckPurchase;

                ItemData itemData = null;
                Managers.Data.ItemDict.TryGetValue(_selectedItemId, out itemData);
                if (itemData == null)
                    return;
                GetText((int)Texts.ItemNameText).text = $"{itemData.name}<color=#D1D1D1>?</color>";

                break;
            case SeedStoreState.CheckPurchase:

                if (Purchase() == 0)
                {
                    Debug.Log("재화 또는 공간이 부족합니다");
                    return;
                }

                AddItemPacketReq packet = new AddItemPacketReq()
                {
                    PlayerDbId = Managers.Object.PlayerInfo.PlayerDbId,
                    TemplatedId = _selectedItemId,
                    Count = _selectedItemCount,
                };

                Managers.Web.SendPostRequest<AddItemPacketRes>("item/addItem", packet, (res) =>
                {
                    Item item = Item.MakeItem(res.Item);
                    Managers.Inven.Add(item);

                    UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
                    gameSceneUI.InvenUI.RefreshUI();
                    gameSceneUI.ToolBarUI.RefreshUI();
                });
                /*
                Item duplicatedItem = Managers.Inven.FindDuplicatedItem(_selectedItemId);
                if (duplicatedItem == null)
                {
                    int? slot = Managers.Inven.GetEmptySlot();
                    if (slot == null)
                        return;

                    ItemInfo itemInfo = new ItemInfo()
                    {
                        templatedId = _selectedItemId,
                        count = _selectedItemCount,
                        slot = slot.Value,
                        equipped = false,
                    };

                    Item item = Item.MakeItem(itemInfo);
                    Managers.Inven.Add(item);
                }
                */

                for (int i = 1; i < 4; i++)
                {
                  
                    UI_SeedStore_Item item = GetButton(i).GetComponent<UI_SeedStore_Item>();
                    item.Initialized = false;
                }
                HideStore();
                GetButton((int)Buttons.SeedStoreBtn).interactable = false;
                _state = SeedStoreState.Unavailable;

                Managers.Object.Player.Info.Coin -= _selectedItemCost;
                UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
                gameSceneUI.InvenUI.RefreshUI();
                gameSceneUI.ToolBarUI.RefreshUI();
                break;
        }
    }

    private int Purchase()
    {
        //int playerCoin = Managers.Inven.PlayerCoin;
        int playerCoin = Managers.Object.PlayerInfo.Coin;

        int? slot = Managers.Inven.GetEmptySlot();

        int result = 1;

        if (_selectedItemCost > playerCoin || slot == null)
            result = 0;

        return result;
        

    }
    private void SetRandomItem()
    {
        for (int i = 1; i < 4; i++)
        {
            int randItem = Random.Range(201, 211);
            int randCount = Random.Range(1, 51);
            UI_SeedStore_Item item = GetButton(i).GetComponent<UI_SeedStore_Item>();
            item.SetItem(randItem, randCount);
        }
    }
    private void HideStore()
    {
        GetObject((int)GameObjects.ItemGrid).SetActive(false);
        GetObject((int)GameObjects.CheckPurchase).SetActive(false);
        GetButton((int)Buttons.PurchaseBtn).gameObject.SetActive(false);
        GetButton((int)Buttons.CancelBtn).gameObject.SetActive(false);
    }


}
