using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_Oven : UI_Base
{
  enum GameObjects
    {
        Stove,
        Stove1,
        Stove2,
    }
    public UI_Stove[] Stoves { get; private set; }

    public override void Init()
    {

        Bind<GameObject>(typeof(GameObjects));

        int numStoves = Enum.GetNames(typeof(GameObjects)).Length;
        Stoves = new UI_Stove[numStoves];

        for (int i = 0; i < numStoves; i++)
        {
            Stoves[i] = GetObject(i).GetComponent<UI_Stove>();
        }

    }
 

    public int? GetEmptyStove()
    {
        for(int i = 0;i < Stoves.Length;i++)
        {
            if (Stoves[i].State == StoveState.Empty)
                return i;
        }
        return null;
    }

  

    //public InteractItem Get(int itemDbId)
    //{
    //    InteractItem item;
    //    Items.TryGetValue(itemDbId, out item);

    //    return item;
    //}

}
