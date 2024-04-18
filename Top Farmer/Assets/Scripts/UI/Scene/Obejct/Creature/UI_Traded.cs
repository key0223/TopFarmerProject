using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_Traded : UI_Base
{
    enum Texts
    {
        MessageText,
        TotalValueText,
    }
    enum GameObjects
    {
        TradedItemList,
        ConfirmBtnText,
    }
    public List<UI_Traded_Item> Items { get; } = new List<UI_Traded_Item>();

    int _totalCoin = 0;
    public override void Init()
    {
        Clear();

        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        GetObject((int)GameObjects.ConfirmBtnText).BindEvent(OnConfirmBtnClick, Define.UIEvent.PointerClick);

        GameObject itemList = GetObject((int)GameObjects.TradedItemList);
        foreach(Transform child in itemList.transform)
            Destroy(child.gameObject);

    }

    public void SetItems(List<InteractItem> interactItems)
    {
        // TODO :  아이템 슬롯 생성 후 데이터 초기화

        Clear();

        GameObject itemList = GetObject((int)GameObjects.TradedItemList);
        foreach (InteractItem interactItem in interactItems)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_Traded_Item", itemList.transform);
            UI_Traded_Item item = Util.GetOrAddComponent<UI_Traded_Item>(go);
            item.SetUI(interactItem);
            _totalCoin += item.Coin;
            Items.Add(item);
        }

        GetText((int)Texts.TotalValueText).text = _totalCoin.ToString();

    }

    public void SetMessageText(string text)
    {
        GetText((int)Texts.MessageText).text = text;
    }

    public void Clear()
    {
        if(Items.Count> 0)
        {
            foreach (UI_Traded_Item item in Items)
            {
                GameObject go = item.gameObject;
                Managers.Resource.Destroy(go);
            }
        }
        _totalCoin = 0;
        Items.Clear();
    }
    #region Mouse Interaction

    public void OnConfirmBtnClick(PointerEventData evt)
    {
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_ToolBar toobarUI = gameSceneUI.ToolBarUI;

        Managers.Object.Player.Info.Coin += _totalCoin;
        toobarUI.RefreshUI();
        Clear();

        MerchantController mc =  Managers.Object.GetMerchant().GetComponent<MerchantController>();
        mc.MerchantState = MerchantState.Resting;
        gameObject.SetActive(false);
    }
    #endregion

}
