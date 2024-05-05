using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Define;

public class SaveLoadManager
{
   public GameData CurrentGameData {  get; private set; }

    public void Init()
    {
        LoadGameData();

        if (CurrentGameData == null)
        {
            DateTime initTime = new DateTime(DateTime.Now.Year, 2, 23);

            CurrentGameData = new GameData()
            {
                gameTime = new GameTime()
                {
                    dayState = DayState.Dawn,
                    dayOfSurvival = 1,
                    minute = 0,
                    hour = 6,
                    day = initTime.Day,
                    week = (int)initTime.DayOfWeek-1,
                    month = initTime.Month-1,
                    year = initTime.Year,
                },
                landList = new List<SaveLand>(),
                seedList = new List<SaveSeed>(),
            };
        }
    }

    public void SaveGameData()
    {

        GameTime saveTime = new GameTime()
        {
            dayState = Managers.Time.State,
            dayOfSurvival = Managers.Time.CurrentDayOfSurvival,
            minute = Managers.Time.CurrentMinute,
            hour= Managers.Time.CurrentHour,
            day = Managers.Time.CurrentDay,
            week = Managers.Time.CurrentDayOfWeek,
            month = Managers.Time.CurrentMonth,
            year = Managers.Time.CurrentYear,
        };
        GetSaveObject();
        CurrentGameData.gameTime = saveTime;

        Managers.Inven.UpdateInventoryDatabase();
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(CurrentGameData);
        File.WriteAllText(Application.persistentDataPath + "/gameData.json", json);
    }

    public void LoadGameData()
    {
        string dataPath = Application.persistentDataPath + "/gameData.json";
        if(File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            CurrentGameData = Newtonsoft.Json.JsonConvert.DeserializeObject<GameData>(json);
        }
    }

    public void GetSaveObject()
    {
        List<GameObject> currentObjects = Managers.Object.GetGameObjects();

        foreach (GameObject gameObject in currentObjects)
        {
            ItemController ic = gameObject.GetComponent<ItemController>();
            if (ic == null)
                continue;

            if (ic.ObjectType == ObjectType.OBJECT_TYPE_NONE)
            {
                PlowedLandController pc = (PlowedLandController)ic;

                SaveLand sl = new SaveLand()
                {
                    objectType = pc.ObjectType,
                    cellPos = pc.CellPos,
                    isUsing = pc.IsUsing,
                };

                CurrentGameData.landList.Add(sl);
            }
            else if(ic.ObjectType == ObjectType.OBJECT_TYPE_ITEM)
            {
                switch(ic.Item.ItemType)
                {
                    case ItemType.ITEM_TYPE_SEED:
                        {
                            SeedController sc = (SeedController)ic;

                            SaveSeed ss = new SaveSeed()
                            {
                                objectType = sc.ObjectType,
                                cellPos = sc.CellPos,
                                itemType = sc.Item.ItemType,
                                seedState = sc.State,
                                itemDbId = sc.Item.ItemDbId,
                                templatedId = sc.Item.TemplatedId,
                                currentGrowthDay = sc.currentGrowthDay,
                            };
                            CurrentGameData.seedList.Add(ss);
                        }
                        break;
                    case ItemType.ITEM_TYPE_CRAFTING:
                        break;

                }    
            }
        }
    }
}
