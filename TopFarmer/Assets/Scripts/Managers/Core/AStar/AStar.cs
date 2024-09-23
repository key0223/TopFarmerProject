using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class AStar : MonoBehaviour
{
    [Header("Tiles & Tilemap References")]
    [Header("Options")]
    [SerializeField] private bool _observeMovementPenalties = true;

    [Range(0, 20)]
    [SerializeField] private int _pathMovementPenalty = 0;
    [Range(0, 20)]
    [SerializeField] private int _defaultMovementPenalty = 0;

    private GridNodes _gridNodes;
    private Node _startNode;
    private Node _targetNode;
    private int _gridWidth;
    private int _gridHeight;
    private int _originX;
    private int _originY;

    private List<Node> _openNodeList;
    private HashSet<Node> _closedNodeList;

    private bool _pathFound = false;

    /// <summary>
    /// Builds a path for the given sceneName, from the startGridPosition to the endGridPosition, and adds movement steps to the passed in npcMovementStack.  Also returns true if path found or false if no path found.
    /// </summary>
    public bool BuildPath(Scene sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        _pathFound = false;

        if (PopulateGridNodesFromGridPropertiesDictionary(sceneName, startGridPosition, endGridPosition))
        {
            if (FindShortestPath())
            {
                UpdatePathOnNPCMovementStepStack(sceneName, npcMovementStepStack);

                return true;
            }
        }
        return false;
    }

    private void UpdatePathOnNPCMovementStepStack(Scene sceneName, Stack<NPCMovementStep> npcMovementStepStack)
    {
        Node nextNode = _targetNode;

        while (nextNode != null)
        {
            NPCMovementStep npcMovementStep = new NPCMovementStep();

            npcMovementStep._sceneName = sceneName;
            npcMovementStep._gridCoordinate = new Vector2Int(nextNode._gridPosition.x + _originX, nextNode._gridPosition.y + _originY);

            npcMovementStepStack.Push(npcMovementStep);

            nextNode = nextNode._parentNode;
        }
    }

    /// <summary>
    ///  Returns true if a path has been found
    /// </summary>
    private bool FindShortestPath()
    {
        // Add start node to open list
        _openNodeList.Add(_startNode);

        // Loop through open node list until empty
        while (_openNodeList.Count > 0)
        {
            // Sort List
            _openNodeList.Sort();

            //  current node = the node in the open list with the lowest fCost
            Node currentNode = _openNodeList[0];
            _openNodeList.RemoveAt(0);

            // add current node to the closed list
            _closedNodeList.Add(currentNode);

            // if the current node = target node
            //      then finish

            if (currentNode == _targetNode)
            {
                _pathFound = true;
                break;
            }

            // evaluate fcost for each neighbour of the current node
            EvaluateCurrentNodeNeighbours(currentNode);
        }

        if (_pathFound)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void EvaluateCurrentNodeNeighbours(Node currentNode)
    {
        Vector2Int currentNodeGridPosition = currentNode._gridPosition;

        Node validNeighbourNode;

        // Loop through all directions
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j);

                if (validNeighbourNode != null)
                {
                    // Calculate new gcost for neighbour
                    int newCostToNeighbour;

                    if (_observeMovementPenalties)
                    {
                        newCostToNeighbour = currentNode._gCost + GetDistance(currentNode, validNeighbourNode) + validNeighbourNode._movementPenalty;
                    }
                    else
                    {
                        newCostToNeighbour = currentNode._gCost + GetDistance(currentNode, validNeighbourNode);
                    }

                    bool isValidNeighbourNodeInOpenList = _openNodeList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode._gCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode._gCost = newCostToNeighbour;
                        validNeighbourNode._hCost = GetDistance(validNeighbourNode, _targetNode);

                        validNeighbourNode._parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            _openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        //  distance
        int dstX = Mathf.Abs(nodeA._gridPosition.x - nodeB._gridPosition.x);
        int dstY = Mathf.Abs(nodeA._gridPosition.y - nodeB._gridPosition.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private Node GetValidNodeNeighbour(int neighboutNodeXPosition, int neighbourNodeYPosition)
    {
        // If neighbour node position is beyond grid then return null
        if (neighboutNodeXPosition >= _gridWidth || neighboutNodeXPosition < 0 || neighbourNodeYPosition >= _gridHeight || neighbourNodeYPosition < 0)
        {
            return null;
        }

        // if neighbour is an obstacle or neighbour is in the closed list then skip
        Node neighbourNode = _gridNodes.GetGridNode(neighboutNodeXPosition, neighbourNodeYPosition);

        if (neighbourNode._isObstacle || _closedNodeList.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }

    private bool PopulateGridNodesFromGridPropertiesDictionary(Scene sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition)
    {
        // Get grid properties dictionary for the scene
        SceneSave sceneSave;

        if (GridPropertiesManager.Instance.GameObjectSave.sceneData.TryGetValue(sceneName.ToString(), out sceneSave))
        {
            // Get Dict grid property details
            if (sceneSave._griPropertyDetailDict != null)
            {
                // Get grid height and width
                if (GridPropertiesManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
                {
                    // Create nodes grid based on grid properties dictionary
                    _gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                    _gridWidth = gridDimensions.x;
                    _gridHeight = gridDimensions.y;
                    _originX = gridOrigin.x;
                    _originY = gridOrigin.y;

                    // Create _openNodeList
                    _openNodeList = new List<Node>();

                    // Create closed Node List
                    _closedNodeList = new HashSet<Node>();
                }
                else
                {
                    return false;
                }

                // Populate start node
                _startNode = _gridNodes.GetGridNode(startGridPosition.x - gridOrigin.x, startGridPosition.y - gridOrigin.y);

                // Populate target node
                _targetNode = _gridNodes.GetGridNode(endGridPosition.x - gridOrigin.x, endGridPosition.y - gridOrigin.y);

                // populate obstacle and path info for grid
                for (int x = 0; x < gridDimensions.x; x++)
                {
                    for (int y = 0; y < gridDimensions.y; y++)
                    {
                        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(x + gridOrigin.x, y + gridOrigin.y, sceneSave._griPropertyDetailDict);

                        if (gridPropertyDetails != null)
                        {
                            // If NPC obstacle
                            if (gridPropertyDetails.isNPCObstacle == true)
                            {
                                Node node = _gridNodes.GetGridNode(x, y);
                                node._isObstacle = true;
                            }
                            else if (gridPropertyDetails.isPath == true)
                            {
                                Node node = _gridNodes.GetGridNode(x, y);
                                node._movementPenalty = _pathMovementPenalty;
                            }
                            else
                            {
                                Node node = _gridNodes.GetGridNode(x, y);
                                node._movementPenalty = _defaultMovementPenalty;
                            }
                        }
                    }
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        return true;
    }
}
