using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesScene : BaseScene
{
    [SerializeField] GameObject _mainLevel;
    [SerializeField] GameObject _mainLevelBoundsConfiner;
    [SerializeField] GameObject _ladder;

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
        _mainLevelBoundsConfiner.gameObject.SetActive(false);
        _ladder.gameObject.SetActive(false);

        _currentLevelGO = Managers.Resource.Instantiate($"Map/Mines/MinesLevel_{level}");
        PolygonCollider2D polygonCollider2D = _currentLevelGO.GetComponentInChildren<PolygonCollider2D>();

        if(_cinemachineConfiner !=null && polygonCollider2D != null)
        {
            _cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;
            _cinemachineConfiner.InvalidatePathCache();
        }

        

    }

    public override void Clear()
    {

    }
}
