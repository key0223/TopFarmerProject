using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Interaction : UI_Base
{
    enum Texts
    {
        StringText,
    }
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        GetText((int)Texts.StringText).gameObject.BindEvent(OnBtnClick, Define.UIEvent.PointerClick);
    }

    #region Mouse Interaction
    
    public void OnBtnClick(PointerEventData  evt)
    {

    }
    #endregion
}
