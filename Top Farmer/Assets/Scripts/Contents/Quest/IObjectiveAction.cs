using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectiveAction 
{
    int Run(Objective objective, int currentSuccess, int successCount);
}
