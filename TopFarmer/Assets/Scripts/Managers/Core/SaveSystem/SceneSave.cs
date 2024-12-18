using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    // 특정 씬에 대한 데이터를 저장
    public Dictionary<string, bool> _boolDictionary;
    public Dictionary<string, string> _stringDictionary;
    public Dictionary<string, Vector3Serializable> _vector3Dictionary; // save the player's position
    public List<SceneItem> _listSceneItem;
    public Dictionary<string, GridPropertyDetails> _griPropertyDetailDict;

    // Inventory
    public Dictionary<string, int[]> _intArrayDictionary;
    //public Dictionary<int, InventoryItem>[] _inventoryDictionaries;
    public List<InventoryItem>[] _listInventoryItemArray;

    // Time
    public Dictionary<string, int> _intDictionary;

    // Quest
    public List<QuestSave> _activeQuests;

    // Mails
    public Dictionary<string, string[]> _mails;

}
