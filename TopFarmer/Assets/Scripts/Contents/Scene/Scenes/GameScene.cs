using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    UI_GameScene _sceneUI;
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Scene1_Farm;
      
        //Screen.SetResolution(640, 480, false);
        //_sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();

    }

    public override void Clear()
    {

    }
}
