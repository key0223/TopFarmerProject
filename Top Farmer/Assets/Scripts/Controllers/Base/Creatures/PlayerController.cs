using Assets.Scripts.Contents.Object;
using Data;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;


public class PlayerController : CreatureController
{
    public PlayerInfo Info { get; private set; }
    Coroutine _coSkill;
    Coroutine _coUsingItem;
    Coroutine _coUseToolCooltime;

    bool _rangedSkill = false;

    public Item HoldingItem { get; private set; }

    public void SetPlayerInfo(PlayerInfo info)
    {
        Info= info;
    }
    protected override void Init()
    {
        base.Init();
        Texture2D tex = Managers.Resource.Load<Texture2D>("Textures/Cursor/Cursor");
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
        if (Managers.Time.State == DayState.Night)
            return;

        GetUIKeyInput();
        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                GetIdleInput();
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
   
    // 키보드 입력 이동하는 방향만 설정
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

    void GetIdleInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            _coSkill = StartCoroutine("CoStartPunch");
        }
        else if(_coUseToolCooltime == null && Input.GetKey(KeyCode.C)) // 도구 사용 도는 아이템 배치
        {
            if (HoldingItem == null)
                return;

            UseItem(HoldingItem);
            _coUseToolCooltime = StartCoroutine("CoInputCoolTime", 0.2f);

        }
        else if(Input.GetKeyDown(KeyCode.X)) // 수확/대화/탑승/재료 넣기
        {
            GameObject go = Managers.Object.FindObject(GetFrontCellPos());
            if (go == null)
                return;
            
            ObjectController oc = go.GetComponent<ObjectController>();

            if (oc.ObjectType == ObjectType.OBJECT_TYPE_ITEM)
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

                        if(sc.State == SeedState.Completed)
                        {
                            sc.OnHarvest();
                        }
                        break;
                    case ItemType.ITEM_TYPE_CRAFTING:
                        break;

                }
            }
            else if(oc.ObjectType == ObjectType.OBJECT_TYPE_INTERACTABLE_OBJECT)
            {
                UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);
                InteractableObject interactableObj = Util.GetOrAddComponent<InteractableObject>(go);
                Managers.InteractableObject.OnInteract(interactableObj);
            }

        }
        else if(Input.GetKey(KeyCode.P))
        {
            // Managers.Map.SaveMap("Assets/Resources/Map");
            Managers.SaveLoad.SaveGameData();
            Debug.Log("SaveFile saved");
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
                        if (Managers.Object.FindLandObject(GetFrontCellPos()) == null)
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

            if (Managers.Object.FindLandObject(GetFrontCellPos()) == null)
                return;

            GameObject go = Managers.Object.FindLandObject(GetFrontCellPos()).gameObject;
            if (go.name == "Land_Plowed")
            {
                PlowedLandController pc = go.GetComponent<PlowedLandController>();
                if (pc == null)
                    return;

                if(!pc.IsUsing)
                {
                    State = CreatureState.UsingItem;
                    _coUsingItem = StartCoroutine("CoStartSeed", seed);
                    pc.IsUsing = true;
                }
            }
        }
        else if (item.ItemType == ItemType.ITEM_TYPE_CRAFTING)
        {

        }
    }

    #region Skill
    IEnumerator CoStartPunch()
    {
        // 피격 판정
        GameObject go =  Managers.Object.Find(GetFrontCellPos());
        if(go !=null)
        {
            Debug.Log(go.name);
        }
        // 대기 시간
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
        pc.ObjectType = ObjectType.OBJECT_TYPE_NONE;
        pc.CellPos = GetFrontCellPos();
        pc.IsUsing = false;
        Managers.Object.Add(plowed);

        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coUsingItem = null;
    }
    IEnumerator CoStartSeed(Item item)
    {
        if(item.Count > 0 )
        {
            GameObject seed = Managers.Resource.Instantiate($"Object/Land/Seed");
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
    #endregion

}
