using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropController : ItemController
{
    public Crop Crop { get; private set; }
    private CropData _cropData = null;

    protected override void Init()
    {
        base.Init();
        
    }

    public void SetIem()
    {
        Crop = Item as Crop;
        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(Item.TemplatedId, out itemData);
        _cropData = (CropData)itemData;

        _sprite = gameObject.GetComponent<SpriteRenderer>();
        Sprite sprite = Managers.Resource.Load<Sprite>($"{_cropData.iconPath}");
        _sprite.sprite = sprite;
    }

   
}
