using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLadder : MonoBehaviour
{
    MinesScene _minesScene;
    MineLevelManager _mineLevelManager;

    void Start()
    {
        _minesScene = FindAnyObjectByType<MinesScene>();
        _mineLevelManager = FindAnyObjectByType<MineLevelManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _minesScene.LoadMainLevel();
        }
    }

 
}
