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
                GetText((int)Texts.MessageText).text = "������ �������� �ʽ��ϴ�.";
                break;
            case SYSTEM_MESSAGE.Game_WrongPassword:
                GetText((int)Texts.MessageText).text = "���� �Ǵ� ��й�ȣ�� Ȯ�����ּ���.";
                break;
            case SYSTEM_MESSAGE.Game_CreateAddressOK:
                GetText((int)Texts.MessageText).text = "������ ���� �Ǿ����ϴ�.";
                break;
        }
    }

    void OnInteractionBtnClicked(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI();
    }

}
