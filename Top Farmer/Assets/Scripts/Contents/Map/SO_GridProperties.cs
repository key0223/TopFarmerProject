using System.Collections.Generic;
using UnityEngine;
using static Define;

[CreateAssetMenu(fileName = "so_GridProperties", menuName = "Scriptable Objects/Grid Properties")]
public class SO_GridProperties : ScriptableObject
{
    public Scene sceneName;
    public int gridWidth;
    public int gridHeight;
    public int originX; // Bottom Left
    public int originY;

    [SerializeField]
    public List<GridProperty> gridPropertyList;
}
