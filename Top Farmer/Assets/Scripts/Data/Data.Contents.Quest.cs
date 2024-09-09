using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

namespace Data
{
    [Serializable]
    public class QuestLoader : ILoader<int, QuestData>
    {
        public List<QuestData> array = new List<QuestData>();
        public Dictionary<int, QuestData> MakeDict()
        {
            Dictionary<int, QuestData> dict = new Dictionary<int, QuestData>();

            foreach (QuestData item in array)
            {
                dict.Add(item.questId, item);
            }

            return dict;
        }
    }

    [Serializable]
    public class ItemDeliveryQuestLoader : ILoader<int, ItemDeliveryQuestData>
    {
        public List<ItemDeliveryQuestData> array = new List<ItemDeliveryQuestData>();
        public Dictionary<int, ItemDeliveryQuestData> MakeDict()
        {
            Dictionary<int, ItemDeliveryQuestData> dict = new Dictionary<int, ItemDeliveryQuestData>();

            foreach (ItemDeliveryQuestData item in array)
            {
                if (!string.IsNullOrEmpty(item.target))
                {
                    string[] targetArray = item.target.Split(' ');

                    item.targetName = targetArray[0];
                    item.targetItemId = Convert.ToInt32( targetArray[1]);

                    if(targetArray.Length > 2 )
                    {
                        item.targetQuantity = Convert.ToInt32(targetArray[2]);
                    }
                    else
                    {
                        item.targetQuantity = 1;
                    }
                }
                dict.Add(item.questId, item);
            }
            return dict;
        }
    }
    [Serializable]
    public class MonsterQuestLoader : ILoader<int, MonsterQuestData>
    {
        public List<MonsterQuestData> array = new List<MonsterQuestData>();
        public Dictionary<int, MonsterQuestData> MakeDict()
        {
            Dictionary<int, MonsterQuestData> dict = new Dictionary<int, MonsterQuestData>();

            foreach (MonsterQuestData item in array)
            {
                if (!string.IsNullOrEmpty(item.target))
                {
                    string[] targetArray = item.target.Split(' ');

                    item.targetName = targetArray[0];
                    item.targetQuantity = Convert.ToInt32(targetArray[1]);
                }
                dict.Add(item.questId, item);
            }
            return dict;
        }
    }

    [Serializable]
    public class SocializeQuestLoader : ILoader<int, SocializeQuestData>
    {
        public List<SocializeQuestData> array = new List<SocializeQuestData>();
        public Dictionary<int, SocializeQuestData> MakeDict()
        {
            Dictionary<int, SocializeQuestData> dict = new Dictionary<int, SocializeQuestData>();

            foreach (SocializeQuestData item in array)
            {
                if (!string.IsNullOrEmpty(item.target))
                {
                    string[] targetArray = item.target.Split(' ');

                    item.targetName = targetArray[0];
                }
                dict.Add(item.questId, item);
            }
            return dict;
        }
    }

    [Serializable]
    public class LocationQuestLoader : ILoader<int, LocationQuestData>
    {
        public List<LocationQuestData> array = new List<LocationQuestData>();
        public Dictionary<int, LocationQuestData> MakeDict()
        {
            Dictionary<int, LocationQuestData> dict = new Dictionary<int, LocationQuestData>();

            foreach (LocationQuestData item in array)
            {
                if (!string.IsNullOrEmpty(item.target))
                {
                    string[] targetArray = item.target.Split(' ');

                    item.targetPlace = targetArray[0];
                }
                dict.Add(item.questId, item);
            }
            return dict;
        }
    }

    [Serializable]
    public class HarvestQuestLoader : ILoader<int, HarvestQuestData>
    {
        public List<HarvestQuestData> array = new List<HarvestQuestData>();
        public Dictionary<int, HarvestQuestData> MakeDict()
        {
            Dictionary<int, HarvestQuestData> dict = new Dictionary<int, HarvestQuestData>();

            foreach (HarvestQuestData item in array)
            {
                if (!string.IsNullOrEmpty(item.target))
                {
                    string[] targetArray = item.target.Split(' ');

                    item.targetItemId = Convert.ToInt32(targetArray[0]);


                    if (targetArray.Length > 1)
                    {
                        item.targetQuantity = Convert.ToInt32(targetArray[1]);
                    }
                    else
                    {
                        item.targetQuantity = 1;
                    }
                    
                }
                dict.Add(item.questId, item);
            }
            return dict;
        }
    }

}
