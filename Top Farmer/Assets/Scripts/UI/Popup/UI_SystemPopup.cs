using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_SystemPopup : UI_Popup
{
    enum Texts
    {
        MessageText,
    }
    enum Images
    {
        InteractionBtn,
    }
    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        GetImage((int)Images.InteractionBtn).gameObject.BindEvent(OnInteractionBtnClicked,UIEvent.PointerClick);
    }

    public void SetText(SYSTEM_MESSAGE message)
    {
        switch (message)
        {
            case SYSTEM_MESSAGE.Game_NoAddress:
                GetText((int)Texts.MessageText).text = "계정이 존재하지 않습니다.";
                break;
            case SYSTEM_MESSAGE.Game_WrongPassword:
                GetText((int)Texts.MessageText).text = "계정 또는 비밀번호를 확인해주세요.";
                break;
            case SYSTEM_MESSAGE.Game_CreateAddressOK:
                GetText((int)Texts.MessageText).text = "계정이 생성 되었습니다.";
                break;
        }
    }

    void OnInteractionBtnClicked(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI();
    }

}
