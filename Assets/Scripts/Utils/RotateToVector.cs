using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class RotateToVector : MonoBehaviour
{
    public Vector3 RotateTo;
    public Vector3 E1,E2,E3;
    public Vector3 localE1, localE2;
    public Vector3 absE1, absE2;

    public Vector3 forwardVector;
    private void OnDrawGizmos()
    {
       // Gizmos.color = Color.white;
        //Gizmos.DrawLine(transform.position, transform.position + transform.forward * 50);

    }
    private void Start()
    {
        forwardVector = transform.forward;
    }
    private void FixedUpdate()
    {
        forwardVector = transform.forward;
        localE1 = transform.localRotation * new Vector3(1, 0, 0);
        localE2 = transform.localRotation * new Vector3(0, 1, 0);
        absE1 = transform.rotation * new Vector3(1, 0, 0);
        absE2 = transform.rotation * new Vector3(0, 1, 0);
    }
    public void rotateTranstorm()
    {
        transform.localRotation=Quaternion.FromToRotation(transform.forward, transform.localRotation*RotateTo);
        forwardVector = transform.forward;


    }

    internal void rotateTranstormDirUp()
    {
        transform.localRotation = Quaternion.FromToRotation(new Vector3(1, 0, 0),E1);
        transform.localRotation*= Quaternion.FromToRotation(new Vector3(0, 1, 0),E2);

        forwardVector = transform.forward;
    }
}
[CustomEditor(typeof(RotateToVector))]
public class RotateToVectorEditor: Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RotateToVector myScript = (RotateToVector)target;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Rotate"))
        {
            myScript.rotateTranstorm();
        }
        if (GUILayout.Button("Rotate Dir/Up"))
        {
            myScript.rotateTranstormDirUp();
        }
        GUILayout.EndHorizontal();
    }
}
