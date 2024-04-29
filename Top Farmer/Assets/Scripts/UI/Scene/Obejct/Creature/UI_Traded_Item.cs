using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Traded_Item : UI_Base
{
    enum Images
    {
        ItemIconImage,
    }
    enum Texts
    {
        ItemNameText,
        ItemCountText,
        CoinText
    }

    public int TemplatedId { get; private set; }
    public int Count { get; private set; }
    public int Coin { get; private set; }

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

    }

    public void SetUI(InteractItem interactItem)
    {
        TemplatedId = interactItem.templatedId;
        Count = interactItem.count;
        Coin = CalculateTotalCoin();

        ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(TemplatedId, out itemData);

        GetImage((int)Images.ItemIconImage).sprite = Managers.Resource.Load<Sprite>($"{itemData.iconPath}");
        GetText((int)Texts.ItemNameText).text = itemData.name;
        GetText((int)Texts.ItemCountText).text = $"x {Count}";
        GetText((int)Texts.CoinText).text = Coin.ToString();
    }

    public int CalculateTotalCoin()
    {
        ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(TemplatedId, out itemData);

        int sellingPrice = 0;
        switch(itemData.itemType)
        {
            case ItemType.ITEM_TYPE_TOOL:
                {

                }
                break;
            case ItemType.ITEM_TYPE_CROP:
                {
                    CropData cropData = (CropData)itemData;
                    sellingPrice = cropData.sellingPrice;
                }
                break;
            case ItemType.ITEM_TYPE_SEED:
                {
                    SeedData seedData = (SeedData)itemData;
                    sellingPrice = seedData.sellingPrice;
                }
                break;
            case ItemType.ITEM_TYPE_FOOD:
                {
                    FoodData foodData = (FoodData)itemData;
                    sellingPrice = foodData.sellingPrice;
                }
                break;
        }

        int total = Count * sellingPrice;
        return total;
    }
}
