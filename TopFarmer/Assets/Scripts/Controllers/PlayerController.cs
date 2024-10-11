using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;
using Random = UnityEngine.Random;

public class PlayerController : CreatureController, ISaveable
{
    #region Player Stat
    float _currentHp;
    int _maxHp = 100;
    int _currentStamina;
    int _maxStamina = 230;
    #endregion

    private static PlayerController _instance;
    public static PlayerController Instance { get { return _instance; } }

    int _playerCoin;
    public int PlayerCoin
    {
        get { return _playerCoin; }
        set 
        { 
            _playerCoin = value;
            Managers.Event.UpdateCoin();
        }

    }

    public string FarmerName { get;set; }
    public string FarmName { get; set; }

    GridCursor _gridCursor;
    Cursor _cursor;
    MoveDir _cursorDir;
    CursorController _cursorController;


    float _inputX;
    float _inputY;
    WaitForSeconds _afterUseToolAnimationPause;
    WaitForSeconds _useToolAnimationPause;

    bool _isCarrying = false;
    [SerializeField]
    bool _playerInputDisabled = false;
    bool _playerToolUseDisabled = false;

    ToolEffectAnimationController _toolEffect;
    public bool PlayerInputDisabled { get { return _playerInputDisabled; } set { _playerInputDisabled = value; } }
    public bool IsCarraying
    {
        get { return _isCarrying; }
        set
        {
            if (_isCarrying == value)
                return;

            _isCarrying = value;
            UpdateAnimation();
        }
    }

