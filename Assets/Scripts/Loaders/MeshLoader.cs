using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MeshLoader : MonoBehaviour
{
    public string meshName;
    public uint meshId;
    public void SetModelByName()
    {
        /* ResourcePack rp = new ResourcePack();
         rp.Init("Data/Graphics/mesh.dat");
         rp.LoadRAT();

         //MemoryStream ms = rp.GetStreamByName("avalon0132_Im0");

         //FpoData.FpoGraphData lods = FileUtils.ReadSruct<FpoData.FpoGraphData>(ms);
         //Debug.Log(lods);

         MemoryStream ms = rp.GetStreamByName(meshName);
         GameObject gameObject = StormMeshImport.GetModelByName(ms, meshName);*/

        //GameObject gameObject = StormMeshImport.GetModelByName(GameDataHolder.GetResource<GameObject>(PackType.MeshDB, meshId));

        GameObject gameObject = GameDataHolder.GetResource<GameObject>(PackType.MeshDB, meshName);
        Debug.Log(gameObject);
    }

    public void SetModelById()
    {
        //GameObject gameObject = StormMeshImport.GetModelById(GameDataHolder.GetResource<GameObject>(PackType.MeshDB, (uint) meshId)),meshId); ;
        GameObject gameObject = GameDataHolder.GetResource<GameObject>(PackType.MeshDB, meshId);
    }
}

[CustomEditor(typeof(MeshLoader))]
public class MeshLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MeshLoader myScript = (MeshLoader)target;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load by name"))
        {
            myScript.SetModelByName();
        }
        if (GUILayout.Button("Load by Id"))
        {
            myScript.SetModelById();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Generate Inverted Cube"))
        {
            Mesh mesh = Instantiate(Resources.GetBuiltinResource<Mesh>("Cube.fbx"));
            var normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = normals[i] * -1;
            }
            mesh.normals = normals;

            mesh.name = "Inverted Cube";
            AssetDatabase.CreateAsset(mesh, "Assets/InvertedCube.mesh");
        }
    }
}
