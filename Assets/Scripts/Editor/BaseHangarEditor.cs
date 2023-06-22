using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaseHangar))]
public class BaseHangarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BaseHangar hangar = (BaseHangar)target;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Open"))
        {
            hangar.OpenDoor();
        }
        if (GUILayout.Button("Close"))
        {
            hangar.CloseDoor();
        }
        GUILayout.EndHorizontal();
    }
}
