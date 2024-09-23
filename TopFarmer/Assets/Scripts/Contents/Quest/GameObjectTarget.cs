using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectTarget : ObjectiveTarget
{
    private GameObject _value;
    public override object Value { get { return _value; } }
    public override bool IsEquals(object target)
    {
        var targetGameObject = target as GameObject;
        if(targetGameObject == null)
            return false;
        return targetGameObject.name.Contains(_value.name);
    }
}
