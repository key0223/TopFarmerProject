using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementObjectiveAction : IObjectiveAction
{
    public int Run(Objective objective, int currentSuccess, int successCount)
    {
        return successCount > 0 ? currentSuccess + successCount : currentSuccess; //성공 값이 0보다 크다면 current값에 더해서 반환 아니면 현재 값 그대로 반환
    }
}
