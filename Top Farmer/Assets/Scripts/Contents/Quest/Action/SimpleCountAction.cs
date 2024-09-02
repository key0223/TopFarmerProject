using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCountAction : IObjectiveAction
{
    public int Run(Objective objective, int currentSuccess, int successCount)
    {
        return currentSuccess + successCount;
    }
}
