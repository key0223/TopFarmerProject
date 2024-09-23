using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseScene : BaseScene
{

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Scene3_House;

        if (Managers.Instance.IsFirstLoad)
        {
            StartCoroutine(CoFirstLoad());
        }
    }

    IEnumerator CoFirstLoad()
    {

        yield return new WaitForSeconds(0.1f);

        Managers.Save.LoadDataFromFile();
        Managers.Instance.IsFirstLoad = false;

    }

    public override void Clear()
    {

    }
}
