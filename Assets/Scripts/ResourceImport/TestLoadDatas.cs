using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TestLoadDatas : MonoBehaviour
{
    public string ResourceName;
}

[CustomEditor(typeof(TestLoadDatas))]
public class TestLoadDatasEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TestLoadDatas myScript = (TestLoadDatas)target;

        if (GUILayout.Button("Load txt resource"))
        {
            LoadFile(myScript.ResourceName);
        }
    }

    public void LoadFile(string ResourceName)
    {
        Stream ms = GameDataHolder.GetResource<Stream>(PackType.gData,ResourceName);

        //READ_TEXT_STREAM reader = new READ_TEXT_STREAM(ms, ';', false, 0);
        //Debug.Log(reader.ReadLine(true));
        CARRIER_DATA cd = new CARRIER_DATA();
        cd.loadCarrierData(ms);
    }
}
