using Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}
public class DataManager
{
    #region ItemDict
    public Dictionary<int, Data.ItemData> ItemDict { get; private set; } = new Dictionary<int, Data.ItemData>();
    public Dictionary<int, Data.NpcData> NpcDict { get; private set; } = new Dictionary<int, NpcData>();
    public Dictionary<string, Data.StringData> StringDict { get; private set; } = new Dictionary<string, Data.StringData>();
    public Dictionary<string, Sprite> CropSpriteDict { get; private set; } = new Dictionary<string, Sprite>();
    #endregion

    public void Init()
    {
        //SkillDict = LoadJson<Data.SkillData, int, Data.Skill>("SkillData").MakeDict();
        //ItemDict = LoadJson<Data.ItemLoader, int, Data.ItemData>("ItemData").MakeDict();
        //MonsterDict = LoadJson<Data.MonsterLoader, int, Data.MonsterData>("MonsterData").MakeDict();

        //ItemDict = LoadJson<Data.ItemLoader,int,Data.ItemData>("ItemData").MakeDict();

        #region Item
        Dictionary<int, Data.ItemData> toolDict = LoadJson<Data.ToolItemLoader,int,Data.ItemData>("ItemData_Tool").MakeDict();
        Dictionary<int, Data.ItemData> cropDict = LoadJson<Data.CropItemLoader,int,Data.ItemData>("ItemData_Crop").MakeDict();
        Dictionary<int, Data.ItemData> seedDict = LoadJson<Data.SeedItemLoader,int,Data.ItemData>("ItemData_Seed").MakeDict();
        Dictionary<int, Data.ItemData> craftableDict = LoadJson<Data.CraftableItemLoader,int,Data.ItemData>("ItemData_Craftable").MakeDict();
        Dictionary<int, Data.ItemData> foodDict = LoadJson<Data.FoodItemLoader,int,Data.ItemData>("ItemData_Food").MakeDict();
        //Dictionary<int, Data.ItemData> modernDict = LoadJson<Data.ModernItemLoader,int,Data.ItemData>("ItemData_Modern").MakeDict();
        ItemDict = CombinedDict<int, Data.ItemData>(toolDict, cropDict, seedDict,craftableDict,foodDict);
        #endregion


        Dictionary<int, Data.NpcData> merchantDict = LoadJson<Data.MerchantNpcLoader, int, Data.NpcData>("NpcData_Merchant").MakeDict();
        NpcDict = CombinedDict<int, Data.NpcData>(merchantDict);

        #region String
        Dictionary<string, Data.StringData> npcStringDict = LoadJson<Data.StringLoader, string, Data.StringData>("StringData_Npc").MakeDict();
        Dictionary<string, Data.StringData> itemStringDict = LoadJson<Data.StringLoader, string, Data.StringData>("StringData_Item").MakeDict();
        StringDict = CombinedDict<string, Data.StringData>(npcStringDict, itemStringDict);
        #endregion

        #region Sprites
        Sprite[] sprites = Resources.LoadAll<Sprite>("Textures/Seed/Seeds");
        foreach (Sprite sprite in sprites)
        {
            CropSpriteDict.Add(sprite.name, sprite);
        }

        #endregion

    }
    public Sprite GetSpriteByName(string name)
    {
        if(CropSpriteDict.ContainsKey(name))
        {
            return CropSpriteDict[name];
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


}
