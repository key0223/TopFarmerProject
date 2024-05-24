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
        PlayerController pc = player.GetComponent<PlayerController>();
        pc.ObjectType = ObjectType.OBJECT_TYPE_PLAYER;

        Managers.Object.Add(player,player:true);

        // Npc
        NpcData npcData = null;
        Managers.Data.NpcDict.TryGetValue(601, out npcData);
       
        Npc merchant1 = Npc.MakeNpc(601);

        GameObject merchantGo = Managers.Resource.Instantiate($"{npcData.prefabPath}");
        merchantGo.name = "Merchant";

        MerchantController mc1 = merchantGo.GetComponent<MerchantController>();
        mc1.ObjectType = npcData.objectType;
        mc1.Npc = merchant1;

        Managers.Object.Add(merchantGo);
        Managers.Npc.Init();

        // Oven
        GameObject oven = Managers.Resource.Instantiate("Object/Craftable/Interactable/Oven");
        OvenController oc = oven.GetComponent<OvenController>();
        oc.ObjectType = ObjectType.OBJECT_TYPE_OBJECT;
        Managers.Object.Add(oven);


        int randCount = Random.Range(0, 6);
        int monsterId = 7;
        int randType = Random.Range(0, 5);

        int randTemplatedId = monsterId * 100 + randType * 10 + 1;

        for (int i = 0; i < 5; i++)
        {
            int randSpawnPosX = Random.Range(Managers.Object.Player.CellPos.x - 10, Managers.Object.Player.CellPos.x + 10);
            int randSpawnPosY = Random.Range(Managers.Object.Player.CellPos.y - 10, Managers.Object.Player.CellPos.y + 10);
            Vector3Int pos = new Vector3Int(randSpawnPosX, randSpawnPosY);

            if (Managers.Map.Find((Vector2Int)pos) == null && Managers.Map.CanGo(pos))
            {
                Data.MonsterData monsterData = null;
                Managers.Data.MonsterDict.TryGetValue(731, out monsterData);


                GameObject monsterGo = Managers.Resource.Instantiate($"{monsterData.prefabPath}");
                MaggotController mc = monsterGo.GetComponent<MaggotController>();
                mc.CellPos = new Vector3Int(randSpawnPosX, randSpawnPosY, 0);
            }
        }

        //if(Managers.Object.Find(randSpawnPos) == null )
        //{
        //    gameObject.GetComponent<ObjectController>().CellPos = (Vector3Int)randSpawnPos;
        //    Managers.Object.Add(gameObject);

        //}

        //GameObject fire = Managers.Resource.Instantiate("Object/Craftable/Campfire");
        //CampfireController cc = fire.GetComponent<CampfireController>();
        //Managers.Object.Add(fire);



        #endregion
        //Screen.SetResolution(640, 480, false);
        _sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();

    }

    public override void Clear()
    {

    }
   
}
