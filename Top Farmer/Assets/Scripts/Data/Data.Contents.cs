using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using static Define;

namespace Data
{
    [Serializable]
    public class ItemLoader : ILoader<int, ItemData>
    {
        public List<ItemData> array = new List<ItemData>();
        public Dictionary<int, ItemData> MakeDict()
        {
            Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();

            foreach (ItemData item in array)
            {
                dict.Add(item.itemId, item);
            }
            return dict;
        }
    }

    [Serializable]
    public class CropLoader : ILoader<int, CropData>
    {
        public List<CropData> array = new List<CropData>();
        public Dictionary<int, CropData> MakeDict()
        {
            Dictionary<int, CropData> dict = new Dictionary<int, CropData>();

            foreach (CropData item in array)
            {
                dict.Add(item.seedId, item);
            }
            return dict;
        }

    }

    [Serializable]
    public class ShopItemLoader : ILoader<int, ShopItemData>
    {
        public List<ShopItemData> array = new List<ShopItemData>();

        public Dictionary<int,ShopItemData> MakeDict()
        {
            Dictionary<int, ShopItemData> dict = new Dictionary<int, ShopItemData>();
            foreach (ShopItemData item in array)
            {
                dict.Add(item.itemId, item);
            }
            return dict;
        }
       
    }
    #region String
    [Serializable]
    public class StringData
    {
        public string stringId;
        public string ko;
        public string en;
    }

    [Serializable]
    public class StringLoader : ILoader<string, StringData>
    {
        public List<StringData> array = new List<StringData>();

        public Dictionary<string, StringData> MakeDict()
        {
            Dictionary<string, StringData> dict = new Dictionary<string, StringData>();

            foreach (StringData str in array)
            {
                dict.Add(str.stringId, str);
            }
            return dict;
        }
    }
    #endregion

    [Serializable]
    public class SpriteLoader:ILoader<string, Sprite>
    {
        Sprite[] sprites;
        
        public SpriteLoader(string atlasPath)
        {
            sprites = Resources.LoadAll<Sprite>($"{ObjectSpritePath}{atlasPath}");
        }

        public Dictionary<string, Sprite> MakeDict()
        {
            Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();
            foreach(Sprite sprite in sprites)
            {
                spriteDict.Add(sprite.name, sprite);
            }

            return spriteDict;
        }

    }


    #region Creature


    [Serializable]
    public class NpcData 
    {
        public int npcId;
        public string name;
        public CreatureType creatureType;
        public string prefabPath;
        public NpcType npcType;
    }
    
    public class MerchantData  : NpcData
    {
        public int dayOffPeriod;
        public int workingInPeriod;
        public int workingOutPeriod;
    }

    [Serializable]
    public class MerchantNpcLoader : ILoader<int, NpcData>
    {
        public List<MerchantData> array = new List<MerchantData>();
        public Dictionary<int, NpcData> MakeDict()
        {
            Dictionary<int, NpcData> dict = new Dictionary<int, NpcData>();

            foreach (MerchantData merchant in array)
            {
                merchant.creatureType = CreatureType.CREATURE_TYPE_NPC;
                merchant.npcType = NpcType.NPC_TYPE_MERCHANT;

                dict.Add(merchant.npcId, merchant);
            }
            return dict;
        }
    }

    [Serializable]
    public class MonsterData
    {
        public int monsterId;
        public string name;
        public CreatureType creatureType;
        public string prefabPath;
        public MonsterType monsterType;
        public int level;
        public int maxHp;
        public int attack;
        public float speed;
        public int skillRange;
        public int searchRange;
        public int totalExp;
    }

    [Serializable]
    public class MonsterLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> array = new List<MonsterData>();

        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData monster in array)
            {
                monster.creatureType = CreatureType.CREATURE_TYPE_MONSTER;
                dict.Add(monster.monsterId, monster);
            }
            return dict;
        }
    }
   

    #endregion


}

