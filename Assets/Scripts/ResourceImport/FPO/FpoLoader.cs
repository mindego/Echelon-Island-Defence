using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Serialization;
using System.Threading.Tasks;

public class FpoLoader : MonoBehaviour
{
    public string FpoName;
    public string FpoId;
    //public FPODB UseFPO;
    public Vector3 Dir;
    public Vector3 Up;
    [Range (0,3)]
    public int image;


    public void GetGameObject()
    {
        //FpoGraphData fd = GameDataHolder.GetResource<FpoGraphData>(PackType.MeshDB, "ha_int1_Im0");

        GameObject FPO = GameDataHolder.GetResource<GameObject>(PackType.FPODB, FpoName);
        FPO.transform.rotation = Quaternion.LookRotation(Up,Dir * -1);
        //Debug.Log(FPO);
       // FpoController fpc = FPO.GetComponent<FpoController>();
       // fpc.SetGraph(0);

    }
}

[CustomEditor(typeof(FpoLoader))]
public class FpoLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        FpoLoader myScript = (FpoLoader)target;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load by name"))
        {
            myScript.GetGameObject();
        }
        if (GUILayout.Button("Load by Id"))
        {
            myScript.GetGameObject();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Load by Enum"))
        {
            //string[] list = GameDataHolder.ListContent(PackType.FPODB);
            //int pos = (int) myScript.UseFPO;
            //myScript.FpoName = list[pos];
            myScript.GetGameObject();
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save XML"))
        {
            FpoBuilder.SaveXML(myScript.FpoName);
        }
        if (GUILayout.Button("Load XML"))
        {
            FpoBuilder fpoBuilder = new FpoBuilder();
            fpoBuilder.LoadXML(myScript.FpoName);
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Import gameobject from XML"))
        {
            FpoBuilder fpoBuilder = new FpoBuilder();
            fpoBuilder.BuildFPO(myScript.FpoName);
        }

        if (GUILayout.Button("Generate All FPO XMLs"))
        {
            GenerateAllFPOXMLS();
        }
    }

    public async void GenerateAllFPOXMLS()
    {
        string[] list = GameDataHolder.ListContent(PackType.FPODB);
        AssetDatabase.StopAssetEditing();
        foreach (string item in list)
        {
            FpoBuilder.SaveXML(item);
            await Task.Yield();

        }
        Debug.Log("XML generation finished. Importing assets");
        AssetDatabase.StartAssetEditing();
        await Task.Yield();
    }


}
