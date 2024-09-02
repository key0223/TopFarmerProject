using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{

    public UI_PauseMenu PauseMenuUI { get; private set; } 
    public UI_InventoryBar InvenBarUI { get; private set; }
    public UI_Clock ClockUI { get; private set; }
    public override void Init()
    {
        base.Init();
        PauseMenuUI = GetComponentInChildren<UI_PauseMenu>();
        InvenBarUI =  GetComponentInChildren<UI_InventoryBar>();
        ClockUI = GetComponentInChildren<UI_Clock>();
    }

   
}
