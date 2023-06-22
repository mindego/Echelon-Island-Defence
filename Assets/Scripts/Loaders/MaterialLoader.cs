using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MaterialLoader : MonoBehaviour
{
    public string materialName;
    public uint materialId;

    public void SetMaterialByName()
    {
        ResourcePack rp = new ResourcePack();
        rp.Init("Data/Graphics/materials.dat");

        rp.LoadRAT();
        Stream ms = rp.GetStreamByName(materialName);

        MaterialImport.Mtl Phong = MaterialImport.GetPhongMaterial(ms, materialName);
        Texture2D maskTexture = TextureLoader.GetTextureByName("ha_bf1");
        maskTexture = MaterialImport.GetMetallicMap(Phong, maskTexture);
        Textures.ExportTexture(maskTexture,"ha_bf1-metallic-mask");
        Debug.Log(materialName + ":\n" + Phong);

        ms.Close();
    }
    public void SetMaterialById()
    {
        
        //STUB!
    }
}

[CustomEditor(typeof(MaterialLoader))]
public class MaterialLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MaterialLoader myScript = (MaterialLoader)target;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load by name"))
        {
            myScript.SetMaterialByName();
        }
        if (GUILayout.Button("Load by Id"))
        {
            myScript.SetMaterialById();
        }
        GUILayout.EndHorizontal();
    }
}