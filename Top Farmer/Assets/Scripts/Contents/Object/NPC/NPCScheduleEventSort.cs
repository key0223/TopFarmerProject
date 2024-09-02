using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScheduleEventSort : IComparer<NPCScheduleEvent>
{
  
    public int Compare(NPCScheduleEvent npcScheduleEvent1, NPCScheduleEvent npcScheduleEvent2)
    {
        if(npcScheduleEvent1?.Time == npcScheduleEvent2?.Time)
        {
            if(npcScheduleEvent1._priority < npcScheduleEvent2?._priority)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
        else if(npcScheduleEvent1?.Time> npcScheduleEvent2?.Time)
        {
            return 1;
        }
        else if(npcScheduleEvent1?.Time < npcScheduleEvent2?.Time)
        {
            return -1;
        }
        else
        {
            return 0;
        }    
    }
}
