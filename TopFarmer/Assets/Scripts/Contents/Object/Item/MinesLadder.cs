using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesLadder : MonoBehaviour
{

    MinesScene _minesScene;

    void Start()
    {
        _minesScene= FindAnyObjectByType<MinesScene>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            int nextLevel = _minesScene.CurrentLevel + 1;
            _minesScene.LoadLevel(nextLevel);
        }
    }
}
