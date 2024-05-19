using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Npc
{
    public int TemplatedId { get;  protected set; }
    public string Name { get; private set; }
    public ObjectType ObjectType { get; private set; }
    public CreatureType CreatureType { get; private set; }  
    public NpcType NpcType { get; private set; }
   
    public Npc(NpcType npcType)
    {
        NpcType = npcType;
    }

    public static Npc MakeNpc(int templatedId)
    {
        Npc npc = null;
        NpcData npcData = null;
        Managers.Data.NpcDict.TryGetValue(templatedId, out npcData);
        if (npcData == null) return null;

        switch(npcData.npcType)
        {
            case NpcType.NPC_TYPE_MERCHANT:
                npc = new Merchant(templatedId);
                break;
        }

        return npc;
    }
}
