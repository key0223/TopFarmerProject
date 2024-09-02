using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
public class NPCPath : MonoBehaviour
{
    public Stack<NPCMovementStep> _npcMovementStepStack;

    private NPCMovement _npcMovement;

    private void Awake()
    {
        _npcMovement = GetComponent<NPCMovement>();
        _npcMovementStepStack = new Stack<NPCMovementStep>();
    }

    public void ClearPath()
    {
        _npcMovementStepStack.Clear();
    }

    public void BuildPath(NPCScheduleEvent npcScheduleEvent)
    {
        ClearPath();

        // If schedule event is for the same scene as the current NPC scene
        if (npcScheduleEvent._toSceneName == _npcMovement._npcCurrentScene)
        {
            Vector2Int _npcCurrentGridPosition = (Vector2Int)_npcMovement._npcCurrentGridPosition;

            Vector2Int npcTargetGridPosition = (Vector2Int)npcScheduleEvent._toGridCoordinate;

            // Build path and add movement steps to movement step stack
            NPCManager.Instance.BuildPath(npcScheduleEvent._toSceneName, _npcCurrentGridPosition, npcTargetGridPosition, _npcMovementStepStack);


        }
        // else if the schedule event is for a location in another scene
        else if (npcScheduleEvent._toSceneName != _npcMovement._npcCurrentScene)
        {
            SceneRoute sceneRoute;

            // Get scene route matchingSchedule
           sceneRoute = NPCManager.Instance.GetSceneRoute(_npcMovement._npcCurrentScene.ToString(), npcScheduleEvent._toSceneName.ToString());

            // Has a valid scene route been found?
            if (sceneRoute != null)
            {
                // Loop through scene paths in reverse order

                for (int i = sceneRoute.scenePathList.Count - 1; i >= 0; i--)
                {
                    int toGridX, toGridY, fromGridX, fromGridY;

                    ScenePath scenePath = sceneRoute.scenePathList[i];

                    // Check if this is the final destination
                    if (scenePath.toGridCell.x >= Define.MaxGridWidth || scenePath.toGridCell.y >= Define.MaxGridHeight)
                    {
                        // If so use final destination grid cell
                        toGridX = npcScheduleEvent._toGridCoordinate.x;
                        toGridY = npcScheduleEvent._toGridCoordinate.y;
                    }
                    else
                    {
                        // else use scene path to position
                        toGridX = scenePath.toGridCell.x;
                        toGridY = scenePath.toGridCell.y;
                    }

                    // Check if this is the starting position
                    if (scenePath.fromGridCell.x >= Define.MaxGridWidth || scenePath.fromGridCell.y >= Define.MaxGridHeight)
                    {
                        // if so use npc position
                        fromGridX = _npcMovement._npcCurrentGridPosition.x;
                        fromGridY = _npcMovement._npcCurrentGridPosition.y;
                    }
                    else
                    {
                        // else use scene path from position
                        fromGridX = scenePath.fromGridCell.x;
                        fromGridY = scenePath.fromGridCell.y;
                    }

                    Vector2Int fromGridPosition = new Vector2Int(fromGridX, fromGridY);

                    Vector2Int toGridPosition = new Vector2Int(toGridX, toGridY);

                    // Build path and add movement steps to movement step stack
                    NPCManager.Instance.BuildPath(scenePath.sceneName, fromGridPosition, toGridPosition, _npcMovementStepStack);
                }
            }
        }

        // If stack count >1, update times and then pop off 1st item which is the starting position
        if (_npcMovementStepStack.Count > 1)
        {
            UpdateTimesOnPath();
            _npcMovementStepStack.Pop(); // discard starting step

            // Set schedule event details in NPC movement
            _npcMovement.SetScheduleEventDetails(npcScheduleEvent);
        }

    }

    /// <summary>
    /// Update the path movement steps with expected gametime
    /// </summary>
    public void UpdateTimesOnPath()
    {
        // Get current game time
        TimeSpan currentGameTime = TimeManager.Instance.GetGameTime();

        NPCMovementStep previousNPCMovementStep = null;

        foreach (NPCMovementStep npcMovementStep in _npcMovementStepStack)
        {
            if (previousNPCMovementStep == null)
                previousNPCMovementStep = npcMovementStep;

            npcMovementStep._hour = currentGameTime.Hours;
            npcMovementStep._minute = currentGameTime.Minutes;
            npcMovementStep._second = currentGameTime.Seconds;

            TimeSpan movementTimeStep;

            // if diagonal
            if (MovementIsDiagonal(npcMovementStep, previousNPCMovementStep))
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Define.GridCellDiagonalSize / Define.SecondsPerGameSecond / _npcMovement._npcNormalSpeed));
            }
            else
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Define.GridCellSize / Define.SecondsPerGameSecond / _npcMovement._npcNormalSpeed));
            }

            currentGameTime = currentGameTime.Add(movementTimeStep);

            previousNPCMovementStep = npcMovementStep;
        }

    }

    /// <summary>
    /// returns true if the previous movement step is diagonal to movement step, else returns false
    /// </summary>
    private bool MovementIsDiagonal(NPCMovementStep npcMovementStep, NPCMovementStep previousNPCMovementStep)
    {
        if ((npcMovementStep._gridCoordinate.x != previousNPCMovementStep._gridCoordinate.x) && (npcMovementStep._gridCoordinate.y != previousNPCMovementStep._gridCoordinate.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
