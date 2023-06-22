using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Threading.Tasks;

public class TextureLoader : MonoBehaviour
{
    public string textureName;
    public uint textureId;
    public TextureFormat defaultFormat=TextureFormat.Alpha8;

    public void SetTextureByName()
    {
        /*ResourcePack rp = new ResourcePack();
        rp.Init("Data/Graphics/textures.dat");
        //rp.Init("Data/Graphics/ctrltex.dat");

        rp.LoadRAT();
        MemoryStream ms = rp.GetStreamByName(textureName);
        Texture2D texture = TextureImport.GetTexture(ms,textureName,defaultFormat);
        SetTexture(texture);*/
        SetTexture(GetTextureByName(textureName));
        
    }

    public void SetTextureById()
    {
        ResourcePack rp = new ResourcePack();
        rp.Init("Data/Graphics/textures.dat");
        rp.LoadRAT();
        Stream ms = rp.GetStreamById(textureId);
        Texture2D texture = TextureImport.GetTexture(ms, textureName);
        SetTexture(texture);
    }

    public static Texture2D GetTextureByName(string textureName)
    {
        //ResourcePack rp = new ResourcePack();
        //rp.Init("Data/Graphics/textures.dat");
        ////rp.Init("Data/Graphics/ctrltex.dat");

        //rp.LoadRAT();
        //Stream ms = rp.GetStreamByName(textureName);
        //Texture2D texture = TextureImport.GetTexture(ms, textureName);
        Texture2D texture = GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, textureName);
        return texture;
    }
    public void ExportAll()
    {
        ResourcePack rp = new ResourcePack();
        rp.Init("Data/Graphics/textures.dat");
        rp.LoadRAT();
        foreach (string resname in rp.names) 
        {
            Stream ms = rp.GetStreamByName(resname);
            Texture2D texture = TextureImport.GetTexture(ms, resname, defaultFormat);
            Textures.ExportTexture(texture,resname);
        }

    }
    
    private void SetTexture(Texture2D texture)
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        Material[] mt = mr.materials;
        foreach (Material mat in mt)
        {
            //mat.mainTexture = texture;
            //mat.SetTexture(main, texture);
            mat.SetTexture("_BaseMap", texture);
            //mat.SetTexture("_MetallicGlossMap", texture);

            
        }
        mr.materials = mt;
    }
}

[CustomEditor(typeof(TextureLoader))]
public class TextureLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TextureLoader myScript = (TextureLoader)target;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load by name"))
        {
            myScript.SetTextureByName();
        }
        if (GUILayout.Button("Load by Id"))
        {
            myScript.SetTextureById();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Export all"))
        {
            myScript.ExportAll();
        }

        if (GUILayout.Button("Import All"))
        {
            ImportAllTextures();
        }
    }
    private async void ImportAllTextures()
    {
        string[] list = GameDataHolder.ListContent(PackType.TexturesDB);

        Debug.Log("Importing " + list.Length + " items. It will take some time");
        AssetDatabase.StartAssetEditing();

        //bool NewOne = true;
        CreateTexturesAsset();
        foreach (string TextureName in list)
        {
            //ImportTexture(TextureName);
            /*if (NewOne)
            {
                CreateTexturesAsset(TextureName);
                NewOne = false;
            } else
            {
                AddTextureToAsset(TextureName);
            }*/
            AddTextureToAsset(TextureName);
            await Task.Yield();
        }
        AssetDatabase.StopAssetEditing();
        AssetDatabase.ImportAsset("Assets/Textures/uber.asset");
    }
    private void ImportTexture(string TextureName)
    {
        Texture2D texture = GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, TextureName);
        AssetDatabase.CreateAsset(texture, "Assets/Textures/" + TextureName + ".asset");
    }

    //private void CreateTexturesAsset(string TextureName)
    private void CreateTexturesAsset()
    {
        //Texture2D texture = GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, TextureName);
        Texture2D texture = new Texture2D(0, 0,TextureFormat.RGBA32,false);
        AssetDatabase.CreateAsset(texture, "Assets/Textures/uber.asset");
    }

    private void AddTextureToAsset(string TextureName)
    {
        Texture2D texture = GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, TextureName);
        AssetDatabase.AddObjectToAsset(texture, "Assets/Textures/uber.asset");
    }


}

