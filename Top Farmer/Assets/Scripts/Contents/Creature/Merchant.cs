using Data;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static Define;

public class Merchant : Npc
{
    public int DayOffPeriod { get; private set; }
    public int WorkingInPeriod { get; private set; }
    public int WorkingOutPeriod { get; private set; }

    public Merchant(int templatedId) : base(NpcType.NPC_TYPE_MERCHANT)
    {
        Init(templatedId);
    }

    void Init(int templatedId)
    {
        NpcData npcData = null;
        Managers.Data.NpcDict.TryGetValue(templatedId, out npcData);
        if (npcData.npcType != NpcType.NPC_TYPE_MERCHANT)
            return;

        MerchantData data = (MerchantData)npcData;
        {
            TemplatedId = data.npcId;
            Name = data.name;
            ObjectType = data.objectType;
            CreatureType = data.creatureType;
            DayOffPeriod = data.dayOffPeriod;
            WorkingInPeriod = data.workingInPeriod;
            WorkingOutPeriod = data.workingOutPeriod;
        }
    }
}
