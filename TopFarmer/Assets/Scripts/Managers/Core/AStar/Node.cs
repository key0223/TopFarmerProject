using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int _gridPosition;
    public int _gCost = 0; // distance from starting node
    public int _hCost = 0; // distance from finishing node
    public bool _isObstacle = false;
    public int _movementPenalty;
    public Node _parentNode;

    public Node(Vector2Int gridPostion)
    {
        this._gridPosition = gridPostion;
        _parentNode = null;
    }
    public int FCost
    {
        get
        {
            return _gCost + _hCost;
        }
    }
    public int CompareTo(Node nodeToCompare)
    {
        // compare will be <0 if this instance Fcost is less than nodeToCompare.FCost
        // compare will be >0 if this instance Fcost is greater than nodeToCompare.FCost
        // compare will be ==0 if the values are the same

        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0)
        {
            compare = _hCost.CompareTo(nodeToCompare._hCost);
        }
        return compare;
    }
}
