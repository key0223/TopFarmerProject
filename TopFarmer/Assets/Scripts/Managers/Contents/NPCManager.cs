using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AStar))]
public class NPCManager : SingletonMonobehaviour<NPCManager>
{
    [SerializeField] SO_SceneRouteList _soSceneRouteList = null;
    Dictionary<string, SceneRoute> _sceneRouteDict;

    [HideInInspector] NPC[] _npcArray;
    AStar _aStar;

    protected override void Awake()
    {
        base.Awake();

        _sceneRouteDict = new Dictionary<string, SceneRoute>();

        if(_soSceneRouteList.sceneRouteList.Count> 0 )
        {
            foreach(SceneRoute so_SceneRoute in _soSceneRouteList.sceneRouteList)
            {
                if(_sceneRouteDict.ContainsKey(so_SceneRoute.fromSceneName.ToString() + so_SceneRoute.toSceneName.ToString()))
                {
                    Debug.Log("key already exists");
                    continue;
                }

                _sceneRouteDict.Add(so_SceneRoute.fromSceneName.ToString()+ so_SceneRoute.toSceneName.ToString(), so_SceneRoute);
            }
        }

        _aStar = GetComponent<AStar>();
        _npcArray = FindObjectsOfType<NPC>();
    }

    private void OnEnable()
    {
        Managers.Event.AfterSceneLoadEvent += AfterSceneLoad;
    }

    private void OnDisable()
    {
        Managers.Event.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    void AfterSceneLoad()
    {
        SetNpcActiveStatus();
    }

    void SetNpcActiveStatus()
    {
        foreach(NPC npc in _npcArray)
        {
            NPCMovement npcMovement = npc.GetComponent<NPCMovement>();
            if(npcMovement._npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
            {
                npcMovement.SetNPCActiveInScene();
            }
            else
            {
                npcMovement.SetNPCInactiveInScene();
            }
        }
    }

    public SceneRoute GetSceneRoute(string fromSceneName, string toSceneName)
    {
        SceneRoute sceneRoute;

        if(_sceneRouteDict.TryGetValue(fromSceneName + toSceneName, out sceneRoute))
        {
            return sceneRoute;
        }
        else
            return null;
    }

    public bool BuildPath(Define.Scene sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        if(_aStar.BuildPath(sceneName,startGridPosition,endGridPosition,npcMovementStepStack))
        {
            return true;
        }
        else 
        { 
            return false; 
        }
    }
   
}
