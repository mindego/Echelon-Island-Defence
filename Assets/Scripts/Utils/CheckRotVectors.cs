using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CheckRotVectors : MonoBehaviour
{
    public Vector3 dir;
    public Vector3 up;
}
[CustomEditor(typeof(CheckRotVectors))]
public class CheckRotVectorsEditor : Editor
{
    CheckRotVectors configScript;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        configScript = (CheckRotVectors) target;

        if (GUILayout.Button("Update"))
        {
            configScript.dir = configScript.transform.forward;
            configScript.up = configScript.transform.up;
        }
    }
}

/*public class CheckRotVectorsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Update"))
        {
            Debug.Log("Pressed!");
        }
    }
}*/
