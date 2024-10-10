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
    public class WeaponItemLoader : ILoader<int, ItemData>
    {
        public List<WeaponData> array = new List<WeaponData>();
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
        public string prefabPath;
        public MonsterType monsterType;
        public int maxHp;
        public int damage;
        public int defense;
        public int searchRange;
        public int skillRange;
        public int speed;
        public int randomdurantionMovement;
        public int xp;
        public string dropItems;
        public Dictionary<int, float> dropTable;
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
                monster.dropTable = new Dictionary<int, float>();
                string[] dropParts = monster.dropItems.Split(' ');

                for (int i = 0; i < dropParts.Length; i += 2)
                {
                    int itemId;
                    float probability;

                    // 아이템 ID 변환 처리
                    if (!int.TryParse(dropParts[i], out itemId))
                    {
                        Debug.LogError($"Invalid item ID format at index {i}: {dropParts[i]}");
                        continue;  // 유효하지 않은 경우 건너뜀
                    }

                    // 확률 변환 처리
                    if (!float.TryParse(dropParts[i + 1], out probability))
                    {
                        Debug.LogError($"Invalid probability format at index {i + 1}: {dropParts[i + 1]}");
                        continue;  // 유효하지 않은 경우 건너뜀
                    }

                    // 드랍 테이블에 추가
                    monster.dropTable.Add(itemId, probability);
                }
                dict.Add(monster.monsterId, monster);
            }
            return dict;
        }
    }
   

    #endregion


}

