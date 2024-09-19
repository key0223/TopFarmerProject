using Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}
public interface ILoader<Value>
{
    List<Value> MakeList();
}

public class DataManager
{
    #region Dict

    public Dictionary<int, QuestData> QuestDict { get; private set; } = new Dictionary<int, QuestData>();
    public Dictionary<int, ObjectiveData> ObjectiveDict { get; private set; } = new Dictionary<int, ObjectiveData>();

    public Dictionary<int, ItemData> ItemDict { get; private set; } = new Dictionary<int, ItemData>();
    public Dictionary<int, CropData> CropDict { get; private set; } = new Dictionary<int, CropData>();
    public Dictionary<int, ShopItemData> ShopItemDict { get; private set; }= new Dictionary<int, ShopItemData>();
    public Dictionary<int, Data.NpcData> NpcDict { get; private set; } = new Dictionary<int, NpcData>();
    public Dictionary<int, Data.MonsterData> MonsterDict { get; private set; } = new Dictionary<int, MonsterData>();
    public Dictionary<string, Data.StringData> StringDict { get; private set; } = new Dictionary<string, Data.StringData>();
    public Dictionary<string, Sprite> SpriteDict { get; private set; } = new Dictionary<string, Sprite>();
    #endregion

    public void Init()
    {

        Dictionary<int,ItemData> comodityDict = LoadJson<Data.ItemLoader, int,ItemData>("ItemData_Comodity").MakeDict();
        Dictionary<int,ItemData> reapableDict = LoadJson<Data.ItemLoader, int,ItemData>("ItemData_Reapable").MakeDict();
        Dictionary<int,ItemData> seedDict = LoadJson<Data.ItemLoader, int,ItemData>("ItemData_Seed").MakeDict();
        Dictionary<int,ItemData> toolDict = LoadJson<Data.ItemLoader, int,ItemData>("ItemData_Tool").MakeDict();
        ItemDict = CombinedDict<int, ItemData>(comodityDict, reapableDict, seedDict,toolDict);

        Dictionary<int, CropData> cropDict = LoadJson<Data.CropLoader, int, CropData>("ItemData_Crop").MakeDict();
        CropDict = CombinedDict<int, CropData>(cropDict);

        Dictionary<int,ShopItemData> seedShopDict = LoadJson<Data.ShopItemLoader, int, ShopItemData>("ShopData_SeedShop").MakeDict();
        ShopItemDict = CombinedDict<int, ShopItemData>(seedShopDict);

        // Npc
        Dictionary<int, Data.NpcData> merchantDict = LoadJson<Data.MerchantNpcLoader, int, Data.NpcData>("NpcData_Merchant").MakeDict();
        NpcDict = CombinedDict<int, Data.NpcData>(merchantDict);

        // Monster
        Dictionary<int, Data.MonsterData> monsterDict = LoadJson<Data.MonsterLoader, int, Data.MonsterData>("MonsterData_Monster").MakeDict();
        MonsterDict = CombinedDict(monsterDict);

        #region String
        Dictionary<string, Data.StringData> npcStringDict = LoadJson<Data.StringLoader, string, Data.StringData>("StringData_Npc").MakeDict();
        Dictionary<string, Data.StringData> itemStringDict = LoadJson<Data.StringLoader, string, Data.StringData>("StringData_Item").MakeDict();
        Dictionary<string, Data.StringData> questStringDict = LoadJson<Data.StringLoader, string, Data.StringData>("StringData_Quest").MakeDict();
        Dictionary<string, Data.StringData> dialogueStringDict = LoadJson<Data.StringLoader, string, Data.StringData>("StringData_Dialogue").MakeDict();
        StringDict = CombinedDict<string, Data.StringData>(npcStringDict, itemStringDict,questStringDict,dialogueStringDict);
        #endregion

        #region Sprites
        //Dictionary<string, Sprite> cropSpriteDict = new Dictionary<string, Sprite>();
        //Sprite[] sprites = Resources.LoadAll<Sprite>("Textures/Object/Seed/Seeds");
        //foreach (Sprite sprite in sprites)
        //{
        //    CropSpriteDict.Add(sprite.name, sprite);
        //}
        Dictionary<string,Sprite> objectSpriteDict = new SpriteLoader("Object/Objects").MakeDict();
        Dictionary<string,Sprite> grassSpriteDict = new SpriteLoader("Object/Scenary/Grass").MakeDict();
        Dictionary<string, Sprite> toolSpriteDict = new SpriteLoader("Object/Tool/Tool_Icons").MakeDict();
        Dictionary<string, Sprite> weaponSpriteDict = new SpriteLoader("Object/Weapon/Weapons").MakeDict();
        Dictionary<string, Sprite> timeUISpriteDict = new SpriteLoader("UI/TimeUI").MakeDict();
        Dictionary<string, Sprite> cropSpriteDict = new SpriteLoader("Object/Crops").MakeDict();
        Dictionary<string, Sprite> oakSpriteDict = new SpriteLoader("Object/Scenary/Spring/OakTree_spring").MakeDict();
        Dictionary<string, Sprite> mapleSpriteDict = new SpriteLoader("Object/Scenary/Spring/MapleTree_spring").MakeDict();
        Dictionary<string, Sprite> pineSpriteDict = new SpriteLoader("Object/Scenary/Spring/PineTree_spring").MakeDict();



        SpriteDict = CombinedDict<string, Sprite>(objectSpriteDict,grassSpriteDict, toolSpriteDict,weaponSpriteDict,timeUISpriteDict,cropSpriteDict,
                                                  oakSpriteDict,mapleSpriteDict,pineSpriteDict);

        #endregion

        #region Quest
        Dictionary<int, QuestData> basicQuestDict = LoadJson<Data.QuestLoader, int, QuestData>("QuestData_Basic").MakeDict();
        Dictionary<int, ItemDeliveryQuestData> itemDeliveryQuestDict = LoadJson<Data.ItemDeliveryQuestLoader, int, ItemDeliveryQuestData>("QuestData_ItemDelivery").MakeDict();
        Dictionary<int, MonsterQuestData> monsterQuestDict = LoadJson<Data.MonsterQuestLoader, int, MonsterQuestData>("QuestData_Monster").MakeDict();
        Dictionary<int, SocializeQuestData> socializeQuestDict = LoadJson<Data.SocializeQuestLoader, int, SocializeQuestData>("QuestData_Socialize").MakeDict();
        Dictionary<int, LocationQuestData> locationQuestDict = LoadJson<Data.LocationQuestLoader, int, LocationQuestData>("QuestData_Location").MakeDict();
        Dictionary<int, HarvestQuestData> harvestQuestDict = LoadJson<Data.HarvestQuestLoader, int, HarvestQuestData>("QuestData_Harvest").MakeDict();

        MergeDictionaries(basicQuestDict, QuestDict);
        MergeDictionaries(itemDeliveryQuestDict, QuestDict);
        MergeDictionaries(monsterQuestDict, QuestDict);
        MergeDictionaries(socializeQuestDict, QuestDict);
        MergeDictionaries(locationQuestDict, QuestDict);
        MergeDictionaries(harvestQuestDict, QuestDict);
        #endregion
    }
    public Sprite GetSpriteByName(string name)
    {
        if(SpriteDict.ContainsKey(name))
        {
            return SpriteDict[name];
        }
        else
        {
            Debug.Log("Sprite not found" + name);
            return null;
        }
    }

    public ItemData GetItemData(int itemId)
    {
        ItemData itemData = null;
        if (ItemDict.TryGetValue(itemId, out itemData))
        {
            return itemData;
        }
        else
        {
            return null;
        }
    }
    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }

    Dictionary<TKey,TValue> CombinedDict<TKey,TValue> (params Dictionary<TKey,TValue>[] dictionaries)
    {
        Dictionary<TKey, TValue> combinedDict = new Dictionary<TKey, TValue>();

        foreach (var dict in dictionaries)
        {
            foreach (var kvp in dict)
            {
                combinedDict[kvp.Key] = kvp.Value;
            }
        }

        return combinedDict;
    }

    void MergeDictionaries<TKey, TValue>(Dictionary<TKey, TValue> source, Dictionary<TKey, QuestData> destination) where TValue : QuestData
    {
        foreach (var kvp in source)
        {
            if (!destination.ContainsKey(kvp.Key))
            {
                destination.Add(kvp.Key, kvp.Value);
            }
            else
            {
                destination[kvp.Key] = kvp.Value; // µ¤¾î¾²±â
            }
        }
    }


}
