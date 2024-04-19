using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Define 
{
    #region State
    public enum InventoryState
    {
        Inventory,
        Merchant,
        Oven,
    }

    public enum StateMarkState
    {
        ExclamationMark,
        ExclamationMark_Red,
        QuestionMark,
        ProgressingMark,
    }
    public enum StoveState
    {
        Empty,
        Using,
        Completed,
    }
    public enum SeedStoreState
    {
        Available,
        Unavailable,
        ItemGrid,
        Purchase,
        CheckPurchase,
    }
    public enum DayState
    {
        Dawn,
        Day,
        Noon,
        Night,
        Lightning_1,
        Lightning_2,
        Raining,
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
        UsingItem,
        Dead,
    }
    public enum SeedState
    {
        None,
        Progressing,
        Completed,
    }
    public enum MerchantState
    {
        TradeCompleted,
        Resting,
        Working_Inside,
        Working_Outside,
    }
    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
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

    #region Type
    public enum InteractableObjectType
    {
       INTERACTABLE_OBEJCT_TYPE_STORAGE,
       INTERACTABLE_OBEJCT_TYPE_NPC,
       INTERACTABLE_OBEJCT_TYPE_KITCHEN,
    }
    public enum NpcType
    {
        NPC_TYPE_MERCHANT,
    }
    public enum ObjectType
    {
        OBJECT_TYPE_NONE,
        OBJECT_TYPE_ITEM,
        OBJECT_TYPE_INTERACTABLE_OBJECT,
        OBJECT_TYPE_NPC,
    }
    public enum ItemType
    {
        ITEM_TYPE_NONE,
        ITEM_TYPE_TOOL,
        ITEM_TYPE_CROP,
        ITEM_TYPE_SEED,
        ITEM_TYPE_CRAFTING,
        ITEM_TYPE_FOOD,
        ITEM_TYPE_MODERN,
    }
    public enum ToolType
    {
        TOOL_TYPE_NONE,
        TOOL_TYPE_PICKAXE,
        TOOL_TYPE_AXE,
        TOOL_TYPE_HOE,
        TOOL_TYPE_WATERINGCAN,
    }
    public enum CraftingType
    {
        CRAFTING_TYPE_KITCHEN,
        CRAFTING_TYPE_DECORATION,
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

   
    #endregion
}