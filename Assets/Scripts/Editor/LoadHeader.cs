using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(TerrainLoader))]
public class LoadHeader : Editor
{
   public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TerrainLoader myScript = (TerrainLoader) target;

        if (GUILayout.Button("Generate")) 
        {
            myScript.LoadHeader();
        }

        if (GUILayout.Button("Import flatmap texture")) {
            ImportFlatmapTexture(myScript.GameMapName,myScript.GameMapSize);
        }
    }

    public void ImportFlatmapTexture(string TextureName,Vector2Int GameMapSize)
    {
        
        List<string> TextureNames = new List<string>();
        foreach (string PackTextureName in GameDataHolder.ListContent(PackType.TexturesDB))
        {
            if (PackTextureName.StartsWith(TextureName))
            {
                if (!PackTextureName.EndsWith("Def"))  TextureNames.Add(PackTextureName);
            }
        }

        Texture2D defaultTexture = GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, TextureName + "Def");
        
        Texture2D mapTexture = new Texture2D(GameMapSize.x * defaultTexture.width,GameMapSize.y * defaultTexture.height,TextureFormat.RGB24,false);
        mapTexture.name = TextureName;
        Texture2D tmpTexture;
        Vector2Int pos;
        Color[] pixels;
        foreach(string PackTextureName in TextureNames)
        {
            Debug.Log("Parsing " + PackTextureName);
            tmpTexture = GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, PackTextureName);
            Debug.Log("Texture format: " + tmpTexture.format);
            pixels = tmpTexture.GetPixels();
            pos = TextureNameToPosition(PackTextureName);
            
            Debug.Log("Pos coords:" + pos);
            Debug.Log((pos.x * defaultTexture.width, pos.y * defaultTexture.height));
            mapTexture.SetPixels(pos.x * defaultTexture.width, pos.y * defaultTexture.height,defaultTexture.width,defaultTexture.height,pixels);
        }
        AssetDatabase.CreateAsset(mapTexture, "Assets/" + TextureName+ ".texture2D");
    }

    private Vector2Int TextureNameToPosition(string TextureName)
    {
        int x = Convert.ToInt32(TextureName[TextureName.Length - 2].ToString(),16);
        int y = Convert.ToInt32(TextureName[TextureName.Length - 1].ToString(),16);
        return new Vector2Int(x, y);
    }
}
