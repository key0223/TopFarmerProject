using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager 
{
    // 팝업 sort order 정리
    int _order = 10;

    // 가장 마지막에 띄운 팝업 먼저 삭제
    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    public UI_Scene SceneUI { get; private set; }

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = Managers.Resource.Instantiate("UI/@UI_Root");

            return root.gameObject.transform.GetChild(0).gameObject;
        }
    }
    // 팝업 UI가 켜질때 캔버스에 있는 order를 채워달라고 요청
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas _canvas = Util.GetOrAddComponent<Canvas>(go);
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // 캔버스가 중첩으로 있을때 부모가 어떤 값을 가지던 무조건 자기 소팅 오더를 가진다.
        _canvas.overrideSorting = true;

        if (sort)
        {
            _canvas.sortingOrder = (_order);
            _order++;
        }
        else
        {
            // PopupUI가 아닌 일반 UI
            _canvas.sortingOrder=0;
        }

    }
    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}");
        if (parent != null)
            go.transform.SetParent(parent,false);

        Canvas _canvas = go.GetOrAddComponent<Canvas>();
        _canvas.renderMode = RenderMode.WorldSpace;
        _canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(go);
    }
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        // 이름을 안받았다면 T의 이름을 사용한다.
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        // UI 프리팹 생성
        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        SceneUI = sceneUI;

        go.transform.SetParent(Root.transform);
        return sceneUI;

    }
    // T =  script , name = prefab name
    public T ShowPopUI<T>(string name = null) where T : UI_Popup
    {
        // 이름을 안받았다면 T의 이름을 사용한다.
        if(string.IsNullOrEmpty(name))
            name  = typeof(T).Name;

        // UI 프리팹 생성
        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup =  Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);
        return popup;

    }

    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return;

        if(_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed");
            return;
        }

        ClosePopupUI();
    }
    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;

        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }
    public void Clear()
    {
        CloseAllPopupUI();
        SceneUI = null;
    }
}
