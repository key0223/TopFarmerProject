using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Define;
using Random = UnityEngine.Random;

public class SeedController : ItemController
{
    public Seed Seed { get; private set; }
    private SeedData _seedData = null;

    public int currentGrowthDay;

    private SeedState _state = SeedState.None;
    public SeedState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;
        }
    }

    protected override void Init()
    {
        base.Init();

        SetItem();
    }

    protected void SetItem()
    {
        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(Item.TemplatedId, out itemData);
        _seedData = (SeedData)itemData;
        _sprite.sprite = Managers.Resource.Load<Sprite>($"{_seedData.seedSprite}");

        //currentGrowthDay = 3;
        //State = SeedState.None;
    }
  
    public void OnPlant()
    {
        Managers.Time.DayPassedRegistered -= OnDayPassed;
        Managers.Time.DayPassedRegistered += OnDayPassed;
        Seed = Item as Seed;
        currentGrowthDay = 0;
        State = SeedState.Progressing;


    }
    public void OnHarvest()
    {
        Debug.Log("OnHarvest");
        AddInven();

        GameObject land = Managers.Object.FindLandObject(CellPos);
        if (land.name == "Land_Plowed")
        {
            PlowedLandController pc = land.GetComponent<PlowedLandController>();
            if (pc == null)
                return;
            pc.IsUsing = false;
            SetItem();

        }

        Managers.Object.Remove(gameObject);
        Managers.Resource.Destroy(gameObject);
    }

    void HarvestReady()
    {
        State = SeedState.Completed;
        Managers.Time.DayPassedRegistered -= OnDayPassed;
    }
    public void OnDayPassed()
    {
        Debug.Log("OnDayPassed 이벤트 발생");
        currentGrowthDay++;
        
        if(_state == SeedState.Progressing)
            UpdateSprite();


        if (currentGrowthDay >= Seed.GrowthDay)
        {
            HarvestReady();
            Debug.Log("Grow Completed");

        }
    }

    void UpdateSprite()
    {
        if (currentGrowthDay == 1)
        {
            _sprite.sprite = Managers.Data.GetSpriteByName($"{_seedData.growthSprite1}");
        }
        else if (currentGrowthDay == Mathf.FloorToInt((Seed.GrowthDay / 2)))
        {
            _sprite.sprite = Managers.Data.GetSpriteByName($"{_seedData.growthSprite2}");
        }
        else if (currentGrowthDay >= Seed.GrowthDay)
        {
            _sprite.sprite = Managers.Data.GetSpriteByName($"{_seedData.growthSprite3}");
        }
    }

    void AddInven()
    {
        float dropRate = 0.2f;
        float randNum = Random.Range(0, 10f);
        int cropCount = _seedData.cropQuantity;
        if(randNum<= dropRate)
        {
            cropCount *= 2;
        }

        AddItemPacketReq packet = new AddItemPacketReq()
        {
            PlayerDbId = Managers.Object.PlayerInfo.PlayerDbId,
            TemplatedId = _seedData.resultCrop,
            Count = cropCount,
        };

        Managers.Web.SendPostRequest<AddItemPacketRes>("item/addItem", packet, (res) =>
        {
            Item item = Item.MakeItem(res.Item);
            Managers.Inven.Add(item);

            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            gameSceneUI.InvenUI.RefreshUI();
            gameSceneUI.ToolBarUI.RefreshUI();
        });

      

    }
}
