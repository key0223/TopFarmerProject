using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

public class MerchantController : NpcController
{
    // Npc stringId = NpcId + NpcState + stringId

    public Merchant Merchant { get; private set; }


    public int _remainingDayOffPeriod;
    public int _remainingWorkingInPeriod;
    public int _remainingWorkingOutPeriod;

    private MerchantState _merchantState = MerchantState.Working_Inside;
    public MerchantState MerchantState
    {
        get { return _merchantState; }
        set
        {
            if(_merchantState == value)
                return;

            _merchantState = value;
        }
    }

    protected override void Init()
    {
        base.Init();
        SetNpc();
        _remainingDayOffPeriod = Merchant.DayOffPeriod;
        _remainingWorkingInPeriod = Merchant.WorkingInPeriod;
        _remainingWorkingOutPeriod = Merchant.WorkingOutPeriod;

        Managers.Time.DayPassedRegistered -= OnDayPassed;
        Managers.Time.DayPassedRegistered += OnDayPassed;

    }
    protected override void UpdateController()
    {
        base.UpdateController();
    }
    protected override void SetNpc()
    {
        base.SetNpc();
       Merchant = Npc as Merchant;

    }
    public void OnDayPassed()
    {
        switch (_merchantState)
        {
            case MerchantState.TradeCompleted:
                
                break;
            case MerchantState.Resting:

                // 쉬는 날
                _remainingDayOffPeriod--;
                if (_remainingDayOffPeriod <= 0)
                {
                    _remainingDayOffPeriod = Merchant.DayOffPeriod;
                    //_workingInsidePeriod = 1;

                    MerchantState = MerchantState.Working_Inside;
                }
                break;
            case MerchantState.Working_Inside:

                _remainingWorkingInPeriod--;
                if(_remainingWorkingInPeriod <= 0)
                {
                    Managers.Object.Remove(this.gameObject);
                    _sprite.enabled = false;
                    MerchantState = MerchantState.Working_Outside;
                }
                break;
            case MerchantState.Working_Outside:

                _remainingWorkingOutPeriod--;
                if (_remainingWorkingOutPeriod <= 0)
                {
                    _remainingWorkingOutPeriod = Merchant.WorkingOutPeriod;

                    Managers.Object.Add(this.gameObject);
                    _sprite.enabled = true;
                    MerchantState = MerchantState.TradeCompleted;

                }
                break;
        }
    }
    public void OnInteract()
    {
        Debug.Log(MerchantState);
        switch (MerchantState)
        {
            case MerchantState.TradeCompleted:
                OnTradeCompleted();
                break;
            case MerchantState.Resting:
                OnResting();
                break;
            case MerchantState.Working_Inside:
                OnWorking_Inside();
                break;
            //case MerchantState.Working_Outside:
            //    break;
        }
    }

    private void OnTradeCompleted()
    {
        UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
        UI_Traded tradedUI = gameScene.TradedUI;
        UI_Merchant merchantUI = gameScene.MerchantUI;

        if (tradedUI.gameObject.activeSelf)
        {
            tradedUI.Clear();
            tradedUI.gameObject.SetActive(false);
        }
        else
        {
            tradedUI.gameObject.SetActive(true);
            List<InteractItem> interactItems = merchantUI.Items.Values.ToList();
            tradedUI.SetItems(interactItems);

            // npc 대화 설정
            int randStringId = Random.Range(1, 6);
            string messageId = string.Format("npcText({0}{1}{2})", Merchant.Info.templatedId, (int)MerchantState, randStringId);
            string randMessage = Managers.Data.StringDict[messageId].ko;
            tradedUI.SetMessageText(randMessage);
        }

    }
    private void OnResting()
    {
        UI_TextBox activeTextbox = transform.GetComponentInChildren<UI_TextBox>();
       if(activeTextbox == null)
        {
            UI_TextBox textBoxUI = Managers.UI.MakeWorldSpaceUI<UI_TextBox>(transform);

            int randStringId = Random.Range(1, 6);
            string messageId = string.Format("npcText({0}{1}{2})", Merchant.Info.templatedId, (int)MerchantState, randStringId);
            string randMessage = Managers.Data.StringDict[messageId].ko;
            textBoxUI.SetMessageText(randMessage);
        }
    }
    private void OnWorking_Inside()
    {
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Merchant merchantUI = gameSceneUI.MerchantUI;
        UI_Inventory invenUI = gameSceneUI.InvenUI;
        
        if (merchantUI.gameObject.activeSelf)
        {
            invenUI.gameObject.SetActive(false);
            merchantUI.gameObject.SetActive(false);

            invenUI.State = InventoryState.Inventory;
        }
        else
        {
            invenUI.State = InventoryState.Merchant;
            invenUI.gameObject.SetActive(true);
            merchantUI.gameObject.SetActive(true);

            // npc 대화 설정
            int randStringId = Random.Range(1, 6);
            string messageId = string.Format("npcText({0}{1}{2})", Merchant.Info.templatedId, (int)MerchantState, randStringId);
            string randMessage = Managers.Data.StringDict[messageId].ko;
            merchantUI.SetMessageText(randMessage);

        }
    }
    private void OnWorking_Outside()
    {
    }
   
}
