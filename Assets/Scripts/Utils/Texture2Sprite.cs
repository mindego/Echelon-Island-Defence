using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Texture2Sprite : MonoBehaviour
{
    public Texture2D texture;
}
[CustomEditor(typeof(Texture2Sprite))]
public class Texture2SpriteEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Texture2Sprite myScript = (Texture2Sprite)target;
        string buttonName = (myScript.texture == null) ? "invalid texture" : "Import " + myScript.texture.name;

        if (GUILayout.Button(buttonName))
        {
            Sprite sp = GetSprite(myScript.texture);
            AssetDatabase.CreateAsset(sp, "Assets/Sprites/Sprite-" + myScript.texture.name + ".asset");
        }
    }
    private Sprite GetSprite(Texture2D texture2D)
    {
        Sprite res = Sprite.Create(texture2D,new Rect(0, 0, texture2D.width, texture2D.height),Vector2.zero);
        return res;
    }

    
}