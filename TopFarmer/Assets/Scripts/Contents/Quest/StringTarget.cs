using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringTarget : ObjectiveTarget
{
    string _value;

    public StringTarget(string value)
    {
        _value = value;
    }
    public override object Value { get { return _value; } }
    public override bool IsEquals(object target)
    {
        string targetString = target as string;
        if(targetString == null)
            return false;
        return _value == targetString;
    }
}
