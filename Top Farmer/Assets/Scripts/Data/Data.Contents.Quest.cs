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
                if (!string.IsNullOrEmpty(item.questObjectiveRaw))
                {
                    item.questObjectiveIds = item.questObjectiveRaw.Split(',').Select(t => int.Parse( t.Trim())).ToArray();
                }

                if(!string.IsNullOrEmpty(item.questDeliverTimeRaw))
                {
                    string[] parts = item.questDeliverTimeRaw.Split(",");

                    int year = int.Parse(parts[0]);
                    Season season = (Season)Enum.Parse(typeof(Season), parts[1]);
                    int day = int.Parse(parts[2]);

                    item.questDeliverYear = year;
                    item.questDeliverSeason = season;
                    item.questDeliverDay = day;
                }
                dict.Add(item.questId, item);
            }

            return dict;
        }
    }
    [Serializable]
    public class ObjectiveLoader : ILoader<int,ObjectiveData>
    {
        public List<ObjectiveData> array = new List<ObjectiveData>();
        public Dictionary<int,ObjectiveData>MakeDict()
        {
            Dictionary<int,ObjectiveData> dict = new Dictionary<int, ObjectiveData> ();

            foreach(ObjectiveData item in array)
            {
                if(!string.IsNullOrEmpty(item.targetsRaw))
                {
                    item.targets = item.targetsRaw.Split(',').Select(t=>t.Trim()).ToArray();
                }
                dict.Add(item.objectiveId, item);
            }

            return dict;
        }
    }

   

  
}
