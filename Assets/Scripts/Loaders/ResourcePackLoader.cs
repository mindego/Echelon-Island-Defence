using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

public class ResourcePackLoader : MonoBehaviour
{
    public string filename = "Data/Graphics/textures.dat";
    public int resourceIndex;
    public string resourceName;
    public uint resourceId;
    public PackType selectedPack;
    public Hashtable hashtable;

    #region Particles
    public string ParticleName;
    #endregion


    public void LoadPack()
    {
        ResourcePack rp = new ResourcePack();
        if (!rp.Init(filename)) return;
        if (!rp.LoadRAT()) return;
        Debug.Log("Data of: " + rp.RAT.index[resourceIndex]);
        Debug.Log($"Data size: {rp.RAT.index.Length}");
        Debug.Log($"Data name: {rp.names[resourceIndex]}");

        //MemoryStream ms = rp.GetStreamByIndex(resourceIndex);
        //Debug.Log("Index " + ms.Length);
        //MemoryStream ms = rp.GetStreamByName(resourceName);
        //Debug.Log("Name " + ms.Length);
        //ms = rp.GetStreamById(resourceId);
        //Debug.Log("Id " + ms.Length);
        //Debug.Log("Resource Name: [" + rp.GetName(resourceIndex)+"] found");

    }


}

[CustomEditor(typeof(ResourcePackLoader))]
public class ResourcePackEditor : Editor
{
    private PackType resourcePack;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ResourcePackLoader myScript = (ResourcePackLoader)target;
        resourcePack = myScript.selectedPack;

        if (GUILayout.Button("Test Button"))
        {
            Type type = typeof(Texture2D);
            Debug.Log(type);

        }
        if (GUILayout.Button("Generate Packlist"))
        {
            List<string> list = new List<string>();
            foreach (string name in new string[] {"AiData.dat", "GameData.dat", "objects2.dat", "VisualConfigs.dat", "Voice2.sfx","Voice5.sfx", "Enviroment.sfx", "game.sfx", "objects.dat", "setup.dat",
                "Voice0.sfx","Voice3.sfx","Voice6.sfx","boxes.dat","Flares.dat","gdata.dat", "menu.sfx","pictures.dat","sounds.dat","Voice1.sfx","Voice4.sfx" })
            {
                list.Add(name);
            }

            foreach (string name in new string[] { "ctrlobj.dat", "ctrltex.dat", "dxm.dat", "materials.dat", "mesh.dat", "particles.dat", "rdata.dat", "textures.dat" })
            {
                list.Add("Graphics/" + name);
            }
            GenEnum("Packs", list.ToArray());
        }
        if (GUILayout.Button("Generate content enum"))
        {
            //GenEnum(Path.GetFileName(resourcePack.ToString()), GameDataHolder.ListContent(resourcePack));
            GenVoidDictionary(resourcePack);
        }

        if (GUILayout.Button("Import particle " + myScript.ParticleName))
        {
            ImportParticleData(myScript.ParticleName);
        }

        if (GUILayout.Button("Export All ParticleData to xml"))
        {
            ExportParticles();
        }

        if (GUILayout.Button("Load UniVarDB"))
        {
            UniVarMemROManager memMgr = new UniVarMemROManager(myScript.filename);
            Debug.Log(memMgr.GetDataByID(1));
        }

