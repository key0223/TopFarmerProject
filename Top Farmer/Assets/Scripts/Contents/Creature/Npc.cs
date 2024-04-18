using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class NpcInfo
{
    public int npcDbId;
    public int templatedId;
    public string name;

}
public class Npc 
{
    public NpcInfo Info { get; } = new NpcInfo();
    public NpcType NpcType { get;private set; }
    public int NpcDbId
    {
        get { return Info.npcDbId; }
        set { Info.npcDbId = value;}
    }
    public int TemplatedId
    {
        get { return Info.templatedId; }
        set { Info.templatedId = value; }
    }
    public string Name
    {
        get { return Info.name; }
        set { Info.name = value; }
    }


    public Npc(NpcType npcType)
    {
        NpcType = npcType;
    }

    public static Npc Init(NpcInfo  npcInfo)
    {
        Npc npc = null;
        NpcData npcData = null;

        Managers.Data.NpcDict.TryGetValue(npcInfo.templatedId, out npcData);
        if (npcData == null)
            return null;

        switch(npcData.npcType)
        {
            case NpcType.NPC_TYPE_MERCHANT:
                npc = new Merchant(npcInfo.templatedId);
                break;
        }

        if(npc !=null)
        {
            npc.NpcDbId = npcInfo.npcDbId;
        }
        return npc;

    }
}

public class Merchant: Npc
{
    public int DayOffPeriod { get; private set; }
    public int WorkingInPeriod { get;private set; }
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
            DayOffPeriod = data.dayOffPeriod;
            WorkingInPeriod = data.workingInPeriod;
            WorkingOutPeriod = data.workingOutPeriod;
        }
    }
}
