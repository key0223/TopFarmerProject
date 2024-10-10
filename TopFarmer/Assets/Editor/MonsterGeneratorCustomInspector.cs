using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonsterGenerator))]
public class MonsterGeneratorCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        SerializedProperty itemId = serializedObject.FindProperty("monsterId");
        serializedObject.ApplyModifiedProperties();



        if (GUILayout.Button("Generate"))
        {
            MonsterGenerator monsterGenerator = (MonsterGenerator)target;
            monsterGenerator.GenerateMonster();
        }

    }
}
