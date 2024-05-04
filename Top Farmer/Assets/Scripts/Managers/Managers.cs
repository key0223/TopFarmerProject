using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers instance;
    public static Managers Instance { get { Init(); return instance; } }

    #region Contents
    InventoryManager _inven = new InventoryManager();
    MapManager _map = new MapManager();
    ObjectManager _obj = new ObjectManager();
    InteractableObjectManager _interactableObj = new InteractableObjectManager();
    TimeManager _time = new TimeManager();
    WebManager _web = new WebManager();
    NpcManager _npc = new NpcManager();
    public static InventoryManager Inven { get { return Instance._inven; } }
    public static MapManager Map { get { return Instance._map; } }
    public static ObjectManager Object { get { return Instance._obj; } }
    public static InteractableObjectManager InteractableObject { get { return Instance._interactableObj; } }
    public static TimeManager Time { get { return Instance._time; } }
    public static WebManager Web { get {  return Instance._web; } }

    public static NpcManager Npc { get { return Instance._npc; } }

    #endregion

    #region Core
    DataManager _data = new DataManager();
    SaveLoadManager _saveLoad = new SaveLoadManager();
    ResourceManager _resource = new ResourceManager();
    PoolManager _pool = new PoolManager();
    SceneManagerEx _scene = new SceneManagerEx();
    UIManager _ui = new UIManager();

    public static DataManager Data { get{ return  Instance._data; } }
    public static SaveLoadManager SaveLoad {  get { return Instance._saveLoad; } }  
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static UIManager UI { get { return Instance._ui; } }

    #endregion
    void Start()
    {
        Init();
        
    }

    private void Update()
    {
        if (_scene.CurrentScene.SceneType == Define.Scene.Game)
        {
            _time.Update();
        }
    }

    static void Init()
    {
        if(instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if(go == null )
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            instance = go.GetComponent<Managers>();

            instance._data.Init();
            instance._pool.Init();
            instance._saveLoad.Init();
            //instance._time.Init();
            //instance._npc.Init();
        }
    }

    public static void Clear()
    {
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
    }
}
