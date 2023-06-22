using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class MaterialImport 
{
    private const string DEFAULT_MATERIAL = "Universal Render Pipeline/Lit";
    private const string DEFAULT_PHONG_MATERIAL = "Universal Render Pipeline/Simple Lit";

    private static MaterialImport instance;

    public MaterialImport GetInstance()
    {
        if (instance == null) instance = new MaterialImport();
        return instance;
    }

    public static Mtl GetPhongMaterial(Stream ms, string name="")
    {
        /* из editmtl.cpp
         * Diffuse - цвет источника, на который влияет расстояния.
Ambient - цвет источника, на который не влияет расстояние/угол.
Specular - цвет источника, зависящий от угла нормали (блики).
Attenuation(0, 1, 2), Range - параметр затухания.
        */
        int multiplier = 1;
        byte[] buffer = new byte[4];

        ms.Seek(0, SeekOrigin.Begin);

        Mtl PhongMaterialData = new Mtl();
        PhongMaterialData.diffuse = GetVector3(ms)* multiplier;
        PhongMaterialData.diffuse_a = GetFloat(ms)* multiplier;
        PhongMaterialData.ambient = GetVector3(ms)* multiplier;
        PhongMaterialData.ambient_a = GetFloat(ms)* multiplier;
        PhongMaterialData.specular = GetVector3(ms)* multiplier;
        PhongMaterialData.specular_a = GetFloat(ms)* multiplier;
        PhongMaterialData.emissive = GetVector3(ms)* multiplier;
        PhongMaterialData.emissive_a = GetFloat(ms)* multiplier;
        PhongMaterialData.power = GetFloat(ms) ;

        return PhongMaterialData;
    }

    public static D3DMATERIAL7 GetD3DMATERIAL7(Stream ms)
    {
        D3DMATERIAL7 StormMaterial = StormFileUtils.ReadStruct<D3DMATERIAL7>(ms);
        Debug.Log(StormMaterial);
        return StormMaterial;
    }

    public static Material GetMaterial(Mtl PhongMaterial,Texture2D texture)
    {
        Shader shader = Shader.Find(DEFAULT_MATERIAL);
        Material material = new Material(shader);

        material.mainTexture = texture;
       
        return material;
    }

    public static Material GetMaterial(Stream ms, string name)
    {
        Material material;
        if (!name.Contains('#'))
        {
            MaterialImport.D3DMATERIAL7 mat = MaterialImport.GetD3DMATERIAL7(ms);
            //Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            Shader shader = Shader.Find("HDRP/Lit");
            material = new Material(shader);
            material.name = name;
            material.SetColor("_BaseColor", mat.diffuse.ToColor());
            //material.SetColor("_SpecColor",mat.specular.ToColor());

            //material.EnableKeyword("_EMISSION");
            //material.SetColor("_EmissionColor", mat.emissive.ToColor());

            //material.SetFloat("_WorkflowMode", 0f);
            return material;
        }
        string[] parts = name.Split('#');

        Material templateMaterial = GameDataHolder.GetResource<Material>(PackType.MaterialsDB, parts[0]);
        Texture2D texture = GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, parts[1]);

        material = new Material(templateMaterial);
        Debug.Log(texture.name);
        material.name = name;
        material.mainTexture = texture;

        return material;


    }

    public static Material GetMaterialOld(Stream ms,string name)
    {
        Mtl PhongMaterialData = GetPhongMaterial(ms, name);

        Shader shader = Shader.Find(DEFAULT_PHONG_MATERIAL);
        Material material = new Material(shader);
        material.name = "Generated " + name;
        material.SetColor("_BaseColor", new Color(PhongMaterialData.diffuse.x, PhongMaterialData.diffuse.y, PhongMaterialData.diffuse.z, PhongMaterialData.diffuse_a));
        material.SetColor("_SpecColor", new Color(PhongMaterialData.specular.x, PhongMaterialData.specular.y, PhongMaterialData.specular.z, PhongMaterialData.specular_a));
        material.SetColor("_EmissionColor", new Color(PhongMaterialData.emissive.x, PhongMaterialData.emissive.y, PhongMaterialData.emissive.z, PhongMaterialData.emissive_a));
        //material.EnableKeyword("_EMISSION");
        return material;
    }
    public static Material GetMaterial(uint materialId)
    {
        Shader shader = Shader.Find(DEFAULT_PHONG_MATERIAL);
        Material material = new Material(shader);


        switch (materialId)
        {
            case 0x26F56FA4:
                Debug.Log("ME GLASS!");
                /*material.name = "Generated Glass";
                material.SetFloat("_Smoothness", 1f);
                Color tmpColor = material.color;
                tmpColor.a = 0.25f;
                material.color = tmpColor;

                material.SetFloat("_WorkflowMode", 0);

                //material.SetOverrideTag("RenderType", "Opaque");
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetFloat("_Surface", 1f);
                material.SetFloat("_Blend", 0.0f);
                */


                /*
                material.SetFloat("_Mode", 2);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                */
                break;
            default:
                break;
        }

        return material;
    }
    public static Material GetMaterialStub(uint materialId)
    {
        Shader shader = Shader.Find(DEFAULT_MATERIAL);
        Material material = new Material(shader);

        if (materialId == 0x26F56FA4)
        {
            foreach(Material mat in Resources.FindObjectsOfTypeAll<Material>())
            {
                //Debug.Log(mat.name + "\n");
                if (mat.name == "Glass") return mat;
            }
        }
        
        return material;
    }
    public static Texture2D GetMetallicMap(Mtl PhongMaterial,Texture2D texture)
    {
        Texture2D resTexture = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false)
        {
            name = texture.name + "_metallic"
        };
        Color[] pixels = texture.GetPixels();
        Color[] outPixels = new Color[pixels.Length];
        Color inColor = pixels[300];
        Color outColor;
        Color PhongColorAmbient = new Color(PhongMaterial.ambient.x, PhongMaterial.ambient.y, PhongMaterial.ambient.z);
        Color PhongColorSpecular = new Color(PhongMaterial.specular.x, PhongMaterial.specular.y, PhongMaterial.specular.z,PhongMaterial.specular_a/2);
        Debug.Log(("Example Pixel data: ",pixels[300]));
        Debug.Log(("Phong Spec: ", PhongColorSpecular));
        Debug.Log(("DiffA: ", PhongColorSpecular - pixels[300]));
        Debug.Log(("DiffB: ", pixels[300] - PhongColorSpecular));
        Debug.Log(("Constructed: "+(new Color(inColor.r - PhongColorSpecular.r, inColor.g - PhongColorSpecular.g, inColor.b - PhongColorSpecular.b))));
        Debug.Log(("Distance: ", GetColorDistance(inColor, PhongColorSpecular)));

        for (int i=0;i< pixels.Length;i++)
        {
            inColor = pixels[i];

            //outColor = inColor - PhongColorAmbient;
            //outColor = inColor;
            //outColor = PhongColorSpecular - inColor;
            //outColor = new Color(inColor.r - PhongColorSpecular.r, inColor.g - PhongColorSpecular.g, inColor.b - PhongColorSpecular.b);
            float distanceSpec = GetColorDistance(inColor, PhongColorSpecular);
            float distanceAmbient = GetColorDistance(inColor, PhongColorAmbient);
            outColor = new Color(distanceSpec, 0, 0, distanceAmbient);


            outPixels[i] = outColor;
        }
        resTexture.SetPixels(outPixels);
        resTexture.Apply();
        return resTexture;
    }

    private static float GetColorDistance(Color colorA,Color ColorB)
    {
        float distance = 0;
        distance += Math.Abs(colorA.r - ColorB.r);
        distance += Math.Abs(colorA.g - ColorB.g);
        distance += Math.Abs(colorA.b - ColorB.b);
        distance += Math.Abs(colorA.a - ColorB.a);

        return distance;
    }
    private static Vector3 GetVector3(Stream ms)
    {
        byte[] buffer = new byte[4];
        Vector3 vector3 = new Vector3();

        vector3.x = GetFloat(ms);
        vector3.y = GetFloat(ms);
        vector3.z = GetFloat(ms);

        return vector3;
    }

    private static float GetFloat(Stream ms)
    {
        byte[] buffer = new byte[4];
        
        ms.Read(buffer);
        return BitConverter.ToSingle(buffer);
    }

    public struct D3DMATERIAL7
    {
        public D3DCOLORVALUE diffuse;
        public D3DCOLORVALUE ambient;
        public D3DCOLORVALUE specular;
        public D3DCOLORVALUE emissive;
        public float power;

        public override string ToString()
        {
            return $"Diffuse {diffuse}\nAmbient {ambient}\nSpecular {specular}\nEmissive {emissive}\nPower {power}";
        }
    }

    public struct D3DCOLORVALUE
    {
        float r;
        float g;
        float b;
        float a;

        public override string ToString()
        {
            return $"Red: {r} Green: {g} Blue: {b} Alpha {a}";
        }

        public Color ToColor()
        {
            Color color = new Color
            {
                r = r,
                g = g,
                b = b,
                a = a
            };
            return color;
        }
    }
    public struct Mtl
    {
        public Vector3 diffuse; public float diffuse_a;
        public Vector3 ambient; public float ambient_a;
        public Vector3 specular; public float specular_a;
        public Vector3 emissive; public float emissive_a;
        public float power;

        public override string ToString()
        {
            string res = "Diffuse: " + diffuse.ToString() + " : " + diffuse_a + "\n";
            res += "Ambient: " + ambient.ToString() + " : " + ambient_a + "\n";
            res += "Specular: " + specular.ToString() + " : " + specular_a + "\n";
            res += "Emissive: " + emissive.ToString() + " : " + emissive_a + "\n";
            res += "Power: " + power + "\n";

            return res;
        }
    }
}
