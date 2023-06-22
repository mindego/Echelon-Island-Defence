using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReplaceTexture", menuName = "ScriptableObjects/Create Replace Texture Dictionary", order = 1)]
public class ResourceReplacer : ScriptableObject
{
    public List<ReplaceKeyPair> ReplaceKeyPairValues;

    public string GetTexture2D(string textureName)
    {
        //if (replaceDictionary.ContainsKey(textureName)) return replaceDictionary[textureName];
        return textureName;
    }
    [System.Serializable]
    public struct ReplaceKeyPair
    {
        public string original;
        public string replaced;
    }
}
