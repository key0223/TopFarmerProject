using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_GameScene : UI_Scene
{
    public UI_Inventory InvenUI { get; private set; }

    public UI_ToolBar ToolBarUI { get; private set; }
    public UI_Day DayUI { get; private set; }
    public UI_Light LightUI { get; private set; }
    public UI_SeedStore SeedStoreUI { get; private set; } 

    public UI_Merchant MerchantUI { get; private set; }
    public UI_ItemInfoPopup InfoPopupUI { get; private set; }

    public UI_Traded TradedUI { get; private set; }
    public UI_Oven OvenUI { get; private set; }
    public override void Init()
    {
        base.Init();

        InvenUI = GetComponentInChildren<UI_Inventory>();
        InvenUI.gameObject.SetActive(false);

        ToolBarUI = GetComponentInChildren<UI_ToolBar>();
        ToolBarUI.gameObject.SetActive(true);

        DayUI = GetComponentInChildren<UI_Day>();
        DayUI.gameObject.SetActive(true);

        LightUI = GetComponentInChildren<UI_Light>();
        LightUI.gameObject.SetActive(true);

        SeedStoreUI = GetComponentInChildren<UI_SeedStore>();
        SeedStoreUI.gameObject.SetActive(true);

        MerchantUI = GetComponentInChildren<UI_Merchant>();
        MerchantUI.gameObject.SetActive(false);

        InfoPopupUI = GetComponentInChildren<UI_ItemInfoPopup>();
        InfoPopupUI.gameObject.SetActive(false);

        TradedUI = GetComponentInChildren<UI_Traded>();
        TradedUI.gameObject.SetActive(false);

        OvenUI = GetComponentInChildren<UI_Oven>();
        OvenUI.gameObject.SetActive(false);
    }

    public void NightUIOff()
    {
        InvenUI.gameObject.SetActive(false);
        ToolBarUI.gameObject.SetActive(false);
        MerchantUI.gameObject.SetActive(false);
        InfoPopupUI.gameObject.SetActive(false);
        TradedUI.gameObject.SetActive(false);
        OvenUI.gameObject.SetActive(false);

        Debug.Log("NightUI Off");
    }
    public void NightUIOn()
    {
        ToolBarUI.gameObject.SetActive(true);

    }
    [ContextMenu("AddPlayerItem")]
    public void AddPlayerItem(/*int playerDbId*/)
    {
        AddItemPacketReq packet = new AddItemPacketReq()
        {
            PlayerDbId = 32,
            TemplatedId = 203,
            Count = 1,
        };
        Managers.Web.SendPostRequest<AddItemPacketRes>("item/addItem", packet, (res) =>
        {
            if (res.Item !=null)
            {
                Item item = Item.MakeItem(res.Item);
                Managers.Inven.Add(item);

                if (res.ExtraItems != null)
                {
                    foreach (ItemInfo iteminfo in res.ExtraItems)
                    {
                        int? slot = Managers.Inven.GetEmptySlot();
                        if (slot == null)
                            return;

                        Item extraItem = Item.MakeItem(iteminfo);
                        extraItem.Slot = (int)slot;
                        Managers.Inven.Add(extraItem);
                    }
                }
            }
            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            gameSceneUI.InvenUI.RefreshUI();
            gameSceneUI.ToolBarUI.RefreshUI();
        });

    }
}
