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
    #region Map
    [Serializable]
    public class PotalData
    {
        public string potalName;
        public string mapName;
        public string prefabPath;
        public float potalPosX;
        public float potalPosY;
        public float playerPosX;
        public float playerPosY;
        public string connectedMap;
        public string connectedPotal;
    }
    [Serializable]
    public class PotalLoader : ILoader<string, PotalData>
    {
        public List<PotalData> array = new List<PotalData>();

        public Dictionary<string, PotalData> MakeDict()
        {
            Dictionary<string, PotalData> dict = new Dictionary<string, PotalData>();

            foreach (PotalData str in array)
            {
                dict.Add(str.potalName, str);
            }
            return dict;
        }
    }
    #endregion
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

    #region Item
    [Serializable]
    public class RewardData
    {
        public int ownerId;
        public int itemId;
        public int probability;
        public int count;
    }
    [Serializable]
    public class RewardLoader : ILoader<int, List<RewardData>>
    {
        public List<RewardData> array = new List<RewardData>();
        public Dictionary<int, List<RewardData>> MakeDict()
        {
            Dictionary<int, List<RewardData>> dict = new Dictionary<int, List<RewardData>>();

            foreach (RewardData reward in array)
            {
                if (!dict.ContainsKey(reward.ownerId))
                {
                    dict[reward.ownerId] = new List<RewardData>();
                }

                dict[reward.ownerId].Add(reward);
            }
            return dict;
        }
    }
    [Serializable]
    public class ItemData
    {
        public int itemId;
        public string name;
        public int maxStack;
        public ItemType itemType;
        public string iconPath;
    }
    public class ToolData : ItemData
    {
        public ToolType toolType;
        public int range;
        public int durability;

    }

    [Serializable]
    public class ToolItemLoader : ILoader<int, ItemData>
    {
        public List<ToolData> array = new List<ToolData>();
        public Dictionary<int, ItemData> MakeDict()
        {
            Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();

            foreach (ItemData tool in array)
            {
                tool.itemType = ItemType.ITEM_TYPE_TOOL;
                dict.Add(tool.itemId, tool);
            }
            return dict;
        }
    }

    public class CropData : ItemData
    {
        public int sellingPrice;
    }
    [Serializable]
    public class CropItemLoader : ILoader<int, ItemData>
    {
        public List<CropData> array = new List<CropData>();
        public Dictionary<int, ItemData> MakeDict()
        {
            Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();

            foreach (ItemData crop in array)
            {
                crop.itemType = ItemType.ITEM_TYPE_CROP;
                dict.Add(crop.itemId, crop);
            }
            return dict;
        }
    }
    public class SeedData : ItemData
    {
        public int growthDay;
        public int purchasePrice;
        public int sellingPrice;
        public int resultCrop;
        public int cropQuantity;
        public string seedSprite;
        public string prefabPath;
        public string growthSprite1;
        public string growthSprite2;
        public string growthSprite3;

    }
    [Serializable]
    public class SeedItemLoader : ILoader<int, ItemData>
    {
        public List<SeedData> array = new List<SeedData>();
        public Dictionary<int, ItemData> MakeDict()
        {
            Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();

            foreach (ItemData seed in array)
            {
                seed.itemType = ItemType.ITEM_TYPE_SEED;
                dict.Add(seed.itemId, seed);
            }
            return dict;
        }
    }
    public class CraftingData : ItemData
    {
        public CraftingType craftingType;
        public bool sellable;
        public bool destroyable;
        public int sizeX;
        public int sizeY;
        public string prefabPath;
    }
    [Serializable]
    public class CraftableItemLoader : ILoader<int, ItemData>
    {
        public List<CraftingData> array = new List<CraftingData>();
        public Dictionary<int, ItemData> MakeDict()
        {
            Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();

            foreach (ItemData craftable in array)
            {
                craftable.itemType = ItemType.ITEM_TYPE_CRAFTING;
                dict.Add(craftable.itemId, craftable);
            }
            return dict;
        }
    }
    
    public class FoodData:ItemData
    {
        public bool sellable;
        public bool eatable;
        public int time;
        public int sellingPrice;
    }
    [Serializable]
    public class FoodItemLoader : ILoader<int, ItemData>
    {
        public List<FoodData> array = new List<FoodData>();
        public Dictionary <int, ItemData> MakeDict()
        {
            Dictionary<int,ItemData> dict = new Dictionary<int, ItemData>();

            foreach(ItemData food in array)
            {
                food.itemType = ItemType.ITEM_TYPE_FOOD;
                dict.Add(food.itemId, food);
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
            sprites = Resources.LoadAll<Sprite>($"Textures/Object/{atlasPath}");
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
        public ObjectType objectType;
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
                merchant.objectType = ObjectType.OBJECT_TYPE_CREATURE;
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
        public ObjectType objectType;
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
                monster.objectType = ObjectType.OBJECT_TYPE_CREATURE;
                monster.creatureType = CreatureType.CREATURE_TYPE_MONSTER;
                dict.Add(monster.monsterId, monster);
            }
            return dict;
        }
    }
   

    #endregion


    #region File System SavsFiles
    [Serializable]
    public class GameTime
    {
        public DayState dayState;
        public int dayOfSurvival;
        public float minute;
        public int hour;
        public int day;
        public int week;
        public int month;
        public int year;
    }
    [Serializable]
    public class GameData
    {
        public GameTime gameTime;
       
    }
    #endregion
}

