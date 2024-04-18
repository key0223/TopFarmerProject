using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SeedStore_Item : UI_Base
{
    enum Texts
    {
        ItemName,
        ItemCount,
        ItemCost,
    }
    Button _button;
    private Color _originalColor;
    private Color _selectedColor;
    public int TemplatedId { get; private set; }
    public int Count { get; private set; } 
    public int Cost {  get; private set; }
    private bool _selected;
    public bool Selected
    {
        get { return _selected; }
        set
        {
            if (_selected == value)
                _selected = !value;

            _selected = value;
            OnSelected();
        }
    }

    public bool Initialized { get;  set; } = false;

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        _button = GetComponent<Button>();
        ColorBlock colorBlock = _button.colors;
        _originalColor = _button.GetComponent<Image>().color;
        _selectedColor = colorBlock.pressedColor;
    }

    public void SetItem(int templatedId, int count)
    {
        if (Initialized == true)
            return;

        ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(templatedId, out itemData);

        TemplatedId = itemData.itemId;
        Count = count;

        SeedData seedData = (SeedData)itemData;
        int cost = Mathf.FloorToInt( seedData.purchasePrice * count);
        Cost = cost;

        GetText((int)Texts.ItemName).text = seedData.name;
        GetText((int)Texts.ItemCount).text = count.ToString();
        GetText((int)Texts.ItemCost).text = $"{cost} top coin";
        Initialized = true;

    }
    public void OnSelected()
    {
        _button.GetComponent<Image>().color = _selected ? (_originalColor*_selectedColor) : _originalColor;
    }

    public void OnCanceled()
    {
        if (Initialized == false)
            return;
        _button.GetComponent<Image>().color =  _originalColor;
    }
}
