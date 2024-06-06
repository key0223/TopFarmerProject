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
    public Dictionary<string, Data.PotalData> PotalDict { get; private set; } = new Dictionary<string, PotalData>();
    public Dictionary<string, List<Data.MapObjectData>> MapObjectDict { get; private set; } = new Dictionary<string, List<MapObjectData>>();
    public Dictionary<int, Data.ItemData> ItemDict { get; private set; } = new Dictionary<int, Data.ItemData>();
    public Dictionary<int, Data.NpcData> NpcDict { get; private set; } = new Dictionary<int, NpcData>();
    public Dictionary<int, Data.MonsterData> MonsterDict { get; private set; } = new Dictionary<int, MonsterData>();
    public Dictionary<string, Data.StringData> StringDict { get; private set; } = new Dictionary<string, Data.StringData>();
    public Dictionary<int, List<RewardData>> RewardDict { get; private set; } = new Dictionary<int, List<RewardData>>();
    public Dictionary<string, Sprite> SpriteDict { get; private set; } = new Dictionary<string, Sprite>();
    #endregion

    public void Init()
    {
        #region Map
        PotalDict = LoadJson<Data.PotalLoader, string, Data.PotalData>("MapData_Potal").MakeDict();

        List<Data.MapObjectData> farmObject = LoadJson<Data.MapObjectLoader,Data.MapObjectData>("MapObjectData_Farm").MakeList();
        List<string> keyList = KeyProvider<string>(farmObject[0].mapName);
        MapObjectDict = ListDict<string, List<Data.MapObjectData>>(keyList,farmObject);

        #endregion
        #region Item
        Dictionary<int, Data.ItemData> toolDict = LoadJson<Data.ToolItemLoader,int,Data.ItemData>("ItemData_Tool").MakeDict();
        Dictionary<int, Data.ItemData> cropDict = LoadJson<Data.CropItemLoader,int,Data.ItemData>("ItemData_Crop").MakeDict();
        Dictionary<int, Data.ItemData> seedDict = LoadJson<Data.SeedItemLoader,int,Data.ItemData>("ItemData_Seed").MakeDict();
        Dictionary<int, Data.ItemData> craftableDict = LoadJson<Data.CraftableItemLoader,int,Data.ItemData>("ItemData_Crafting").MakeDict();
        Dictionary<int, Data.ItemData> foodDict = LoadJson<Data.FoodItemLoader,int,Data.ItemData>("ItemData_Food").MakeDict();
        ItemDict = CombinedDict<int, Data.ItemData>(toolDict, cropDict, seedDict,craftableDict,foodDict);
        #endregion

        // Npc
        Dictionary<int, Data.NpcData> merchantDict = LoadJson<Data.MerchantNpcLoader, int, Data.NpcData>("NpcData_Merchant").MakeDict();
        NpcDict = CombinedDict<int, Data.NpcData>(merchantDict);

        // Monster
        Dictionary<int, Data.MonsterData> monsterDict = LoadJson<Data.MonsterLoader, int, Data.MonsterData>("MonsterData_Monster").MakeDict();
        MonsterDict = CombinedDict(monsterDict);

        #region String
        Dictionary<string, Data.StringData> npcStringDict = LoadJson<Data.StringLoader, string, Data.StringData>("StringData_Npc").MakeDict();
        Dictionary<string, Data.StringData> itemStringDict = LoadJson<Data.StringLoader, string, Data.StringData>("StringData_Item").MakeDict();
        StringDict = CombinedDict<string, Data.StringData>(npcStringDict, itemStringDict);
        #endregion

        Dictionary<int, List<RewardData>> monsterRewardDict = LoadJson<Data.RewardLoader, int, List<RewardData>>("RewardData_Monster").MakeDict();
        RewardDict = CombinedDict<int, List<RewardData>>(monsterRewardDict);

        #region Sprites
        //Dictionary<string, Sprite> cropSpriteDict = new Dictionary<string, Sprite>();
        //Sprite[] sprites = Resources.LoadAll<Sprite>("Textures/Object/Seed/Seeds");
        //foreach (Sprite sprite in sprites)
        //{
        //    CropSpriteDict.Add(sprite.name, sprite);
        //}
        Dictionary<string,Sprite> cropSpriteDict = new SpriteLoader("Seed/Seeds").MakeDict();
        Dictionary<string, Sprite> capmFireDict = new SpriteLoader("Craftable/CampFire").MakeDict();
        SpriteDict = CombinedDict<string, Sprite>(cropSpriteDict, capmFireDict);

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
    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
    Loader LoadJson<Loader,Value>(string path) where Loader : ILoader<Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }

    List<T> KeyProvider<T>(params T[] value)
    {
        List<T> key = new List<T>();
        foreach(var list  in value)
        {
            key.Add(list);
        }
        return key;
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

    Dictionary<TKey,TValue> ListDict<TKey,TValue>(List<TKey> keyList, params TValue[] lists)
    {
        Dictionary<TKey, TValue> listDict = new Dictionary<TKey, TValue>();

        for (int i = 0; i < keyList.Count; i++)
        {
            listDict[keyList[i]] = lists[i];
        }
        return listDict;
    }

}
