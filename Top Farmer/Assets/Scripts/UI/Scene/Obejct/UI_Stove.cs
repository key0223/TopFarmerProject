using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_Stove : UI_Base
{
    #region Event
    public delegate void CompleteCookRegisterHandler();
    public event CompleteCookRegisterHandler CompleteCookRegistered;
    #endregion
    enum Images
    {
        StoveOnImage,
        StoveOffImage,
        StoveOnBtnImage,
        StoveOffBtnImage,
    }

    private StoveState _state = StoveState.Empty;
    public StoveState State
    {
        get { return _state; }
        set { _state = value; }
    }

    private int _templatedId;
    private int _count;
    private bool _stackable;
    private int _resultFood;

    public override void Init()
    {
        Bind<Image>(typeof(Images));

        GetImage((int)Images.StoveOnBtnImage).gameObject.BindEvent(OnStoveBtnClick, UIEvent.PointerClick);
        //GetImage((int)Images.StoveOffBtnImage).gameObject.BindEvent(OnStoveBtnClick, UIEvent.PointerClick);

        GetImage((int)Images.StoveOnImage).gameObject.SetActive(false);
        GetImage((int)Images.StoveOnBtnImage).gameObject.SetActive(false);
    }

    // 조리 도구를 스토브 위에 생성합니다.
    public void SetInteractItem(int stoveId,InteractItem interactItem)
    {
        _state = StoveState.Using;

        _templatedId = interactItem.templatedId;
        _count = interactItem.count;
        _stackable = interactItem.stackable;

        GameObject go =  Managers.Resource.Instantiate("UI/Scene/Object/UI_Pan");
        go.transform.SetParent(transform.parent.parent);
        go.transform.localScale = Vector3.one;
        go.transform.position = transform.position;
        UI_Pan pan = go.GetComponent<UI_Pan>();

        string strFoodId = $"5{interactItem.templatedId}";
        int foodId = int.Parse(strFoodId);

        ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(foodId, out itemData);
        FoodData foodData = (FoodData)itemData;
        _resultFood = foodData.itemId;

        pan.OnCook(stoveId,foodData.time);
        StoveOnOff();
    }

    /// <summary>
    /// 서버에 아이템 생성을 요청합니다.
    /// </summary>
    private void AddInven()
    {
        int? findSlot = Managers.Inven.GetEmptySlot();
        AddItemPacketReq packet = new AddItemPacketReq()
        {
            PlayerDbId = Managers.Object.PlayerInfo.PlayerDbId,
            TemplatedId = _resultFood,
            Count = _count,
            Slot = (int)findSlot,
        };

        Managers.Web.SendPostRequest<AddItemPacketRes>("item/addItem", packet, (res) =>
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

            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            gameSceneUI.InvenUI.RefreshUI();
            gameSceneUI.ToolBarUI.RefreshUI();
        });
    }
    public void StoveOnOff()
    {
        if (GetImage((int)Images.StoveOnBtnImage).gameObject.activeSelf)
        {
            GetImage((int)Images.StoveOnImage).gameObject.SetActive(false);
            GetImage((int)Images.StoveOnBtnImage).gameObject.SetActive(false);

            GetImage((int)Images.StoveOffImage).gameObject.SetActive(true);
            GetImage((int)Images.StoveOffBtnImage).gameObject.SetActive(true);
        }
        else
        {
            GetImage((int)Images.StoveOnImage).gameObject.SetActive(true);
            GetImage((int)Images.StoveOnBtnImage).gameObject.SetActive(true);

            GetImage((int)Images.StoveOffImage).gameObject.SetActive(false);
            GetImage((int)Images.StoveOffBtnImage).gameObject.SetActive(false);
        }
    }

    #region Mouse Interaction
    public void OnStoveBtnClick(PointerEventData evt)
    {
        switch (State)
        {
            case StoveState.Empty:
                break;
            case StoveState.Using:
                break;
            case StoveState.Completed:
                {
                    StoveOnOff();
                    _state = StoveState.Empty;
                    CompleteCookRegistered?.Invoke();

                    // TODO : 인벤토리에 넣기 (DB 저장)
                    AddInven();
                }
                break;
        }
      
    }
    #endregion

}
