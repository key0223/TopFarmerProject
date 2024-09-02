using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryTextBox : UI_Base
{
    enum Texts
    {
        Text1,
        Text2,
        Text3,
        Text4,
        Text5,
        PriceText,
    }
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
    }

    public void SetTextBoxText(string text1, string text2, string text3, string text4, string text5, string priceText)
    {
        GetText((int)Texts.Text1).text = text1;
        GetText((int)Texts.Text2).text = text2;
        GetText((int)Texts.Text3).text = text3;
        GetText((int)Texts.Text4).text = text4;
        GetText((int)Texts.Text5).text = text5;
        GetText((int)Texts.PriceText).text = priceText;
    }
}
