using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//[CustomEditor(typeof(ResourcePackLoader))]
public class TestEditorRP : Editor
{
    public override void OnInspectorGUI() 
    { 
        DrawDefaultInspector(); 
        ResourcePackLoader myScript = (ResourcePackLoader)target; 
        if (GUILayout.Button("Load")) 
        {
            myScript.LoadPack(); 
        } }
}
