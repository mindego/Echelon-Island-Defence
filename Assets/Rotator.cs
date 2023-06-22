using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Rotator : MonoBehaviour
{
    [Range(0, 90)]
    public float angle;
    public Vector3 axis;
    private Quaternion closedRotation;
    private bool setClosed = false;
    private float prevAngle;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        handleDoor();
    }

    private void handleDoor()
    {
        if (prevAngle == angle) return;

        setAngle();
    }

    public void setAngle()
    {
        if (setClosed == false)
        {
            closedRotation = transform.localRotation;
            setClosed = true;
        }
        transform.localRotation = closedRotation*Quaternion.Euler(0, angle, 0);
    }
    public void setAxis()
    {
        axis = transform.right;
    }

}
[CustomEditor(typeof(Rotator))]
public class RotatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Rotator myScript = (Rotator) target;
        if (GUILayout.Button("Set angle"))
        {
            myScript.setAngle();
        }

        if (GUILayout.Button("Set axis"))
        {
            myScript.setAxis();
        }

    }

}
