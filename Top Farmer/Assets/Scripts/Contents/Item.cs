using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using static Define;


public class ItemInfo
{
    public int itemDbId;
    public int templatedId;
    public int count;
    public int slot;
    public bool equipped;
}

public class Item
{
    public ItemInfo Info { get; } = new ItemInfo();
    public ItemType ItemType { get; private set; }  
    public bool Stackable { get;protected set; }
    public int MaxStack { get; protected set; }
    public int ItemDbId
    {
        get { return Info.itemDbId; }
        set { Info.itemDbId = value; }
    }
    public int TemplatedId
    {
        get { return Info.templatedId; }
        set { Info.templatedId = value; }
    }
    public int Count
    {
        get { return Info.count; }
        set { Info.count = value; }
    }

    public int Slot
    {
        get { return Info.slot; }
        set { Info.slot = value; }
    }
    public bool Equipped
    {
        get { return Info.equipped; }
        set { Info.equipped = value; }
    }

    // 생성자를 이용하여 인스턴스 초기화
    public Item(ItemType itemType)
    {
        ItemType = itemType;
    }

    public static Item MakeItem(ItemInfo itemInfo)
    {
        Item item = null;

        ItemData itemData = null;

        Managers.Data.ItemDict.TryGetValue(itemInfo.templatedId, out itemData);
        if (itemData == null)
            return null;
        

        switch (itemData.itemType)
        {
            case ItemType.ITEM_TYPE_TOOL:
                item = new Tool(itemInfo.templatedId);
                break;
            case ItemType.ITEM_TYPE_CROP:
                item = new Crop(itemInfo.templatedId);
                break;
            case ItemType.ITEM_TYPE_SEED:
                item = new Seed(itemInfo.templatedId);
                break;
            case ItemType.ITEM_TYPE_CRAFTING:
                item = new Crafting(itemInfo.templatedId);
                break;
            case ItemType.ITEM_TYPE_FOOD:
                item = new Food(itemInfo.templatedId);
                break;

        }
        if (item != null)
        {
            item.ItemDbId = itemInfo.itemDbId;
            item.Count = itemInfo.count;
            item.Slot = itemInfo.slot;
            item.Equipped = itemInfo.equipped;
        }

        return item;
    }
    
}
public class Tool : Item
{
    public ToolType ToolType { get; private set; }
    public int Range { get; private set; }
    public int Durability { get; private set; }

    public Tool(int templatedId) : base(ItemType.ITEM_TYPE_TOOL)
    {
        Init(templatedId);
    }

    void Init(int templatedId)
    {
        ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(templatedId, out itemData);
        if (itemData.itemType != ItemType.ITEM_TYPE_TOOL)
            return;

        ToolData data = (ToolData)itemData;
        {
            TemplatedId = data.itemId;
            Count = 1;
            MaxStack = data.maxStack;
            ToolType = data.toolType;
            Range = data.range;
            Durability = data.durability;
            Stackable = (data.maxStack > 1);
        }
    }

}
public class Crop : Item
{
    public int SellingPrice { get; private set; }
    public Crop(int templatedId) : base(ItemType.ITEM_TYPE_CROP)
    {
        Init(templatedId);
    }

    void Init(int templatedId)
    {
        ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(templatedId, out itemData);
        if (itemData.itemType != ItemType.ITEM_TYPE_CROP)
            return;

        CropData data = (CropData)itemData;
        {
            TemplatedId = data.itemId;
            Count = 1;
            MaxStack = data.maxStack;
            SellingPrice = data.sellingPrice;
            Stackable = (data.maxStack > 1);
        }
    }
}

public class Seed : Item
{

    public int GrowthDay { get; private set; }
    public int PurchasePrice { get; private set; }
    public int SellingPrice { get; private set; }
    public int ResultCrop { get; private set; }
    public int CropQuantity { get; private set; }
    public string GrowthSprite1 { get; private set; }
    public string GrowthSprite2 { get; private set; }
    public string GrowthSprite3 { get; private set; }


    public Seed(int templatedId) : base(ItemType.ITEM_TYPE_SEED)
    {
        Init(templatedId);
    }

    void Init(int templatedId)
    {
        ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(templatedId, out itemData);
        if (itemData.itemType != ItemType.ITEM_TYPE_SEED)
            return;

        SeedData data = (SeedData)itemData;
        {
            TemplatedId = data.itemId;
            Count = 1;
            MaxStack = data.maxStack;
            Stackable = (data.maxStack > 1);
            GrowthDay = data.growthDay;
            PurchasePrice = data.purchasePrice;
            SellingPrice = data.sellingPrice;
            ResultCrop = data.resultCrop;
            CropQuantity = data.cropQuantity;
            GrowthSprite1 = data.growthSprite1;
            GrowthSprite2 = data.growthSprite2;
            GrowthSprite3 = data.growthSprite3;
        }
    }
}

public class Crafting :Item
{
    public CraftingType CraftingType { get; private set; }
    public bool Sellable { get; private set; }
    public bool Destroyable { get;private set; }
    public int SizeX { get; private set; }
    public int SizeY {  get; private set; }
    public string PrefabPath { get; private set; }


    public Crafting(int templatedId) : base(ItemType.ITEM_TYPE_CRAFTING)
    {
        Init(templatedId);
    }

    void Init(int templatedId)
    {
        ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(templatedId, out itemData);
        if (itemData.itemType != ItemType.ITEM_TYPE_CRAFTING)
            return;

        CraftingData data = (CraftingData)itemData;
        {
            TemplatedId = data.itemId;
            Count = 1;
            MaxStack = data.maxStack;
            Stackable = (data.maxStack > 1);
            CraftingType = data.craftingType;
            Sellable = data.sellable;
            Destroyable = data.destroyable;
            SizeX = data.sizeX;
            SizeY = data.sizeY;
            PrefabPath = data.prefabPath;
            
        }
    }
}
public class Food : Item
{
    public bool Sellable { get;private set; }
    public bool Eatable { get;private set; }
    public int SellingPrice { get;private set; } 
    public Food(int templatedId) : base(ItemType.ITEM_TYPE_FOOD)
    {
        Init(templatedId);
    }

    void Init(int templatedId)
    {
        ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(templatedId, out itemData);
        if (itemData.itemType != ItemType.ITEM_TYPE_FOOD)
            return;

        FoodData data = (FoodData)itemData;
        {
            TemplatedId = data.itemId;
            Count = 1;
            MaxStack = data.maxStack;
            Stackable = (data.maxStack > 1);
            Sellable = data.sellable;
            Eatable = data.eatable;
            SellingPrice = data.sellingPrice;
        }
    }
}
