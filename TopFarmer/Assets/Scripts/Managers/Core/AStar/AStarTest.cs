using Data;
using System;
using UnityEngine;
using static Define;
using Scene = Define.Scene;

[RequireComponent(typeof(AStar))]
public class AStarTest : MonoBehaviour
{
    [SerializeField] private NPCPath _npcPath = null;
    [SerializeField] private bool _moveNPC = false;
    [SerializeField] private Vector2Int _finishPosition;
    [SerializeField] private AnimationClip _idleDownAnimationClip = null;
    [SerializeField] private AnimationClip _eventAnimationClip = null;
    [SerializeField] private Scene _sceneName = Scene.Scene1_Farm;

    private NPCMovement npcMovement;


    private void Start()
    {

        npcMovement = _npcPath.GetComponent<NPCMovement>();
        npcMovement.Dir = MoveDir.Down;
        npcMovement._npcTargetAnimationClip = _idleDownAnimationClip;

    }

    private void Update()
    {
        if (_moveNPC)
        {
            _moveNPC = false;

            NPCScheduleEvent npcScheduleEvent = new NPCScheduleEvent(0, 0, 0, 0, Weather.NONE, Season.NONE, _sceneName, new GridCoordinate(_finishPosition.x, _finishPosition.y), _eventAnimationClip);

            _npcPath.BuildPath(npcScheduleEvent);

        }
       

    }
}
