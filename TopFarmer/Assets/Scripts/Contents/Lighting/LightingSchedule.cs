using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
[CreateAssetMenu(fileName = "lightingSchedule_", menuName ="Scriptable Objects/Lighting/LightingSchedule")]
public class LightingSchedule : ScriptableObject
{
    public LightingBrighteness[] lightingBrightenssArray;
}

[Serializable]
public struct LightingBrighteness
{
    public Season season;
    public int hour;
    public float lightIntensity;
}