        if (GUILayout.Button("Play sound"))
        {
            //string soundName = "RadioMessage";//LostCrit1
            //string soundName = "LostCrit1";
            //string soundName = "10_секунд_до_старта";
            AudioClip audioClip = GameDataHolder.GetResource<AudioClip>(myScript.selectedPack, myScript.resourceName);
            GameObject soundplayer = new GameObject
            {
                name = "Sound player"
            };
            AudioSource aus = soundplayer.AddComponent<AudioSource>();
            aus.clip = audioClip;
            aus.loop = false;
            aus.Play();

        }
    }

    private async void ExportParticles()
    {
        var data = GameDataHolder.ListContent(PackType.ParticlesDB);
        PARTICLE_DATA pd;
        Stream st;
        foreach (string particleName in data)
        {
            st = GameDataHolder.GetResource<Stream>(PackType.ParticlesDB, particleName);
            pd = StormFileUtils.ReadStruct<PARTICLE_DATA>(st);
            st.Close();

            string dir = "Assets/Database/Particles";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            StormFileUtils.SaveXML<PARTICLE_DATA>(dir + "/" + particleName + ".xml", pd);
            await Task.Yield();
        }

        AssetDatabase.Refresh();
        await Task.Yield();
    }
    private void ImportParticleData(string ParticleName)
    {
        Stream ms = GameDataHolder.GetResource<Stream>(PackType.ParticlesDB, ParticleName);
        PARTICLE_DATA pd = StormFileUtils.ReadStruct<PARTICLE_DATA>(ms);

        //AssetDatabase.LoadAllAssetsAtPath("Assets/Textures/uber.asset");
        //Object[] texturesDB = AssetDatabase.LoadAllAssetsAtPath("Assets/Textures/uber.asset");

        //Texture2D resTexture = default;
        //foreach(Texture2D texture in texturesDB)
        //{
        //    if (texture.name == pd.GetTextureName())
        //    {

        //        int x = (int)(pd.Mapping[0] * texture.width);
        //        int y = (int)(pd.Mapping[1] * texture.height);
        //        int blockWidth = (int)((pd.Mapping[2] - pd.Mapping[0]) * texture.width);
        //        int blockHeight = (int)((pd.Mapping[3] - pd.Mapping[1]) * texture.height);
        //        Color[] pixels = texture.GetPixels(x, y, blockWidth, blockHeight);
        //        for (int i = 0; i < pixels.Length; i++)
        //        {
        //            pixels[i].r = pixels[i].g = pixels[i].b = pixels[i].a;
        //        }
        //        resTexture = new Texture2D(blockWidth, blockHeight);
        //        resTexture.SetPixels(pixels);
        //        resTexture.Apply();
        //        //Color[] pixels = texture.GetPixels();
        //        //for (int i = 0; i < pixels.Length; i++)
        //        //{
        //        //    pixels[i].r = pixels[i].g = pixels[i].b = pixels[i].a;
        //        //}
        //        //resTexture = texture;

        //        break;
        //    }
        //}


        GameObject tmp = new GameObject
        {
            name = ParticleName
        };
        tmp.transform.rotation = Quaternion.LookRotation(Vector3.up, Vector3.forward);
        ParticleSystem ps = tmp.AddComponent<ParticleSystem>();
        ps.name = ParticleName;

        var main = ps.main;
        //main.startSize = pd.SelfRadius;
        main.startSize = pd.Size[0]; //Ќачальный размер партикла.

        main.startSpeed = pd.SpeedRadius.z * pd.SpeedBase.z;

        main.startLifetime = 255 / pd.DecaySpeed;
        Debug.Log(ParticleName + "\n" + pd);
        main.maxParticles = pd.MaxParts;

        var emission = ps.emission;
        emission.rateOverTime = pd.BirthFrequence;

        var shape = ps.shape;
        //if (resTexture != default) shape.texture = resTexture;
        switch (pd.BirthType)
        {
            case ParticlesDefines.BirthType.PB_NORMAL:
                shape.shapeType = ParticleSystemShapeType.Cone;
                shape.radius = pd.BirthBase.magnitude;
                shape.angle = Mathf.Sqrt(Mathf.Pow(pd.BirthRadius.x, 2) + Mathf.Pow(pd.BirthRadius.y, 2));
                break;
            case ParticlesDefines.BirthType.PB_SPHERICAL:
                shape.shapeType = ParticleSystemShapeType.Sphere;
                break;
            case ParticlesDefines.BirthType.PB_TOROIDAL:
                shape.shapeType = ParticleSystemShapeType.Donut;
                break;
            case ParticlesDefines.BirthType.PB_TORUS:
                shape.shapeType = ParticleSystemShapeType.Donut;
                break;
        }

        var limitVelocity = ps.limitVelocityOverLifetime;
        limitVelocity.enabled = true;
        limitVelocity.drag = pd.Friction;

        var col = ps.colorOverLifetime;
        //StormGradient gradient = new StormGradient();
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[8];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[8];


        var size = ps.sizeOverLifetime;
        AnimationCurve curve = new AnimationCurve();
        float sizeMin = pd.Size.Min();
        float sizeMax = pd.Size.Max();

        col.enabled = true;
        size.enabled = true;


        Color32 tmpColor;
        for (int i = 0; i < 256; i++)
        {
            float time = (float)i / 255f;
            curve.AddKey(time, (pd.Size[i] - sizeMin) / (sizeMax - sizeMin));

            //uint colorValue = pd.Color[i];
            //byte a = (byte)((colorValue >> 24) & 0xFF);
            //byte r = (byte)((colorValue >> 16) & 0xFF);
            //byte g = (byte)((colorValue >> 8) & 0xFF);
            //byte b = (byte)((colorValue) & 0xFF);
            //tmpColor = new Color32(r, g, b, a);
            ////colorKeys[i / 32] = new GradientColorKey(tmpColor, time);
            ////alphaKeys[i / 32] = new GradientAlphaKey(a, time);
            //colorKeys[i] = new GradientColorKey(tmpColor, time);
            //alphaKeys[i] = new GradientAlphaKey(a, time);
        }

        for (int i = 0; i < 8; i++)
        {
            float time = (float)i / 8f;
            uint colorValue = pd.Color[i * 32];
            byte a = (byte)((colorValue >> 24) & 0xFF);
            byte r = (byte)((colorValue >> 16) & 0xFF);
            byte g = (byte)((colorValue >> 8) & 0xFF);
            byte b = (byte)((colorValue) & 0xFF);
            tmpColor = new Color32(r, g, b, a);
            colorKeys[i] = new GradientColorKey(tmpColor, time);
            alphaKeys[i] = new GradientAlphaKey(a / 255, time);
        }
        gradient.SetKeys(colorKeys, alphaKeys);
        col.color = gradient;

        size.size = new ParticleSystem.MinMaxCurve(1f, curve);

        //ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();

    }
    public void GenVoidDictionary(PackType packId)
    {
        string filePathAndName = "Assets/Scripts/Enums/" + packId.ToString() + ".cs";

        Dictionary<string, uint> dict = GameDataHolder.GetContentDictionary(packId);
        using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
        {
            streamWriter.WriteLine("public enum " + packId.ToString());
            streamWriter.WriteLine("{");

            foreach (KeyValuePair<string, uint> kvp in dict)
            {
                string tmpkey = kvp.Key;
                tmpkey = tmpkey.Replace(".", "_");
                tmpkey = tmpkey.Replace("/", "_");
                tmpkey = tmpkey.Replace("-", "_");
                streamWriter.WriteLine("\t" + tmpkey + "=" + kvp.Value + ",");
            }
            streamWriter.WriteLine("}");
        }
        AssetDatabase.Refresh();
    }
    public static void GenEnum(string enumName, string[] enumValues)
    {
        string filePathAndName = "Assets/Scripts/Enums/" + enumName + ".cs";
        using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
        {
            streamWriter.WriteLine("public enum " + enumName);
            streamWriter.WriteLine("{");
            foreach (string value in enumValues)
            {
                string tmpEnumName = value.Replace('.', '_');
                tmpEnumName = tmpEnumName.Replace('/', '_');
                tmpEnumName = tmpEnumName.Replace('-', '_');

                //streamWriter.WriteLine("\t" + tmpEnumName + "=\"" + value + "\",");
                streamWriter.WriteLine("\t" + tmpEnumName + ",");
            }
            streamWriter.WriteLine("}");
        }
        AssetDatabase.Refresh();
    }
}

