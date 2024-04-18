using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UI_TextBox : UI_Base
{
    enum Texts
    {
        MessageText
    }
    public override void Init()
    {
       Bind<Text>(typeof(Texts));
    }

    private void OnEnable()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.position = new Vector3(0, 0.7f, 0);
        StartCoroutine(CoStartTextMessage());
    }

    public void SetMessageText(string text)
    {
        GetText((int)Texts.MessageText).text = text;
    }

    IEnumerator CoStartTextMessage()
    {
        yield return new WaitForSeconds(2f);
        Managers.Resource.Destroy(gameObject);
    }



}