    string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; }set { _iSaveableUniqueID = value; } }
    GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    [SerializeField]
    SpriteRenderer _equippedItemSpriteRenderer = null;

    Camera _mainCamera;
    Rigidbody2D _rigid;

    Coroutine _coSkillCooltime;
    Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPos, Vector3Int playerGridPos)
    {
        Vector3Int dir = Vector3Int.zero;

        if (cursorGridPos.x > playerGridPos.x)
        {
            dir = Vector3Int.right;
        }
        else if (cursorGridPos.x < playerGridPos.x)
        {
            dir = Vector3Int.left;
        }
        else if (cursorGridPos.y > playerGridPos.y)
        {
            dir = Vector3Int.up;
        }
        else if (cursorGridPos.y < playerGridPos.y)
        {
            dir = Vector3Int.down;
        }

        return dir;
    }

    public Vector3 GetPlayerViewportPosition()
    {
        // Vector3 viewport position for player((0,0) viewport bottom left, (1,1) viewport top right
        return _mainCamera.WorldToViewportPoint(transform.position);
    }
    public Vector3 GetPlayerCenterPosition()
    {
        return new Vector3(transform.position.x, transform.position.y +Define.PlayerCenterYOffset, transform.position.z);
    }
    private Vector3Int GetPlayerDirection(Vector3 cursorPosition, Vector3 playerPosition)
    {
        if (

          cursorPosition.x > playerPosition.x
          &&
          cursorPosition.y < (playerPosition.y + _cursor.ItemUseRadius / 2f)
          &&
          cursorPosition.y > (playerPosition.y - _cursor.ItemUseRadius / 2f)
          )
        {
            return Vector3Int.right;
        }
        else if (
            cursorPosition.x < playerPosition.x
            &&
            cursorPosition.y < (playerPosition.y + _cursor.ItemUseRadius / 2f)
            &&
            cursorPosition.y > (playerPosition.y - _cursor.ItemUseRadius / 2f)
            )
        {
            return Vector3Int.left;
        }
        else if (cursorPosition.y > playerPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        FarmerName = Managers.PlayerInfo.FarmerName;
        FarmName = Managers.PlayerInfo.FarmName;
        PlayerCoin = int.Parse(Managers.PlayerInfo.FarmerCoin);
        //ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        ISaveableUniqueID = "PlayerController";
        GameObjectSave = new GameObjectSave();

        _cursorController = FindObjectOfType<CursorController>();
    }


    private void OnEnable()
    {
        ISaveableRegister();

        Managers.Event.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndResetMovement;
        Managers.Event.AfterSceneLoadFadeInEvent += EnablePlayerInput;

    }
    private void OnDisable()
    {
        ISaveableDeregister();

        Managers.Event.BeforeSceneUnloadFadeOutEvent -= DisablePlayerInputAndResetMovement;
        Managers.Event.AfterSceneLoadFadeInEvent -= EnablePlayerInput;
    }
    // Start
    protected override void Init()
    {
        _gridCursor = FindObjectOfType<GridCursor>();
        _cursor = FindObjectOfType<Cursor>();

        _useToolAnimationPause = new WaitForSeconds(Define.UseToolAnimationPause);
        _afterUseToolAnimationPause = new WaitForSeconds(Define.AfterUseToolAnimationPause);

        _mainCamera = Camera.main;
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _sprite = GetComponentInChildren<SpriteRenderer>();

        _toolEffect = FindObjectOfType<ToolEffectAnimationController>();

        _speed = 30f;
        _currentHp = _maxHp;
        _currentStamina = _maxStamina;

    }
  
    private void PlayerTestInput()
    {
        if(Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvancedMinute();
        }

        if(Input.GetKey(KeyCode.G))
        {
            TimeManager.Instance.TestAdvancedDay();
        }

        if (Input.GetKey(KeyCode.L))
        {
            SceneControllerManager.Instance.FadeAndLoadScene(Define.Scene.Scene1_Farm.ToString(), transform.position);
        }
    }
    
    protected override void UpdateAnimation()
    {
        // play body animation
        if (_state == CreatureState.Idle)
        {
           
            if (_isCarrying)
            {
                switch (_lastDir)
                {
                    case MoveDir.Up:
                        _animator.Play("IDLE_CARRIED_BACK");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.Down:
                        _animator.Play("IDLE_CARRIED_FRONT");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.Left:
                        _animator.Play("IDLE_CARRIED_RIGHT");
                        _sprite.flipX = true;
                        break;
                    case MoveDir.Right:
                        _animator.Play("IDLE_CARRIED_RIGHT");
                        _sprite.flipX = false;
                        break;
                }
            }
            else
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
                        _animator.Play("IDLE_RIGHT");
                        _sprite.flipX = true;
                        break;
                    case MoveDir.Right:
                        _animator.Play("IDLE_RIGHT");
                        _sprite.flipX = false;
                        break;
                }
            }
           
        }
        else if (_state == CreatureState.Moving)
        {
            if(_isCarrying)
            {
                switch (_dir)
                {
                    case MoveDir.Up:
                        _animator.Play("RUN_CARRIED_BACK");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.Down:
                        _animator.Play("RUN_CARRIED_FRONT");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.Left:
                        _animator.Play("RUN_CARRIED_RIGHT");
                        _sprite.flipX = true;
                        break;
                    case MoveDir.Right:
                        _animator.Play("RUN_CARRIED_RIGHT");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.None:

                        break;
                }
            }
            else
            {
                switch (_dir)
                {
                    case MoveDir.Up:
                        _animator.Play("RUN_BACK");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.Down:
                        _animator.Play("RUN_FRONT");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.Left:
                        _animator.Play("RUN_RIGHT");
                        _sprite.flipX = true;
                        break;
                    case MoveDir.Right:
                        _animator.Play("RUN_RIGHT");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.None:

                        break;
                }
            }
           
        }
        else if (_state == CreatureState.ClickInput)
        {
            ItemData itemData = InventoryManager.Instance.GetSelectedInventoryItemData(InventoryType.INVEN_PLAYER);
            if (itemData == null) return;
            string animationName = GetToolAnimationName(itemData.itemType);
            if (animationName == "") return;

            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play($"{animationName}_BACK");
                    Managers.Event.StartToolAnimation(_lastDir, itemData.itemType);
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play($"{animationName}_FRONT");
                    Managers.Event.StartToolAnimation(_lastDir, itemData.itemType);
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play($"{animationName}_RIGHT");
                    Managers.Event.StartToolAnimation(_lastDir, itemData.itemType);
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play($"{animationName}_RIGHT");
                    Managers.Event.StartToolAnimation(_lastDir, itemData.itemType);
                    _sprite.flipX = false;
                    break;
                case MoveDir.None:

                    break;
            }
        }
    }
    protected override void UpdateController()
    {
        ClickInput();
        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                PlayerTestInput();
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
        }
        base.UpdateController();
    }
    protected override void UpdateIdle()
    {
        if (Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
        }
    }

    #region Click
    void ClickInput()
    {
        if(!_playerInputDisabled)
        {
            if (Input.GetMouseButton(0))
            {
                if (_gridCursor.CursorIsEnabled || _cursor.CursorIsEnabled)
                {
                    Vector3Int cursorGridPosition = _gridCursor.GetGridPositionForCursor();
                    Vector3Int playerGridPosition = _gridCursor.GetGridPositionForPlayer();
                    ProcessPlayerClickInput(cursorGridPosition, playerGridPosition);
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, 100.0f);


                if (hit.collider !=null )
                {
                    Debug.DrawLine(transform.position, hit.transform.position, Color.blue, 0.1f);
                    float distance = (hit.transform.position - transform.position).magnitude;

                    if(distance <= 2)
                    {
                        if (_cursorController.CursorType == CursorType.Gift)
                        {

                        }
                        else if (_cursorController.CursorType == CursorType.Dialogue)
                        {
                            IRaycastable raycastable = hit.transform.GetComponentInChildren<IRaycastable>();
                            if (raycastable.HandleRaycast(this))
                            {
                                Managers.Reporter.ConversationNPC(hit.transform.name);
                            }
                        }
                        else if(_cursorController.CursorType == CursorType.Quest)
                        {
                            Managers.Reporter.ItemDelivered(hit.transform.name,InventoryManager.Instance.GetSelectedInventoryItemData(InventoryType.INVEN_PLAYER));
                        }
                        Debug.Log("Target in position");
                    }
                }
            }
          
        }
        
    }

    void ProcessPlayerClickInput(Vector3Int cursorGridPos, Vector3Int playerGridPos)
    {
        ResetMovement();

        Vector3Int playerDirection = GetPlayerClickDirection(cursorGridPos, playerGridPos);

        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPos.x, cursorGridPos.y);

        ItemData itemData = InventoryManager.Instance.GetSelectedInventoryItemData(InventoryType.INVEN_PLAYER);

        if(itemData != null)
        {
            switch(itemData.itemType)
            {
                case ItemType.ITEM_SEED:
                    if(Input.GetMouseButtonDown(0))
                    {
                        ProcessClickInputSeed(gridPropertyDetails,itemData);
                    }
                    break;
                case ItemType.ITEM_COMODITY:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessClickInputCommodity(itemData);
                    }
                    break;
                case ItemType.ITEM_TOOL_WATERING:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemData, playerDirection);
                    break;
                case ItemType.ITEM_TOOL_HOEING:
                     ProcessPlayerClickInputTool(gridPropertyDetails,itemData,playerDirection);
                    break;
                case ItemType.ITEM_TOOL_AXE:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemData, playerDirection);
                    break;
                case ItemType.ITEM_TOOL_PICKAXE:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemData, playerDirection);
                    break;
                case ItemType.ITEM_TOOL_SCYTHE:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemData, playerDirection);
                    break;
                case ItemType.ITEM_TOOL_COLLECTING:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemData, playerDirection);
                    break;
                case ItemType.ITEM_WEAPON:
                    if(_coSkillCooltime == null)
                    {
                        ProcessPlayerClickInputTool(gridPropertyDetails, itemData, playerDirection);
                    }
                    break;
                case ItemType.NONE:
                    break;
                case ItemType.COUNT:
                    break;

                default:
                    break;
            }
        }
    }

   
    void ProcessClickInputSeed(GridPropertyDetails gridPropertyDetails, ItemData itemData)
    {
        if(itemData.canBeDropped && _gridCursor.CursorPositionIsValid && gridPropertyDetails.daysSinceDug >-1 && gridPropertyDetails.seedItemId == -1)
        {
            PlantSeedAtCursor(gridPropertyDetails, itemData);
        }
        else if(itemData.canBeDropped && _gridCursor.CursorPositionIsValid)
        {
            Managers.Event.DropSelectedItem();
        }
    }

    private void PlantSeedAtCursor(GridPropertyDetails gridPropertyDetails, ItemData itemData)
    {
        CropData cropData = null;
        if(Managers.Data.CropDict.TryGetValue(itemData.itemId, out cropData))
        {
            gridPropertyDetails.seedItemId = itemData.itemId;
            gridPropertyDetails.growthDays = 0;


            GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);

            Managers.Event.RemoveSelectedItemFromInventory();

            SoundManager.Instance.PlaySound(Define.Sound.SOUND_PLANT);
        }
    }
    void ProcessClickInputCommodity(ItemData itemData)
    {
        if (itemData.canBeDropped && _gridCursor.CursorPositionIsValid)
        {
            Managers.Event.DropSelectedItem();
        }
    }

    void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemData itemData, Vector3Int playerDirection)
    {
        switch(itemData.itemType)
        {
            case ItemType.ITEM_TOOL_WATERING:
                if(_gridCursor.CursorPositionIsValid)
                {
                    WaterGroundAtCursor(gridPropertyDetails,playerDirection);
                    Managers.Event.StartToolAnimation(_lastDir, itemData.itemType);
                }
                break;
            case ItemType.ITEM_TOOL_HOEING:
                if(_gridCursor.CursorPositionIsValid)
                {
                    HoeGroundAtCursor(gridPropertyDetails, playerDirection);
                    Managers.Event.StartToolAnimation(_lastDir, itemData.itemType);
                }
                break;
            case ItemType.ITEM_TOOL_AXE:
                if(_gridCursor.CursorPositionIsValid)
                {
                    ChopInPlayerDirection(gridPropertyDetails,itemData, playerDirection); 
                    Managers.Event.StartToolAnimation(_lastDir, itemData.itemType);
                }
                break;
            case ItemType.ITEM_TOOL_PICKAXE:
                if (_gridCursor.CursorPositionIsValid)
                {
                    BreakingInPlayerDirection(gridPropertyDetails, itemData, playerDirection);
                    Managers.Event.StartToolAnimation(_lastDir, itemData.itemType);
                }
                break;
            case ItemType.ITEM_TOOL_SCYTHE:
                if(_cursor.CursorPositionIsValid)
                {
                    playerDirection = GetPlayerDirection(_cursor.GetWorldPositionForCursor(), GetPlayerCenterPosition());
                    ReapInPlayerDirectionAtCursor(itemData,playerDirection);
                    Managers.Event.StartToolAnimation(_lastDir, itemData.itemType);
                }
                break;

            case ItemType.ITEM_TOOL_COLLECTING:
                if (_gridCursor.CursorPositionIsValid)
                {
                   CollectInPlayerDirection(gridPropertyDetails,itemData,playerDirection);
                }
                break;
            case ItemType.ITEM_WEAPON:
                {
                    UseWeapon();
                    Managers.Event.StartToolAnimation(_lastDir, itemData.itemType);
                }

                break;

        }
    }
    void UseWeapon()
    {
        StartCoroutine(CoUseWeapon());
    }
    IEnumerator CoUseWeapon()
    {
        PlayerInputDisabled = true;
        _playerToolUseDisabled = true;
        State = CreatureState.ClickInput;

        yield return _useToolAnimationPause;

        yield return AfterUseToolAnimationPause;

        Managers.Event.StartToolAnimation(MoveDir.None, ItemType.NONE);
        PlayerInputDisabled = false;
        _playerToolUseDisabled = false;
        State = CreatureState.Moving;
        _coSkillCooltime = StartCoroutine("ColnputCoolTime", 0.2f);
    }
    IEnumerator ColnputCoolTime(float time)
    {
        yield return new WaitForSeconds(time);
        _coSkillCooltime = null;
    }

    //It is called when a monster is hit by a weapon
    public void OnMonsterTriggered(Collider2D coll)
    {
        //TODO : Decrease monster's hp;
        ItemData itemData = InventoryManager.Instance.GetSelectedInventoryItemData(InventoryType.INVEN_PLAYER);
        if (itemData == null) return;
        WeaponData data = (WeaponData)itemData;
        MonsterController mc = coll.GetComponentInChildren<MonsterController>();

        string[] damageArray = data.damage.Split(",");
        float minDamage = float.Parse(damageArray[0]);
        float maxDamage = float.Parse(damageArray[1]);
        
        float randDamage = Random.Range(minDamage, maxDamage);
        mc.OnDamaged(randDamage,data.knokback,transform.position);
    }
    #region Tool Coroutines
    void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        SoundManager.Instance.PlaySound(Define.Sound.SOUND_WATERING);
        StartCoroutine(CoWaterGroundAtCursor(playerDirection, gridPropertyDetails));
    }
    IEnumerator CoWaterGroundAtCursor(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputDisabled = true;
        _playerToolUseDisabled = true;


        Dir = GetDirFromVec(playerDirection);
        State = CreatureState.ClickInput;

        Vector3 gripPos = new Vector3(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        // Tool Effect
        _toolEffect.UpdateAnimation(Dir, ItemType.ITEM_TOOL_WATERING, gripPos);

        yield return _useToolAnimationPause;

        if (gridPropertyDetails.daysSinceWatered == -1)
        {
            gridPropertyDetails.daysSinceWatered = 0;
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        GridPropertiesManager.Instance.DisplayWateredGround(gridPropertyDetails);

        yield return AfterUseToolAnimationPause;
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorClipInfo(0).Length);

        Managers.Event.StartToolAnimation(MoveDir.None, ItemType.NONE);
        _toolEffect.UpdateAnimation(MoveDir.None, ItemType.NONE, Vector3.zero);

        PlayerInputDisabled = false;
        _playerToolUseDisabled = false;
        State = CreatureState.Moving;

    }
    void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        SoundManager.Instance.PlaySound(Define.Sound.SOUND_HOE);
        StartCoroutine(CoHoeGroundAtCursorRoutine(playerDirection, gridPropertyDetails));

    }
    IEnumerator CoHoeGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputDisabled = true;
        _playerToolUseDisabled = true;

        Dir = GetDirFromVec(playerDirection);

        State = CreatureState.ClickInput;

        yield return _useToolAnimationPause;

        if (gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
        }

        // Set grid property to dug
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // Display dug grid tiles
        GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails);


        yield return AfterUseToolAnimationPause;

        yield return new WaitForSeconds(_animator.GetCurrentAnimatorClipInfo(0).Length);
        Managers.Event.StartToolAnimation(MoveDir.None, ItemType.NONE);

        PlayerInputDisabled = false;
        _playerToolUseDisabled = false;
        State = CreatureState.Moving;
    }

    void ReapInPlayerDirectionAtCursor(ItemData itemData, Vector3Int playerDirection)
    {
        StartCoroutine(CoReapInPlayerDirectionAtCursor(itemData, playerDirection));
    }
    IEnumerator CoReapInPlayerDirectionAtCursor(ItemData itemData, Vector3Int playerDirection)
    {
        PlayerInputDisabled = true;
        _playerToolUseDisabled = true;

        Dir = GetDirFromVec(playerDirection);
        State = CreatureState.ClickInput;

        UseToolInPlayerDirection(itemData, playerDirection);

        yield return _useToolAnimationPause;

        AnimatorClipInfo[] currentClip = _animator.GetCurrentAnimatorClipInfo(0);

        yield return new WaitForSeconds(currentClip[0].clip.length);

        Managers.Event.StartToolAnimation(MoveDir.None, ItemType.NONE);

        PlayerInputDisabled = false;
        _playerToolUseDisabled = false;

        State = CreatureState.Moving;
    }

    private void UseToolInPlayerDirection(ItemData equippedItemDetails, Vector3Int playerDirection)
    {
        if (Input.GetMouseButton(0))
        {

            // Define centre point of square which will be used for collision testing
            Vector2 point = new Vector2(GetPlayerCenterPosition().x + (playerDirection.x * (equippedItemDetails.itemUseRadius / 2f)), GetPlayerCenterPosition().y + playerDirection.y * (equippedItemDetails.itemUseRadius / 2f));

            // Define size of the square which will be used for collision testing
            Vector2 size = new Vector2(equippedItemDetails.itemUseRadius, equippedItemDetails.itemUseRadius);

            // Get Item components with 2D collider located in the square at the centre point defined (2d colliders tested limited to maxCollidersToTestPerReapSwing)
            Item[] itemArray = HelperMethods.GetComponentsAtBoxLocationNonAlloc<Item>(Define.MaxCollidersToTestPerReapSwing, point, size, 0f);

            int reapableItemCount = 0;

            // Loop through all items retrieved
            for (int i = itemArray.Length - 1; i >= 0; i--)
            {
                if (itemArray[i] != null)
                {
                    // Destroy item game object if reapable
                    if (Managers.Data.GetItemData(itemArray[i].ItemId).itemType == ItemType.ITEM_REAPABLE_SCENARY)
                    {
                        // Effect position
                        Vector3 effectPosition = new Vector3(itemArray[i].transform.position.x, itemArray[i].transform.position.y + Define.GridCellSize / 2f, itemArray[i].transform.position.z);

                        // Trigger reaping effect
                        HarvestEffectAnimationController effect = Managers.Resource.Instantiate("Effect/HarvestEffect").GetComponent<HarvestEffectAnimationController>();
                        effect.StartAnimation(effectPosition, HarvestEffectType.EFFECT_WEED);

                        SoundManager.Instance.PlaySound(Define.Sound.SOUND_SCYTHE);

                        Managers.Resource.Destroy(itemArray[i].gameObject);

                        reapableItemCount++;
                        if (reapableItemCount >= Define.MaxTargetComponentsToDestroyPerReapSwing)
                            break;
                    }
                }
            }
        }
    }

    void ChopInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemData itemData, Vector3Int playerDirection)
    {
        SoundManager.Instance.PlaySound(Define.Sound.SOUND_AXE);
        StartCoroutine(CoChopInPlayerDirection(gridPropertyDetails, itemData, playerDirection));
    }
    IEnumerator CoChopInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemData itemData, Vector3Int playerDirection)
    {
        PlayerInputDisabled = true;
        _playerToolUseDisabled = true;

        Dir = GetDirFromVec(playerDirection);
        State = CreatureState.ClickInput;

        ProcessCropWithEquippedItemInPlayerDirection(gridPropertyDetails, itemData, playerDirection);

        yield return _useToolAnimationPause;

        AnimatorClipInfo[] currentClip = _animator.GetCurrentAnimatorClipInfo(0);

        yield return new WaitForSeconds(currentClip[0].clip.length);

        Managers.Event.StartToolAnimation(MoveDir.None, ItemType.NONE);

        PlayerInputDisabled = false;
        _playerToolUseDisabled = false;

        State = CreatureState.Moving;

    }

    void BreakingInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemData itemData, Vector3Int playerDirection)
    {
        SoundManager.Instance.PlaySound(Define.Sound.SOUND_PICKAXE);
        StartCoroutine(CoBreakingInPlayerDirection(gridPropertyDetails, itemData, playerDirection));

    }
    IEnumerator CoBreakingInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemData equippedItemData, Vector3Int playerDirection)
    {
        PlayerInputDisabled = true;
        _playerToolUseDisabled = true;

        Dir = GetDirFromVec(playerDirection);
        State = CreatureState.ClickInput;

        ProcessCropWithEquippedItemInPlayerDirection(gridPropertyDetails, equippedItemData, playerDirection);

        yield return _useToolAnimationPause;

        AnimatorClipInfo[] currentClip = _animator.GetCurrentAnimatorClipInfo(0);

        yield return new WaitForSeconds(currentClip[0].clip.length);

        Managers.Event.StartToolAnimation(MoveDir.None, ItemType.NONE);

        PlayerInputDisabled = false;
        _playerToolUseDisabled = false;

        State = CreatureState.Moving;
    }

    void CollectInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemData itemdata, Vector3Int playerDicrection)
    {
        SoundManager.Instance.PlaySound(Define.Sound.SOUND_COLLECTING);
        StartCoroutine(CoCollectInPlayerDirection(gridPropertyDetails, itemdata, playerDicrection));
    }
    IEnumerator CoCollectInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemData equippedItemData, Vector3Int playerDirection)
    {
        PlayerInputDisabled = true;
        _playerToolUseDisabled = true;

        Dir = GetDirFromVec(playerDirection);
        State = CreatureState.ClickInput;

        ProcessCropWithEquippedItemInPlayerDirection(gridPropertyDetails, equippedItemData, playerDirection);

        yield return _useToolAnimationPause;

        AnimatorClipInfo[] currentClip = _animator.GetCurrentAnimatorClipInfo(0);

        yield return new WaitForSeconds(currentClip[0].clip.length);

        Managers.Event.StartToolAnimation(MoveDir.None, ItemType.NONE);

        PlayerInputDisabled = false;
        _playerToolUseDisabled = false;

        State = CreatureState.Moving;
    }

    private void ProcessCropWithEquippedItemInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemData itemData, Vector3Int playerDicrection)
    {
        Crop crop = GridPropertiesManager.Instance.GetCropObjectAtGridLocation(gridPropertyDetails);

        if (crop != null)
        {
            switch (itemData.itemType)
            {
                case ItemType.ITEM_TOOL_AXE:
                    crop.ProcessToolAction(itemData, _lastDir);
                    break;
                case ItemType.ITEM_TOOL_PICKAXE:
                    crop.ProcessToolAction(itemData, _lastDir);
                    break;
                case ItemType.ITEM_TOOL_COLLECTING:
                    crop.ProcessToolAction(itemData, _lastDir);
                    break;
            }
        }
    }
    #endregion

    #endregion
    // Set direction
    void GetDirInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(horizontal, vertical).normalized;

        Dir = GetDirFromVec(input);
        _inputX = input.x;
        _inputY = input.y;
        State = (_inputX != 0 || _inputY != 0) ? CreatureState.Moving : CreatureState.Idle;

    }

    protected override void UpdateMoving()
    {
        State = CreatureState.Moving;
        Vector2 move = new Vector3(_inputX * _speed, _inputY * _speed) * Time.deltaTime;
        _rigid.MovePosition(_rigid.position + move);
    }

    public void ShowCarriedItem(int itemId)
    {
        ItemData itemData = null;
        if(Managers.Data.ItemDict.TryGetValue(itemId, out itemData))
        {
            _equippedItemSpriteRenderer.sprite = Managers.Data.SpriteDict[itemData.itemSpritePath];
            _equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);

            IsCarraying = true;
        }
    }
    public void ClearCarriedItem()
    {
        _equippedItemSpriteRenderer.sprite = null;
        _equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);

        _isCarrying = false;
    }
    private void ResetAnimationTriggers()
    {
        _isCarrying = false;
        _dir = MoveDir.None;
        _state = CreatureState.Idle;
    }
    private void ResetMovement()
    {
        _inputX = 0;
        _inputY = 0;
        _state = CreatureState.Idle;
    }

    public void DisablePlayerInputAndResetMovement()
    {
        DisablePlayerInput();
        ResetMovement();

    }
    public void EnablePlayerInput()
    {
        PlayerInputDisabled = false;
    }
    public void DisablePlayerInput()
    {
        PlayerInputDisabled = true;
    }

    string GetToolAnimationName(ItemType itemType)
    {
        string animationName = "";

        switch (itemType)
        {
            case ItemType.ITEM_TOOL_WATERING:
                animationName = "WATERING";
                break;
            case ItemType.ITEM_TOOL_HOEING:
                animationName = "BREAKING";
                break;
            case ItemType.ITEM_TOOL_AXE:
                animationName = "BREAKING";
                break;
            case ItemType.ITEM_TOOL_PICKAXE:
                animationName = "BREAKING";
                break;
            case ItemType.ITEM_TOOL_SCYTHE:
                animationName = "SCYTHE";
                break;
            case ItemType.ITEM_TOOL_COLLECTING:
                animationName = "HARVEST";
                break;
            case ItemType.ITEM_WEAPON:
                animationName = "SCYTHE";
                break;

        }

        return animationName;
    }

    public override void OnDamaged(float damage)
    {
        base.OnDamaged(damage);
        _currentHp -= damage;
        Debug.Log($"Remain Hp  : {_currentHp}");
        if (_currentHp <= 0)
        {
            _currentHp = 0;
            Debug.Log("Player Dead");
        }
    }
    #region Save
    public void ISaveableRegister()
    {
       Managers.Save.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
       Managers.Save.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        GameObjectSave.sceneData.Remove(PersistentScene);

        SceneSave sceneSave = new SceneSave();
        sceneSave._vector3Dictionary = new Dictionary<string, Vector3Serializable>();
        sceneSave._stringDictionary = new Dictionary<string, string>();

        Vector3Serializable vector3Serializable = new Vector3Serializable(transform.position.x,transform.position.y,transform.position.z);
        sceneSave._vector3Dictionary.Add("playerPosition", vector3Serializable);
        sceneSave._stringDictionary.Add("currentScene",SceneManager.GetActiveScene().name);
        sceneSave._stringDictionary.Add("playerDirection", Dir.ToString());

        GameObjectSave.sceneData.Add(PersistentScene,sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave._gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            // Get save data dictionary for scene
            if (gameObjectSave.sceneData.TryGetValue(PersistentScene, out SceneSave sceneSave))
            {
                // Get player position
                if (sceneSave._vector3Dictionary != null && sceneSave._vector3Dictionary.TryGetValue("playerPosition", out Vector3Serializable playerPosition))
                {
                    if (SceneManager.GetActiveScene().name != "Scene3_House")
                    {

                        transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
                    }
                    else
                    {
                        transform.position = new Vector3(0, 6f,0f);
                    }
                }



                // Get String dictionary
                if (sceneSave._stringDictionary != null)
                {
                    // Get player scene
                    if (sceneSave._stringDictionary.TryGetValue("currentScene", out string currentScene))
                    {
                        SceneControllerManager.Instance.FadeAndLoadScene(currentScene, transform.position);
                    }

                    // Get player direction
                    if (sceneSave._stringDictionary.TryGetValue("playerDirection", out string playerDir))
                    {
                        bool playerDirFound = Enum.TryParse<MoveDir>(playerDir, true, out MoveDir direction);

                        if (playerDirFound)
                        {
                            _dir = direction;
                            //SetPlayerDirection(playerDirection);
                        }
                    }
                }
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        // Nothing required here since the player is on a persistent scene;
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // Nothing required here since the player is on a persistent scene;
    }
    #endregion
}
