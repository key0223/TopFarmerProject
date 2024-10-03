using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Managers : MonoBehaviour
{
    static Managers instance;
    public static Managers Instance { get { Init(); return instance; } }

    #region Contents
    PlayerInfo _playerInfo = new PlayerInfo();
    EventsHandler _event = new EventsHandler();
    EventReporter _reporter = new EventReporter();  
    QuestManager _quest = new QuestManager();
    DialogueManager _dialogue = new DialogueManager();
    VFXManager _vfx = new VFXManager();

    public static EventsHandler Event { get { return Instance._event; } }
    public static PlayerInfo PlayerInfo { get { return Instance._playerInfo; } }
    public static EventReporter Reporter { get {  return Instance._reporter; } }
    public static QuestManager Quest { get {  return Instance._quest; } }
    public static DialogueManager Dialogue { get {  return Instance._dialogue; } }
    public static VFXManager VFX { get { return Instance._vfx; } }

    #endregion

    #region Core
    DataManager _data = new DataManager();
    ResourceManager _resource = new ResourceManager();
    PoolManager _pool = new PoolManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SaveLoadManager _save = new SaveLoadManager();
    //UIManager _ui = new UIManager();
    //SceneItemsManager _sceneItem = new SceneItemsManager();

    public static DataManager Data { get{ return  Instance._data; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static SaveLoadManager Save { get {  return Instance._save; } }
    //public static UIManager UI { get { return Instance._ui; } }
    //public static SceneItemsManager SceneItem { get {  return Instance._sceneItem; } }

    #endregion

    public Weather _currentWeather;
    private bool _isFirstLoad = true;
    public bool IsFirstLoad { get { return _isFirstLoad; }set { _isFirstLoad = value; } }
    void Start()
    {
        Init();

        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow, 0);

        // Temp

        _currentWeather = Weather.SUNNY;
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
            instance._save.Init();
            instance._playerInfo.Init();
            //instance._time.Init();
        }
    }

    private void Update()
    {
        //Time.Update();
    }

    public static void Clear()
    {
        Scene.Clear();
        //UI.Clear();
        Pool.Clear();
        //Inven.Clear();
    }
}
