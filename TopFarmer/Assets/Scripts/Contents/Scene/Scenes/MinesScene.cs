using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesScene : BaseScene
{
    [SerializeField] GameObject _mainLevel;

    public int CurrentLevel {  get; private set; }
    GameObject _currentLevelGO;
    CinemachineConfiner _cinemachineConfiner;

     protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Scene7_Mines;
        _cinemachineConfiner = FindAnyObjectByType<CinemachineConfiner>();
        CurrentLevel = 0;
    }

    public void LoadLevel(int level)
    {
        if(_currentLevelGO != null)
        {
            Managers.Resource.Destroy( _currentLevelGO );
        }
        _mainLevel.gameObject.SetActive(false);

        _currentLevelGO = Managers.Resource.Instantiate($"Map/Mines/MinesLevel_{level}");
        PolygonCollider2D polygonCollider2D = _currentLevelGO.GetComponentInChildren<PolygonCollider2D>();

        if(_cinemachineConfiner !=null && polygonCollider2D != null)
        {
            _cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;
            _cinemachineConfiner.InvalidatePathCache();
        }
    }

    public void LoadMainLevel()
    {
        if (_currentLevelGO != null)
        {
            Managers.Resource.Destroy(_currentLevelGO);
        }
        _mainLevel.gameObject.SetActive(true);

        _currentLevelGO = _mainLevel;
        GridPropertiesManager.Instance.LoadGridPropertyDictForCurrentScene();
        PolygonCollider2D polygonCollider2D = _currentLevelGO.GetComponentInChildren<PolygonCollider2D>();

        if (_cinemachineConfiner != null && polygonCollider2D != null)
        {
            _cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;
            _cinemachineConfiner.InvalidatePathCache();
        }
       

    }
    void DestorySceneCrops()
    {
        Crop[] cropsInScene = GameObject.FindObjectsOfType<Crop>();

        for (int i = 0; i < cropsInScene.Length - 1; i++)
        {
            Managers.Resource.Destroy(cropsInScene[i].gameObject);
        }
    }
    void DestorySceneItems()
    {
        Item[] itemsInScene = GameObject.FindObjectsOfType<Item>();

        for (int i = 0; i < itemsInScene.Length - 1; i++)
        {
            Managers.Resource.Destroy(itemsInScene[i].gameObject);
        }
    }

    public override void Clear()
    {

    }
}
