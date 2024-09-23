using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Define;

[Serializable]
public struct InventoryItem
{
    public int _itemId;
    public int _itemQuantity;
}
public class InventoryManager :SingletonMonobehaviour<InventoryManager>, ISaveable
{
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; }set { _gameObjectSave = value; } }

    UI_InventoryBar _inventoryBar;
    public List<InventoryItem>[] _inventoryLists;
    public int[] _inventoryCapacity; // _inventoryLists의 인덱스와 해당 _inventoryLists의 저장 용량을 나타냅니다.
    private int[] _selectedInventoryItem; // 인덱스는 인벤토리 목록을, 인덱스의 값은 해당 목록에서 선택된 아이템 아이디를 나타냅니다.


    protected override void Awake()
    {
        base.Awake();

        Init();

        //ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        ISaveableUniqueID = "InventoryManager";
        GameObjectSave = new GameObjectSave();
    }
    private void OnEnable()
    {
        ISaveableRegister();
    }
    private void OnDisable()
    {
        ISaveableDeregister();
    }
    public void Init()
    {
        CreateInventoryLists();

        _inventoryBar = FindAnyObjectByType<UI_InventoryBar>();

        _selectedInventoryItem = new int[(int)InventoryType.COUNT];
        for (int i = 0; i < _selectedInventoryItem.Length; i++)
        {
            _selectedInventoryItem[i] = -1;
        }

    }
    void CreateInventoryLists()
    {
        _inventoryLists = new List<InventoryItem>[(int)InventoryType.COUNT];

        for (int i = 0; i < (int)InventoryType.COUNT; i++)
        {
            _inventoryLists[i] = new List<InventoryItem>();
        }

        _inventoryCapacity = new int[(int)InventoryType.COUNT];
        _inventoryCapacity[(int)InventoryType.INVEN_PLAYER] = PlayerInitInvenCampacity;


    }

    public void AddItem(InventoryType invenType, Item item, GameObject gameObjectToDelete)
    {
        AddItem(invenType, item);
        Managers.Resource.Destroy(gameObjectToDelete);

        //Debug.Log("Item destroyed");
    }
    public void AddItem(InventoryType invenType, Item item)
    {
        int itemId = item.ItemId;
        List<InventoryItem> inventory = _inventoryLists[(int)invenType];
        int itemPos = FindItemInInventory(invenType, itemId);

        if (itemPos != -1)
        {
            //Debug.Log($"Item found at position: {itemPos}, updating quantity.");
            AddItemAtPosition(inventory, itemId, itemPos);
        }
        else
        {
            //Debug.Log("Item not found, adding new item.");
            AddItemAtPosition(inventory, itemId);
        }

        Managers.Event.UpdateInventory(invenType, inventory);

        //AddItem(invenType, itemId);
    }

    public void AddItem(InventoryType invenType, int itemId)
    {

        List<InventoryItem> inventory = _inventoryLists[(int)invenType];

        int itemPos = FindItemInInventory(invenType, itemId);
        if(itemPos != -1)
        {
            AddItemAtPosition(inventory, itemId, itemPos);
        }
        else
        {
            AddItemAtPosition(inventory, itemId); 
        }

        Managers.Event.UpdateInventory(invenType, _inventoryLists[(int)invenType]);
    }

    // 인벤토리 끝에 새로 추가
    public void AddItemAtPosition(List<InventoryItem> inventory, int itemId)
    {
        InventoryItem invenItem = new InventoryItem();

        invenItem._itemId = itemId;
        invenItem._itemQuantity = 1;
        inventory.Add(invenItem);
    }

    // 존재하는 아이템 수정
    void AddItemAtPosition(List<InventoryItem> inventory, int itemId, int position)
    {
        InventoryItem item = new InventoryItem();

        int quantity = inventory[position]._itemQuantity + 1;

        item._itemId = itemId;
        item._itemQuantity = quantity;
        inventory[position] = item;
    }
    public void RemoveItem(InventoryType inventoryType, int itemCode)
    {
        List<InventoryItem> inventory = _inventoryLists[(int)inventoryType];
        // Check if inventory already contains the item
        int itemPosition = FindItemInInventory(inventoryType, itemCode);

        if (itemPosition != -1)
        {
            RemoveItemAtPosition(inventory, itemCode, itemPosition);
        }
        Managers.Event.UpdateInventory(inventoryType, inventory);
    }

    public void RemoveItem(InventoryType inventoryType, int itemCode, int amount)
    {
        List<InventoryItem> inventory = _inventoryLists[(int)inventoryType];
        // Check if inventory already contains the item
        int itemPosition = FindItemInInventory(inventoryType, itemCode);

        if (itemPosition != -1)
        {
            RemoveItemAtPosition(inventory, itemCode, itemPosition,amount);
        }
        Managers.Event.UpdateInventory(inventoryType, inventory);
    }
    private void RemoveItemAtPosition(List<InventoryItem> inventory, int itemId, int position)
    {
        InventoryItem invenItem = new InventoryItem();

        int quantity = inventory[position]._itemQuantity - 1;

        if (quantity > 0)
        {
            invenItem._itemQuantity = quantity;
            invenItem._itemId = itemId;
            inventory[position] = invenItem;

        }
        else
        {
            inventory.RemoveAt(position);
        }

    }
    private void RemoveItemAtPosition(List<InventoryItem> inventory, int itemId, int position, int amount)
    {
        InventoryItem invenItem = new InventoryItem();

        int quantity = inventory[position]._itemQuantity - amount;

        if (quantity > 0)
        {
            invenItem._itemQuantity = quantity;
            invenItem._itemId = itemId;
            inventory[position] = invenItem;

        }
        else
        {
            inventory.RemoveAt(position);
        }

    }

    public int FindItemInInventory(InventoryType invenType, int itemId)
    {
        List<InventoryItem> inventory = _inventoryLists[(int)invenType];

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i]._itemId == itemId)
                return i;
        }
        return -1;

    }
    private int GetFirstEmptyItemSlot(Dictionary<int, InventoryItem> inventoryDict)

    {

        foreach (KeyValuePair<int, InventoryItem> item in inventoryDict)

        {

            if (item.Value._itemId == 0) return item.Key;

        }

        return -1;

    }
    public void SetSelectedInventoryItem(InventoryType invenType, int itemId)
    {
        _selectedInventoryItem[(int)invenType] = itemId;
    }
    public void ClearSelectedInvetoryItem(in InventoryType invenType)
    {
        _selectedInventoryItem[(int)invenType] = -1;
    }
    public void SwapInventoryItems(InventoryType inventoryType, int fromItem, int toItem)
    {
        // if fromItem index and toItemIndex are within the bounds of the list, not the same, and grater than or equal to zero

        if (fromItem < _inventoryLists[(int)inventoryType].Count && toItem < _inventoryLists[(int)inventoryType].Count
               && fromItem != toItem && fromItem >= 0 && toItem >= 0)
        {
            InventoryItem fromInventoryItem = _inventoryLists[(int)inventoryType][fromItem];
            InventoryItem toInventoryItem = _inventoryLists[(int)inventoryType][toItem];

            _inventoryLists[(int)inventoryType][toItem] = fromInventoryItem;
            _inventoryLists[(int)inventoryType][fromItem] = toInventoryItem;

            //  Send event that inventory has been updated
            Managers.Event.UpdateInventory(inventoryType, _inventoryLists[(int)inventoryType]);
        }

    }

    int GetSelectedInventoryItem(InventoryType inventoryType)
    {
        return _selectedInventoryItem[(int)inventoryType];
    }
    public ItemData GetSelectedInventoryItemData(InventoryType inventoryType)
    {
        int itemId = GetSelectedInventoryItem(inventoryType);

        if(itemId == -1)
            return null;
        else
        {
            ItemData itemData = null;
            if(Managers.Data.ItemDict.TryGetValue(itemId, out itemData))
            {
                return itemData;
            }

            return itemData;
        }
    }
    public InventoryItem FindInventoryItem(InventoryType inventoryType, int itemId)
    {
        List<InventoryItem> inventory = _inventoryLists[(int)inventoryType];
        InventoryItem inventoryItem = new InventoryItem();
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i]._itemId == itemId)
            {
                inventoryItem = inventory[i];
                return inventoryItem;
            }
        }

        return inventoryItem;
       
    }

  
    public string GetItemTypeString(ItemType itemType)
    {
        string itemTypeString = null;
        switch (itemType)
        {
            case ItemType.ITEM_SEED:
                itemTypeString = SeedString;
                break;
            case ItemType.ITEM_COMODITY:
                itemTypeString = ComodityString;
                break;
            case ItemType.ITEM_FURNITURE:
                itemTypeString = FurnitureString;
                break;
            case ItemType.ITEM_TOOL_WATERING:
                itemTypeString = WateringCanstring;
                break;
            case ItemType.ITEM_TOOL_HOEING:
                itemTypeString = HoeString;
                break;
            case ItemType.ITEM_TOOL_AXE:
                itemTypeString = AxeString;
                break;
            case ItemType.ITEM_TOOL_PICKAXE:
                itemTypeString = PickaxeString;
                break;
            case ItemType.ITEM_TOOL_SCYTHE:
                itemTypeString = ScytheString;
                break;
            case ItemType.ITEM_TOOL_COLLECTING:
                itemTypeString = BasketString;
                break;
        }
        return itemTypeString;
    }

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
        // Create new scene save
        SceneSave sceneSave = new SceneSave();

        // Remove any existing scene save for persistent scene for this gameobject
        GameObjectSave.sceneData.Remove(PersistentScene);

        // Add inventory lists array to persistent scene save
        sceneSave._listInventoryItemArray = _inventoryLists;

        // Add  inventory list capacity array to persistent scene save
        sceneSave._intArrayDictionary = new Dictionary<string, int[]>();
        sceneSave._intArrayDictionary.Add("inventoryListCapacityArray", _inventoryCapacity);

        // Add scene save for gameobject
        GameObjectSave.sceneData.Add(PersistentScene, sceneSave);

        return GameObjectSave;
    }

   public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave._gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // Need to find inventory lists - start by trying to locate saveScene for game object
            if (gameObjectSave.sceneData.TryGetValue(PersistentScene, out SceneSave sceneSave))
            {
                // list inv items array exists for persistent scene
                if (sceneSave._listInventoryItemArray != null)
                {
                    _inventoryLists = sceneSave._listInventoryItemArray;

                    //  Send events that inventory has been updated
                    for (int i = 0; i < (int)InventoryType.COUNT; i++)
                    {
                        Managers.Event.UpdateInventory((InventoryType)i, _inventoryLists[i]);
                    }

                    // Clear any items player was carrying
                    PlayerController.Instance.ClearCarriedItem();

                    // Clear any highlights on inventory bar
                    _inventoryBar.ClearHighlightedOnInventorySlots();
                }

                // int array dictionary exists for scene
                if (sceneSave._intArrayDictionary != null && sceneSave._intArrayDictionary.TryGetValue("inventoryListCapacityArray", out int[] inventoryCapacityArray))
                {
                    _inventoryCapacity = inventoryCapacityArray;
                }
            }

        }
    }

   public void ISaveableStoreScene(string sceneName)
    {
        // Nothing required her since the inventory manager is on a persistent scene;
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // Nothing required her since the inventory manager is on a persistent scene;
    }
}
