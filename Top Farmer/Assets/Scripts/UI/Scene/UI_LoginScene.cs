using Assets.Scripts.Contents.Object;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_LoginScene : UI_Scene
{
    enum GameObjects
    {
        AccountName,
        Password
    }
    enum Texts
    {
        LoginBtnText,
        CreateBtnText,
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));

        GetText((int)Texts.LoginBtnText).gameObject.BindEvent(OnClickedLoginButton);
        GetText((int)Texts.CreateBtnText).gameObject.BindEvent(OnClickedCreateButton);
    }

    public void OnClickedLoginButton(PointerEventData evt)
    {
        string account = Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text;
        string password = Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text;

        LoginAccountPacketReq packet = new LoginAccountPacketReq()
        {
            AccountName = account,
            Password = password
        };

        Managers.Web.SendPostRequest<LoginAccountPacketRes>("account/login", packet, (res) =>
        {
            Debug.Log(res.LoginResult);
            Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text = "";
            Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text = "";

            switch (res.LoginResult)
            {
                case SYSTEM_MESSAGE.Game_NoAddress:
                    {
                        UI_SystemPopup popup = Managers.UI.ShowPopUI<UI_SystemPopup>();
                        popup.SetText(SYSTEM_MESSAGE.Game_NoAddress);
                    }
                    break;
                case SYSTEM_MESSAGE.Game_LoginOK:
                    //Load GameScene 
                    Managers.Scene.LoadScene(Define.Scene.Game);
                    // 일단 무조건 첫번째로 로그인
                    PlayerInfo player = res.Players[0];

                    GetPlayerData(player.PlayerDbId);
                    Managers.Object.PlayerInfo = player;
                    break;
                case SYSTEM_MESSAGE.Game_WrongPassword:
                    {
                        UI_SystemPopup popup = Managers.UI.ShowPopUI<UI_SystemPopup>();
                        popup.SetText(SYSTEM_MESSAGE.Game_WrongPassword);
                    }
                    break;
            }
        });
    }

    public void GetPlayerData(int playerDbId)
    {
        Managers.Inven.Clear();

        GetPlayerDataPacketReq packet = new GetPlayerDataPacketReq()
        {
            PlayerDbId = playerDbId,
        };
        Managers.Web.SendPostRequest<GetPlayerDataPacketRes>("account/getplayerdata", packet, (res) =>
        {
            if (res.Items.Count > 0)
            {
                foreach (ItemInfo itemInfo in res.Items)
                {
                    Item item = Item.MakeItem(itemInfo);
                    Managers.Inven.Add(item);
                }
            }
            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            gameSceneUI.InvenUI.RefreshUI();
            gameSceneUI.ToolBarUI.RefreshUI();
            gameSceneUI.MerchantUI.RefreshUI();
        });

    }
    public void OnClickedCreateButton(PointerEventData evt)
    {
        string account = Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text;
        string password = Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text;

        CreateAccountPacketReq packet = new CreateAccountPacketReq()
        {
            AccountName = account,
            Password = password
        };

        Managers.Web.SendPostRequest<CreateAccountPacketRes>("account/create", packet, (res) =>
        {
            Debug.Log(res.CreateOk);
            Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text = "";
            Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text = "";

            if(res.CreateOk)
            {
                UI_SystemPopup popup = Managers.UI.ShowPopUI<UI_SystemPopup>();
                popup.SetText(SYSTEM_MESSAGE.Game_CreateAddressOK);
            }
        });
    }
}
