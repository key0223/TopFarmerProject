﻿using static Define;

namespace TopFarmerWebServer.Data
{
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

    public class FoodData : ItemData
    {
        public bool sellable;
        public bool eatable;
        public int time;
    }
    [Serializable]
    public class FoodItemLoader : ILoader<int, ItemData>
    {
        public List<FoodData> array = new List<FoodData>();
        public Dictionary<int, ItemData> MakeDict()
        {
            Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();

            foreach (ItemData food in array)
            {
                food.itemType = ItemType.ITEM_TYPE_FOOD;
                dict.Add(food.itemId, food);
            }
            return dict;
        }
    }

    #endregion

    #region Npc

    [Serializable]
    public class NpcData
    {
        public int npcId;
        public string name;
        public CreatureType creatureType;
        public string prefabPath;
        public NpcType npcType;
    }

    public class MerchantData : NpcData
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
                dict.Add(merchant.npcId, merchant);
            }
            return dict;
        }
    }



    #endregion

    #region Monster

    [Serializable]
    public class MonsterData
    {
        public ObjectType ObjectType;
        public int monsterId;
        public string name;
        public CreatureType creatureType;
        public string prefabPath;
        public MonsterType monsterType;
        public int level;
        public int maxHp;
        public int attack;
        public float speed;
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
                monster.ObjectType = ObjectType.OBJECT_TYPE_CREATURE;
                monster.creatureType = CreatureType.CREATURE_TYPE_MONSTER;
                dict.Add(monster.monsterId, monster);
            }
            return dict;
        }
    }



    #endregion
}
