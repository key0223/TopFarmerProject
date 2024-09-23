using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSave 
{
    // string key = GUID  gameobject ID
    public Dictionary<string, GameObjectSave> _gameObjectData;
    public GameSave()
    {
        _gameObjectData = new Dictionary<string, GameObjectSave>(); 
    }
}
