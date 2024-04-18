using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;
public class InteractItem
{
    public int itemDbId;
    public int templatedId;
    public int count;
    public int slot;
    public bool stackable;
}
public class UI_Merchant_Item : UI_Base
{
    enum Texts
    {
        ItemCountText,
    }
    enum GameObjects
    {
        SelectedFrame,
    }

    private InventoryState _state = InventoryState.Merchant;
    public int SlotId { get; set; }
    public int ItemDbId { get; set; }
    public int TemplatedId { get; set; }
    public int Count { get; set;}
    public bool Equipped { get; set; }

    private Image _icon = null;
    public Image Icon { get{ return _icon; }}
    private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup { get { return _canvasGroup; } }
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        GetObject((int)GameObjects.SelectedFrame).SetActive(false);

        _icon = GetComponent<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();

        gameObject.BindEvent(OnPointerClick, Define.UIEvent.PointerClick);
    }
    public void SetUI(InteractItem item)
    {
        if(item == null)
            return;

        ItemDbId = item.itemDbId;
        TemplatedId = item.templatedId;
        Count = item.count;
        SlotId = item.slot;

        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(TemplatedId, out itemData);
        _icon.sprite = Managers.Resource.Load<Sprite>($"{itemData.iconPath}");
        _canvasGroup.alpha = 1;
        GetText((int)Texts.ItemCountText).text = item.count.ToString();
    }

    public void ClearUI()
    {
        ItemDbId = 0;
        TemplatedId = 0;
        Count = 0;
        SlotId = 0;

        _canvasGroup.alpha = 0f;
        GetText((int)Texts.ItemCountText).text = " ";
    }

    public void SetFrame(bool equipped = false)
    {
        GetObject((int)GameObjects.SelectedFrame).SetActive(equipped);
    }

    #region Mouse Interaction

    
    private void OnPointerClick(PointerEventData evt)
    {
        if(ItemDbId == 0) return;

        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_ToolBar toolbarUI = gameSceneUI.ToolBarUI;

        UI_ItemInfoPopup popupUI = gameSceneUI.InfoPopupUI;
        popupUI.gameObject.SetActive(true);
        popupUI.SetItemInfo(ItemDbId,_state);
    }
    #endregion
}
