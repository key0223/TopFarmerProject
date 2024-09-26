using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Define
{
    static Define()
    {
        EventAnimation = Animator.StringToHash("EventAnimation");
    }
    // Scene
    public const string PersistentScene = "PersistentScene";


    // Tilemap
    public const float GridCellSize = 1f;
    public const float GridCellDiagonalSize = 1.41f; // diagonal distance bwtween unity cell centers
    public const int MaxGridWidth = 99999;
    public const int MaxGridHeight = 99999;
    public static Vector2 CursorSize = Vector2.one;

    

    // Item Sprite Path "Path+/sprite number
    public const string ObjectSpritePath = "Textures/";

    // Obscuring Item fading -ObscuringItemFader
    public const float FadeInSeconds = 0.25f;
    public const float FadeOutSecons = 0.35f;
    public const float TargetAlpha = 0.45f;

    public const int PlayerInitInvenCampacity = 24;
    public const int PlayerMaxInvenCampacity = 36;

    // Player
    public static float PlayerCenterYOffset = 0.875f;

    // Player Movement
    public const float RunningSpeed = 35f;
    public const float WalkingSpeed = 20f;
    public static float UseToolAnimationPause = 0.25f;
    public static float AfterUseToolAnimationPause = 0.2f;

    // NPC Movement
    public static float PixelSize = 0.0625f;

    public static int EventAnimation;

    // Reaping
    public const int MaxCollidersToTestPerReapSwing = 15;
    public const int MaxTargetComponentsToDestroyPerReapSwing = 2;

    // String
    public const string SeedString = "Seed";
    public const string ComodityString = "Comodity";
    public const string FurnitureString = "Furniture";
    public const string HoeString = "Hoe";
    public const string AxeString = "Axe";
    public const string PickaxeString = "Pickaxe";
    public const string ScytheString = "Scythe";
    public const string WateringCanstring = "Watering Can";
    public const string BasketString = "Basket";

    // Prefab Path
    public const string CropStandardPrefabPath = "Object/Item/CropStandard";
    public const string OakTreePrefabPath = "Object/Item/CropTree_Oak";
    public const string MapleTreePrefabPath = "Object/Item/CropTree_Maple";
    public const string PineTreePrefabPath = "Object/Item/CropTree_Pine";

    // Number Sprite
    public const string Number0SpritePath = "UI/TimeUI/TimeUI_30";
    public const string Number1SpritePath = "UI/TimeUI/TimeUI_29";
    public const string Number2SpritePath = "UI/TimeUI/TimeUI_28";
    public const string Number3SpritePath = "UI/TimeUI/TimeUI_23";
    public const string Number4SpritePath = "UI/TimeUI/TimeUI_22";
    public const string Number5SpritePath = "UI/TimeUI/TimeUI_21";
    public const string Number6SpritePath = "UI/TimeUI/TimeUI_20";
    public const string Number7SpritePath = "UI/TimeUI/TimeUI_19";
    public const string Number8SpritePath = "UI/TimeUI/TimeUI_18";
    public const string Number9SpritePath = "UI/TimeUI/TimeUI_17";

    public enum CursorType
    {
        None = 0,
        Gift,
        Dialogue,
        Quest,
    }
    // Quest

    public enum QuestType
    {
        Basic,
        Crafing,
        ItemDelivery,
        Monster,
        Socialize,
        Location,
        Finshing,
        Building,
        Harvest,
        Resource,
    }
    public enum ObjectiveType
    {
        ItemDelivery,
        Monster,
        Socialize,
        Location,
    }
    public enum QuestState
    {
        Inactive,
        Running,
        Complete,
        Cancel,
        WatingForCompletion, //�Ϸ� ��ư�� ������ �Ϸ�Ǵ� ����Ʈ
    }
    public enum ObjectiveActionType
    {
        Increment,
        Decrement,
    }
    public enum ObjectiveState
    {
        Inactive,
        Running,
        Complete,
    }
    public enum ObjectiveGroupState
    {
        Running,
        Complete,
    }
    public enum MailType
    {
        Quest,
        Reward,
    }
    public enum Scene
    {
        PersistentScene,
        Scene1_Farm,
        Scene2_Field,
        Scene3_House,
        Scene4_Town,
        Scene5_Title,
        Scene6_Shop,
        Scene7_Mines,
        SceneMinesLevel_1,
    }

    // Time
    public const float SecondsPerGameSecond = 0.012f;

    public enum Season
    {
        NONE,
        SPRING,
        SUMMER,
        AUTUMN,
        WINTER,
        COUNT,
    }
    
    // Grid
    public enum GridBoolProperty
    {
        Diggable,
        CanDropItem,
        CanPlaceFurniture,
        IsPath,
        IsNPCObstacle,
        CanSpawnMonster,
    }
   
    public enum Layer
    {
        Player = 6,
        Wall =7,
    }
    #region State
    public enum StateMarkState
    {
        ExclamationMark,
        ExclamationMark_Red,
        QuestionMark,
        ProgressingMark,
    }
    
    public enum Weather
    {
        NONE,
        SUNNY,
        RAINING,
        SNOWING,
        COUNT,
    }
    public enum UIEvent
    {
        PointerEnter,
        PointerClick,
        BeginDrag,
        Drag,
        EndDrag,
        Drop,
    }
    public enum CreatureState
    {
        Idle,
        Moving,
        Skill,
        ClickInput,
        Dead,
        Event,
    }
    
   
    public enum MoveDir
    {
        None,
        Up,
        Down,
        Left,
        Right
    }
    #endregion

    #region Effect

    public enum HarvestEffectType
    {
       NONE,
       EFFECT_REAPING,
       EFFECT_WEED,
       EFFECT_LAEAVES_FALLING,
       EFFECT_BREAKING_STONE,
    }
    
    public enum Sound
    {
        NONE = 0,
        FOOTSTEP_SOFT_GROUND = 10,
        FOOTSTEP_HARD_GROUND= 20,
        SOUND_WATERING = 30,
        SOUND_HOE =40,
        SOUND_AXE = 50,
        SOUND_PICKAXE = 60,
        SOUND_SCYTHE = 70,
        SOUND_COLLECTING =80,
        SOUND_PICKUP = 90,
        SOUND_RUSTLE = 100,
        SOUND_PLANT = 110,
        SOUND_PLUCK = 120,
        SOUND_COUNTRYSIDE_1 =1000,
        SOUND_COUNTRYSIDE_2 =1010,
        SOUND_INDOOR = 1020,
        SOUND_BACKGROUND_1 =2000,
        SOUND_BACKGROUND_2 =2010,
    }

    #endregion
    #region Item
    public enum InventoryType
    {
        INVEN_PLAYER,
        INVEN_CHEST,
        COUNT,
    }
    public enum ItemType
    {
        NONE,
        ITEM_SEED,
        ITEM_COMODITY,
        ITEM_FURNITURE,
        ITEM_REAPABLE_SCENARY,
        ITEM_TOOL_WATERING,
        ITEM_TOOL_HOEING,
        ITEM_TOOL_AXE,
        ITEM_TOOL_PICKAXE,
        ITEM_TOOL_SCYTHE,
        ITEM_TOOL_COLLECTING,
        COUNT,
    }
   
   
    #endregion

    #region Creature
    public enum CreatureType
    {
        CREATURE_TYPE_NPC,
        CREATURE_TYPE_MONSTER,
    }
    public enum NpcType
    {
        NPC_TYPE_MERCHANT,
    }
    public enum MonsterType
    {
        MONSTER_TYPE_CONTACT,
        MONSTER_TYPE_RANGED,
        MONSTER_TYPE_COUNTERATTACK,
    }
    #endregion

    #region System Message

    public enum SYSTEM_MESSAGE
    {
        Game_NoAddress,
        Game_LoginOK,
        Game_WrongPassword,

        Game_CreateAddressOK,
    }
    #endregion
    #region Info Class 

    public class PositionInfo
    {
        public int posX;
        public int posY;
    }
    public class ObjectInfo
    {
        public int objectId;
        public string name;
        public PositionInfo posInfo;
    }

    public class StatInfo
    {
        public int level;
        public int hp;
        public int maxHp;
        public int attack;
        public float speed;
        public int totalExp;
    }


    #endregion
}
