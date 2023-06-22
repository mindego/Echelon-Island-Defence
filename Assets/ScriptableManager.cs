using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScriptableManager : MonoBehaviour
{
    public ResourceReplacer resourceReplacer;
}
[CustomEditor(typeof(ScriptableManager))]
public class ScriptableManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ScriptableManager myScript = (ScriptableManager)target;
         if (GUILayout.Button("Save to xml"))
        {
            StormFileUtils.SaveXML<ResourceReplacer>("Assets/Database/Replacer.xml", myScript.resourceReplacer);
        }
        if (GUILayout.Button("Load from XML"))
        {
            ResourceReplacer rp = StormFileUtils.LoadXML<ResourceReplacer>("Assets/Database/Replacer.xml");
            Debug.Log("XML loaded");
            myScript.resourceReplacer.ReplaceKeyPairValues.Clear();
            foreach (ResourceReplacer.ReplaceKeyPair kp in rp.ReplaceKeyPairValues)
            {
                Debug.Log($"{kp.original} -> {kp.replaced}");
                myScript.resourceReplacer.ReplaceKeyPairValues.Add(kp);
            }
            DestroyImmediate(rp);
        }

    }


}
