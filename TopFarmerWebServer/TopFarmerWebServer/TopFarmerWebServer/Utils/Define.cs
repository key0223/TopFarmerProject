public class Define
{
    #region State
    public enum InventoryState
    {
        Inventory,
        Merchant,
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
    
    // Type
    public enum ObjectType
    {
        OBJECT_TYPE_NONE,
        OBJECT_TYPE_PLAYER,
        OBJECT_TYPE_OBJECT,
        OBJECT_TYPE_ITEM,
        OBJECT_TYPE_CREATURE,
    }

    #region Item
    public enum ItemType
    {
        ITEM_TYPE_NONE,
        ITEM_TYPE_TOOL,
        ITEM_TYPE_CROP,
        ITEM_TYPE_SEED,
        ITEM_TYPE_CRAFTING,
        ITEM_TYPE_FOOD,
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
    public enum InteractableObjectType
    {
        INTERACTABLE_OBJECT_NONE,
        INTERACTABLE_OBEJCT_TYPE_STORAGE,
        INTERACTABLE_OBEJCT_TYPE_NPC,
        INTERACTABLE_OBEJCT_TYPE_KITCHEN,
    }
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
        public int posX { get; set; }
        public int posY { get; set; }
    }
    public class ObjectInfo
    {
        //public int objectId { get; set; }
        public string name { get; set; }
        public PositionInfo posInfo { get; set; }
    }

    public class ItemInfo
    {
        public int itemDbId { get; set; }
        public int templatedId { get; set; }
        public int count { get; set; }
        public int slot { get; set; }
        public bool equipped { get; set; }
    }
    public class StatInfo
    {
        public int level { get; set; }
        public int hp { get; set; }
        public int maxHp { get; set; }
        public int attack { get; set; }
        public float speed { get; set; }
        public int totalExp { get; set; }
    }

    #endregion
}
