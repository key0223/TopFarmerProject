using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecrementObjectiveAction : IObjectiveAction
{
    public int Run(Objective objective, int currentSuccess, int successCount)
    {
        return successCount < 0 ? currentSuccess - successCount : currentSuccess;
    }

    
}
