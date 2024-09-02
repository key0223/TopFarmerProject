using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(GenerateGUID))]
public class SceneItemsManager : SingletonMonobehaviour<SceneItemsManager>,ISaveable
{
    Transform _parentItem;
    GameObject _itemPrefab = null;

    string _isSaveableUniqueID;
    public string ISaveableUniqueID { get {return _isSaveableUniqueID; } set {_isSaveableUniqueID = value; } }

    GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        //ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        ISaveableUniqueID = "SceneItemsManager";
        GameObjectSave = new GameObjectSave();

    }

    private void OnEnable()
    {
        ISaveableRegister();
        Managers.Event.AfterSceneLoadEvent += AfterSceneLoad;
    }
    private void OnDisable()
    {
        ISaveableDeregister();
        Managers.Event.AfterSceneLoadEvent -= AfterSceneLoad;
    }
    void AfterSceneLoad()
    {
        _parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    void DestorySceneItems()
    {
        Item[] itemsInScene = GameObject.FindObjectsOfType<Item>();

        for (int i = 0; i < itemsInScene.Length-1; i++)
        {
            Managers.Resource.Destroy(itemsInScene[i].gameObject);
        }
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
        // Store current scene data
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

        return GameObjectSave;
    }
    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave._gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // Restore data for current scene
            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
    public void ISaveableStoreScene(string sceneName)
    {
        // Remove old scene save for gameobject if exists
        GameObjectSave.sceneData.Remove(sceneName); ;

        // Get all items in the scene
        List<SceneItem> sceneItemList = new List<SceneItem>();
        Item[] itemInScene = FindObjectsOfType<Item>();

        foreach(Item item in itemInScene)
        {
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemId = item.ItemId;
            sceneItem.position = new Vector3Serializable(item.transform.position.x,item.transform.position.y,item.transform.position.z);
            sceneItem.itemName = item.name;

            // Add scene item to list

            sceneItemList.Add(sceneItem);
        }

        // Create list scene items list in scene save and add to it
        SceneSave sceneSave =  new SceneSave();
        sceneSave._listSceneItem =  sceneItemList;

        // Add scene save to gameobject
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }
    public void ISaveableRestoreScene(string sceneName)
    {
        if(GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if(sceneSave._listSceneItem != null)
            {
                DestorySceneItems();

                InstantiateSceneItems(sceneSave._listSceneItem);
            }
        }
    }

    public void InstantiateSceneItems(int itemId, Vector3 itemPos)
    {
        GameObject itemGameObject = Managers.Resource.Instantiate("Object/Item/Item",_parentItem);
        itemGameObject.transform.position = itemPos;
        Item item = itemGameObject.GetComponent<Item>();
        item.Init(itemId);
    }
    private void InstantiateSceneItems(List<SceneItem> sceneItemList)
    {
        GameObject itemGameObject;

        foreach(SceneItem sceneItem in sceneItemList)
        {
            itemGameObject = Managers.Resource.Instantiate("Object/Item/Item");
            Vector3 position = new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z);
            itemGameObject.transform.position = position;

            Item item = itemGameObject.GetComponent<Item>();
            item.ItemId = sceneItem.itemId;
            item.name = sceneItem.itemName;
        }

    }

    void DestroySceneItems()
    {
        Item[] itemsInScene = GameObject.FindObjectsOfType<Item>();

        for(int i = itemsInScene.Length-1; i> -1; i--)
        {
            Managers.Resource.Destroy(itemsInScene[i].gameObject);
        }
    }

  
}
