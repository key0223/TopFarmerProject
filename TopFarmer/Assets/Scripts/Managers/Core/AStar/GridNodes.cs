using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNodes 
{
    private int _width;
    private int _height;

    private Node[,] gridNode;

    public GridNodes(int width, int _height)
    {
        this._width = width;
        this._height = _height;

        gridNode = new Node[width, _height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                gridNode[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    public Node GetGridNode(int xPosition, int yPosition)
    {
        if (xPosition < _width && yPosition < _height)
        {
            return gridNode[xPosition, yPosition];
        }
        else
        {
            Debug.Log("Requested grid node is out of range");
            return null;
        }
    }
}
