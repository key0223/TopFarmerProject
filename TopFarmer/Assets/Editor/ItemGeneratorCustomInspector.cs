using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

[CustomEditor(typeof(ItemGenerator))]
public class ItemGeneratorCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        SerializedProperty itemId = serializedObject.FindProperty("ItemId");
        serializedObject.ApplyModifiedProperties();



        if (GUILayout.Button("Generate"))
        {
            ItemGenerator itemGenerator = (ItemGenerator)target;
            itemGenerator.GenerateItem();
        }

    }
}
