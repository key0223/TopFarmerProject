using Assets.Scripts.Contents.Object;
using Data;
using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;


public class PlayerController : CreatureController
{
    public PlayerInfo Info { get; private set; }

    public StatInfo Stat
    {
        get { return Info.Stat; }
        set
        {
            if (Stat.Equals(value))
                return;

            Stat.hp = value.hp;
            Stat.maxHp = value.maxHp;
            Stat.speed = value.speed;
            UpdateHpBar();
        }
    }
    public int Hp
    {
        get { return Stat.hp; }
        set
        {
            Stat.hp = value;
            UpdateHpBar();
        }
    }
    Coroutine _coSkill;
    Coroutine _coUsingItem;
    Coroutine _coUseToolCooltime;

    bool _rangedSkill = false;

    public Item HoldingItem { get; private set; }

    public void SetPlayerInfo(PlayerInfo info)
    {
        Info= info;
        Stat = info.Stat;
        //UpdateHpBar();
    }
    protected override void Init()
    {
        base.Init();
        Texture2D tex = Managers.Resource.Load<Texture2D>("Textures/Icon/Cursor");
        UnityEngine.Cursor.SetCursor(tex, new Vector2(tex.width/5,tex.height/5), CursorMode.Auto);
    }
    protected override void UpdateAnimation()
    {
        if (_state == CreatureState.Idle)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_LEFT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_LEFT");
                    _sprite.flipX = true;
                    break;

            }
        }
        else if (_state == CreatureState.Moving)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_LEFT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_LEFT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.None:

                    break;
            }
        }
        else if (_state == CreatureState.Skill)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play(_rangedSkill ?"PICKUP_LEFT" :"ATTACK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play(_rangedSkill ? "PICKUP_LEFT" : "ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play(_rangedSkill ? "PICKUP_LEFT" : "ATTACK_LEFT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play(_rangedSkill ? "PICKUP_LEFT" : "ATTACK_LEFT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.None:

                    break;
            }
        }
        else
        {

        }
    }
    protected override void UpdateController()
    {
        //if (Managers.Time.State == DayState.Night)
        //    return;

        GetUIKeyInput();
        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
        }
        base.UpdateController();
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y,-10);
    }
    protected override void UpdateIdle()
    {
        //  �̵� ���·� ���� Ȯ��
        if(Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
        }

        // �׼� ���·� ���� Ȯ��
        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            _coSkill = StartCoroutine("CoStartPunch");
        }
        else if (_coUseToolCooltime == null && Input.GetKey(KeyCode.C)) // ���� ��� ���� ������ ��ġ
        {
            if (HoldingItem == null)
                return;

            UseItem(HoldingItem);
            _coUseToolCooltime = StartCoroutine("CoInputCoolTime", 0.2f);

        }
        else if (Input.GetKeyDown(KeyCode.X)) // ��Ȯ/��ȭ/ž��/��� �ֱ�
        {
            GameObject go = Managers.Map.Find((Vector2Int)GetFrontCellPos());
            if (go == null) return;

            ObjectController oc = go.GetComponent<ObjectController>();
            if(oc.ObjectType !=ObjectType.OBJECT_TYPE_CREATURE)
            {
                if (oc.ObjectType == ObjectType.OBJECT_TYPE_OBJECT)
                {
                    InteractableObject io = go.gameObject.GetComponent<InteractableObject>();
                    if (io == null) return;

                    Managers.InteractableObject.OnInteract(io);
                }
                else if (oc.ObjectType == ObjectType.OBJECT_TYPE_ITEM)
                {
                    ItemController ic = (ItemController)oc;

                    switch (ic.Item.ItemType)
                    {
                        case ItemType.ITEM_TYPE_TOOL:
                            break;
                        case ItemType.ITEM_TYPE_CROP:
                            break;
                        case ItemType.ITEM_TYPE_SEED:
                            SeedController sc = oc as SeedController;

                            if (sc.State == SeedState.Completed)
                            {
                                sc.OnHarvest();
                            }
                            break;
                        case ItemType.ITEM_TYPE_CRAFTING:
                            break;

                    }
                }
            }

            else if (oc.ObjectType == ObjectType.OBJECT_TYPE_CREATURE)
            {
                CreatureController cc = go.GetComponent<CreatureController>();
                if (cc == null) return;
                
                if(cc.CreatureType == CreatureType.CREATURE_TYPE_NPC)
                {
                    InteractableObject io = go.gameObject.GetComponent<InteractableObject>();
                    if (io == null) return;
                    Managers.InteractableObject.OnInteract(io);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            // Managers.Map.SaveMap("Assets/Resources/Map");
            Managers.SaveLoad.SaveGameData();
            Debug.Log("SaveFile saved");
        }

    }

    void UpdateHpBar()
    {
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_ToolBar toolbarUI = gameSceneUI.ToolBarUI;

        if (toolbarUI.HpBar == null)
            return;

        float ratio = 0.0f;
        if (Stat.maxHp > 0)
        {
            ratio = ((float)Hp / Stat.maxHp);
        }

        toolbarUI.HpBar.SetHpBar(ratio);
    }
    // Ű���� �Է� �̵��ϴ� ���⸸ ����
    void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Dir = MoveDir.Right;
        }
        else
        {
            Dir = MoveDir.None;
        }
    }
   
    void GetUIKeyInput()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            UI_Inventory invenUI = gameSceneUI.InvenUI;
            UI_ToolBar toolbalUI = gameSceneUI.ToolBarUI;
            UI_Merchant merchantUI = gameSceneUI.MerchantUI;

            if (merchantUI.gameObject.activeSelf)
                return;

            if (invenUI.gameObject.activeSelf)
            {
                invenUI.gameObject.SetActive(false);

            }
            else
            {
                invenUI.gameObject.SetActive(true);
                invenUI.State = InventoryState.Inventory;
            }
            invenUI.RefreshUI();
            toolbalUI.RefreshUI();
            merchantUI.RefreshUI();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {

        }
    }
    IEnumerator CoInputCoolTime(float time)
    {
        yield return new WaitForSeconds(time);
        _coUseToolCooltime = null;

    }

    public void RefreshHoldingItem(Item item)
    {
        HoldingItem = item;
    }

   
    public void UseItem(Item item)
    {
        
        if (item.ItemType == ItemType.ITEM_TYPE_TOOL)
        {
            Tool tool = (Tool)item;
            
            switch(tool.ToolType)
            {
                case ToolType.TOOL_TYPE_PICKAXE:
                    Debug.Log("Pickaxe");
                    break;
                case ToolType.TOOL_TYPE_AXE:
                    Debug.Log("Axe");

                    break;
                case ToolType.TOOL_TYPE_HOE:

                    bool interactable = Managers.Map.CanInteract(GetFrontCellPos());
                    if (interactable)
                    {
                        if(Managers.Object.FindLand(GetFrontCellPos()) == null)
                        {
                            State = CreatureState.UsingItem;
                            _coUsingItem = StartCoroutine("CoStartHoe");
                        }
                    }
                    break;
                case ToolType.TOOL_TYPE_WATERINGCAN:
                    Debug.Log("Watering Can");

                    break;
            }
        }
        else if (item.ItemType == ItemType.ITEM_TYPE_CROP)
        {
           
        }
        else if (item.ItemType == ItemType.ITEM_TYPE_SEED)
        {
            Seed seed = (Seed)item;

            if (Managers.Object.FindLand(GetFrontCellPos()) == null)
                return;

            GameObject go = Managers.Object.FindLand(GetFrontCellPos());
            PlowedLandController pc = go.GetComponent<PlowedLandController>();
            if (pc == null) return;

            if(!pc.IsUsing)
            {
                State = CreatureState.UsingItem;
                _coUsingItem = StartCoroutine("CoStartSeed", seed);
                pc.IsUsing = true;
            }
         
        }
        else if (item.ItemType == ItemType.ITEM_TYPE_CRAFTING)
        {
            Crafting crafting = (Crafting)item;
            bool interactable = Managers.Map.CanInteract(GetFrontCellPos());
            if (interactable)
            {
                if (Managers.Object.Find(GetFrontCellPos()) == null)
                {
                    State = CreatureState.UsingItem;
                    _coUsingItem = StartCoroutine("CoPlaceItem", crafting);
                }
            }
        }
    }

    #region Skill
    IEnumerator CoStartPunch()
    {
        // �ǰ� ����
        GameObject go = Managers.Object.FindMonster(GetFrontCellPos());
        if (go != null)
        {
            MonsterController mc = go.GetComponent<MonsterController>();

            if (mc == null) yield break;

            if(mc.Monster.MonsterType == MonsterType.MONSTER_TYPE_CONTACT)
            {

            }
            else if(mc.Monster.MonsterType == MonsterType.MONSTER_TYPE_RANGED)
            {
                MonsterRangedTypeController rangedC = (MonsterRangedTypeController)mc;
                rangedC.OnDamaged(Stat.attack);
            }
            else if (mc.Monster.MonsterType == MonsterType.MONSTER_TYPE_COUNTERATTACK)
            {
                MonsterConterAttackTypeController counterAttackC = (MonsterConterAttackTypeController)mc;
                counterAttackC.OnDamaged(Stat.attack);
            }
            //if (mc != null)
            //    mc.OnDamaged(Stat.attack);
        }
        // ��� �ð�
        _rangedSkill = false;
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        _coSkill = null;
    }

    IEnumerator CoStartPickup()
    {
        _rangedSkill = true;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
    }
    #endregion

    #region UsingItem

    IEnumerator CoStartHoe()
    {
      
        GameObject plowed = Managers.Resource.Instantiate($"Object/Land/Land_Plowed");
        plowed.name = "Land_Plowed";
        PlowedLandController pc = plowed.GetComponent<PlowedLandController>();
        pc.ObjectType = ObjectType.OBJECT_TYPE_ITEM;
        //pc.ObjectType = ObjectType.OBJECT_TYPE_NONE;
        pc.CellPos = GetFrontCellPos();
        pc.IsUsing = false;
        //Managers.Map.InitPos(plowed, (Vector2Int)pc.CellPos);
        Managers.Object.Add(plowed);

        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coUsingItem = null;
    }
    IEnumerator CoStartSeed(Item item)
    {
        if(item.Count > 0 )
        {
            Data.ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(HoldingItem.TemplatedId, out itemData);
            SeedData seedData = (SeedData)itemData;
            GameObject seed = Managers.Resource.Instantiate($"{seedData.prefabPath}");
            SeedController sc = seed.GetComponent<SeedController>();
            sc.Item = item;
            sc.ObjectType = ObjectType.OBJECT_TYPE_ITEM;
            sc.CellPos = GetFrontCellPos();

            Managers.Object.Add(seed);
            sc.OnPlant();

            item.Count--;

            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            UI_Inventory invenUI = gameSceneUI.InvenUI;
            UI_ToolBar toolbarUI = gameSceneUI.ToolBarUI;

            invenUI.RefreshUI();
            toolbarUI.RefreshUI();

            Managers.Inven.UpdateInventoryDatabase();
        }

        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coUsingItem = null;
    }

    IEnumerator CoPlaceItem(Item item)
    {
        if (item.Count > 0)
        {
            Data.ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(HoldingItem.TemplatedId, out itemData);
            CraftingData craftingData = (CraftingData)itemData;
            GameObject crafting = Managers.Resource.Instantiate($"{craftingData.prefabPath}");
            CampfireController cc = crafting.GetComponent<CampfireController>();
            cc.Item = item;
            cc.CellPos = GetFrontCellPos();

            Managers.Object.Add(crafting);

            item.Count--;

            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            UI_Inventory invenUI = gameSceneUI.InvenUI;
            UI_ToolBar toolbarUI = gameSceneUI.ToolBarUI;

            invenUI.RefreshUI();
            toolbarUI.RefreshUI();

            Managers.Inven.UpdateInventoryDatabase();
        }

        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coUsingItem = null;
    }
    #endregion

    public override void OnDamaged(int damage)
    {
        base.OnDamaged(damage);

        Hp = Mathf.Max(Stat.hp - damage, 0);
    }
    
}
