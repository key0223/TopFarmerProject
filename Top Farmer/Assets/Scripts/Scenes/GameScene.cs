using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    UI_GameScene _sceneUI;
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Managers.Map.LoadMap(1);

        #region Temp
        GameObject player = Managers.Resource.Instantiate("Creature/Player");
        player.name = "Player";
        Managers.Object.Add(player,player:true);

        // Npc
        NpcData npcData = null;
        Managers.Data.NpcDict.TryGetValue(1, out npcData);
        NpcInfo npcInfo = new NpcInfo()
        {
            templatedId = npcData.npcId,
            name = npcData.name,
        };
        Npc merchant1 = Npc.Init(npcInfo);

        GameObject merchantGo = Managers.Resource.Instantiate($"{npcData.prefabPath}");
        merchantGo.name = "Merchant";

        MerchantController mc1 = merchantGo.GetComponent<MerchantController>();
        mc1.ObjectType = ObjectType.OBJECT_TYPE_INTERACTABLE_OBJECT;
        mc1.Npc = merchant1;

        Managers.Object.Add(merchantGo);
        Managers.Npc.Init();


        GameObject chest = Managers.Resource.Instantiate("Object/Craftable/Interactable/Chest");
        chest.name = "Chest";
        Managers.Object.Add(chest);

        // Oven
        GameObject oven = Managers.Resource.Instantiate("Object/Craftable/Interactable/Oven");
        OvenController oc = oven.GetComponent<OvenController>();
        Managers.Object.Add(oven);


        GameObject fire = Managers.Resource.Instantiate("Object/Craftable/Campfire");
        CampfireController cc = oven.GetComponent<CampfireController>();
        Managers.Object.Add(fire);



        #endregion
        //Screen.SetResolution(640, 480, false);
        _sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();

    }

    public override void Clear()
    {

    }
   
}
