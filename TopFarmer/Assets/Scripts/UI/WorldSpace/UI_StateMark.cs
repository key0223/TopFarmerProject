using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_StateMark : UI_Base
{
    enum Images
    {
        FrameImage,
        StateImage,
    }
    public override void Init()
    {
        Bind<Image>(typeof(Images));
    }

    public void SetStateMark(StateMarkState state )
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.transform.localPosition = new Vector3(0, 0.7f, 0);

        string basePath = "Textures/Icon";
        switch (state)
        {
            case StateMarkState.ExclamationMark:
                {
                    Sprite icon = Managers.Resource.Load<Sprite>($"{basePath}/ExclamationMark");
                    GetImage((int)Images.StateImage).sprite = icon;
                }
                break;
            case StateMarkState.ExclamationMark_Red:
                {
                    Sprite icon = Managers.Resource.Load<Sprite>($"{basePath}/ExclamationMark_Red");
                    GetImage((int)Images.StateImage).sprite = icon;
                }
               
                break;
            case StateMarkState.QuestionMark:
                {
                    Sprite icon = Managers.Resource.Load<Sprite>($"{basePath}/QuestionMark");
                    GetImage((int)Images.StateImage).sprite = icon;
                }
                break;
            case StateMarkState.ProgressingMark:
                {
                    Sprite icon = Managers.Resource.Load<Sprite>($"{basePath}/ProgressingMark");
                    GetImage((int)Images.StateImage).sprite = icon;
                }
                break;
        }
    }
}
