using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class UnitImageData : MonoBehaviour
{
    public FpoImport.FPOmesh[] FPOmeshes;
    private StormImage[] stormImages;
    private static Dictionary<string, ImportMeshEditor.meshData> meshInfo;

   [Range(0,3)]
    public int variant, lod;
    private int prevVariant, prevLod;

    private void Start()
    {
        //prevVariant = variant;
        //prevLod = lod;

    }
    private void FixedUpdate()
    {
        updateImage(variant,lod);
    }

    public void setMeshes(FpoImport.FPOmesh[] meshData)
    {
        this.FPOmeshes = meshData;
        //LoadData();
        LoadImages();
        if (UnitImageData.meshInfo == null) LoadData();
        //Debug.Log("Meshes size " + meshData.Length + " for " + name);
    }

    private void LoadData()
    {
        meshInfo = new Dictionary<string, ImportMeshEditor.meshData>();
        stormImages = new StormImage[4];
        XmlSerializer serializer = new XmlSerializer(typeof(ImportMeshEditor.meshData[]));
        FileStream ms = File.Open("Assets/Database/MeshData.xml", FileMode.Open);

        ImportMeshEditor.meshData[] tmpmeshInfo = (ImportMeshEditor.meshData[])serializer.Deserialize(ms);
        for (int i = 0; i < tmpmeshInfo.Length; i++)
        {
            ImportMeshEditor.meshData meshData = tmpmeshInfo[i];

            //        foreach (ImportMeshEditor.meshData meshData in tmpmeshInfo) {
            //Debug.Log(meshData.meshName);
            if (meshData.meshName == null) //TODO! Выяснить, почему пустой meshData
            {
                //Debug.LogError("Empty name " + i + " of " + tmpmeshInfo.Length) ;
                //Debug.Log("Prev: "+ tmpmeshInfo[i - 1]);
                continue;

            }
            else
            {
                meshInfo.Add(meshData.meshName, meshData);
            }
        }
        ms.Close();
        LoadImages();
        return;
    }

    private async void LoadImages()
    {
        Debug.Log("Preloading images: " + FPOmeshes.Length);
        foreach (FpoImport.FPOmesh meshData in FPOmeshes)
        {
            if (meshData.lod == 0)
            {
                Debug.Log("Preloading image: " + meshData.meshName);
                GetMesh(meshData.meshName);
                GetMaterials(meshData.meshName);
            }
            //if (meshData.lod == 0)
            //{
            //    StormImage stormImage = new StormImage();

            //    stormImage.mesh = GetMesh(meshData.meshName);
            //    stormImage.materials = GetMaterials(meshData.meshName);
            //    stormImages[meshData.variant] = stormImage;

            //    Debug.Log($"Adding variant {meshData.variant}");
            //}
            await Task.Yield();
        }
    }
    private Mesh GetMesh(string meshName)
    {
        return GameDataHolder.GetResource<Mesh>(PackType.MeshDB, meshName);
    }

    private Texture2D GetTexture(string textureName)
    {
        Texture2D failed = default;
        if (textureName == null) return failed;

        Texture2D res = GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, textureName);

        if (res == null)
        {
            Debug.Log("Load failed for texture " + textureName);
            return failed;
        }
        return res;
    }

    private Material GenerateMaterial(string materialName,Material templateMaterial,Texture2D texture,int flags=0)
    {
        Material material = new Material(templateMaterial);
        material.mainTexture = texture;
        material.name = materialName;

        if ((flags & (int) ImportMeshEditor.MaterialFlags.MF2_TRANSPARENT) != 0) {
            material.EnableKeyword("_SPECULAR_SETUP");
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.EnableKeyword("_ALPHABLEND_ON");

            material.SetFloat("_WorkflowMode", 0f);
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_DstBlend", 10f); // This makes trasnparent happen
            material.SetFloat("_Mode", 2f); // This says make it transparent
            material.SetFloat("_SrcBlend", 5f);
            material.SetFloat("_ZWrite", 0f);
            material.SetFloat("_Smoothness", 1f);
            material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
            
                
        } else
        {
            material.SetFloat("_Metallic",0.8f);
            material.SetFloat("_Smoothness", 0.5f);
        }
        return material;
    }

    private Material GenerateMaterial(ImportMeshEditor.materialData materialData)
    {
        //       string filename = "Assets/Materials/" + materialData.meshMaterial + ".mat";
        //if (!File.Exists(filename)) return default;
        //Material tmpMaterialTemplate = AssetDatabase.LoadAssetAtPath<Material>(filename);
        Material tmpMaterialTemplate = GameDataHolder.GetResource<Material>(PackType.MaterialsDB, materialData.meshMaterial);

        //Texture2D texture = GetTexture(materialData.meshTexture == "ha_bf1" ? "ha-bf1": materialData.meshTexture);
        Texture2D texture = GetTexture(materialData.meshTexture);
        string materialName = (texture == null) ? tmpMaterialTemplate.name : tmpMaterialTemplate.name + "#" + texture.name;
        return GenerateMaterial(materialName, tmpMaterialTemplate, texture,materialData.flags);
    }

    private Material[] GetMaterials(string meshName)
    {
        if (UnitImageData.meshInfo == null) LoadData();
            ImportMeshEditor.materialData[] mtls = new ImportMeshEditor.materialData[0];
        if (meshName != null)
        {
            //mtls = meshInfo[meshName].meshMaterials;
            mtls = meshInfo.ContainsKey(meshName) ? meshInfo[meshName].meshMaterials : mtls;
        }

        Material[] materials = new Material[mtls.Length];
        //Texture2D texture;
        for (int i = 0; i < mtls.Length; i++)
        {
            materials[i] = GenerateMaterial(mtls[i]);
        }
        return materials;
    }

    public void updateImage(int newVariant,int newLod,bool force=false)
    {
        if ((newVariant == prevVariant) & (newLod == prevLod) & !force) return;
        //if (meshInfo == null) LoadData();
        Debug.Log($"Updating from {prevVariant} to {newVariant}");
        prevLod = newLod;
        prevVariant = newVariant;

        string meshNameEnum = "No mesh found";
        string meshName = "error mesh";
        foreach (FpoImport.FPOmesh meshData in FPOmeshes)
        {
            if (meshData.meshName != null) meshNameEnum = meshData.meshName;
            if (meshData.lod == lod & meshData.variant == variant) meshName = meshData.meshName;
        }
        //gameObject.name = meshName;
        string[] nodetree = meshNameEnum.Split('_');
        gameObject.name = nodetree.Length > 2 ? nodetree[nodetree.Length - 3] : meshNameEnum;

        Mesh mesh = GetMesh(meshName);

        //if (stormImages[newVariant].mesh == null)
        //{
        //    BroadcastMessage("ImageUpdateReceiver", newVariant);
        //    return;
        //}
        //Mesh mesh = stormImages[newVariant].mesh;

        MeshFilter mf;
        if (!TryGetComponent<MeshFilter>(out mf))
        {
            mf = gameObject.AddComponent<MeshFilter>();
                
        }

        MeshRenderer mr;
        if (!TryGetComponent<MeshRenderer>(out mr))
        {
            mr = gameObject.AddComponent<MeshRenderer>();
        }

        //Collider mc;
        if (TryGetComponent<Collider>(out Collider currmc)) Destroy(currmc);
        if (TryGetComponent<Renderer>(out Renderer r))
        {
            //BoxCollider tmpmc = gameObject.AddComponent<BoxCollider>();
            //tmpmc.size = r.localBounds.size;
            //SphereCollider tmpmc = gameObject.AddComponent<SphereCollider>();
            //tmpmc.radius = r.localBounds.size.magnitude / 2;

            MeshCollider tmpmc = gameObject.AddComponent<MeshCollider>();
            tmpmc.sharedMesh = mesh;
            tmpmc.convex = true;

        }

        mf.mesh = mesh;
        //mr.materials = stormImages[newVariant].materials;
        mr.materials = GetMaterials(meshName);

        //string[] nodetree = mesh.name.Split('_');
        //gameObject.name = nodetree.Length > 2 ? nodetree[nodetree.Length - 3] : "Unknown mesh";

        //if (!Application.isEditor) BroadcastMessage("ImageUpdateReceiver",newVariant);
        BroadcastMessage("ImageUpdateReceiver", newVariant);
        //Debug.Log($"{name} switching to {meshName} variant {newVariant}");
    }

    private void ImageUpdateReceiver(int setVariant)
    {
        Debug.Log($"message received by {name} set variant {setVariant}");

        //updateImage(setVariant, 0, false);
        variant = setVariant;
    }

    public struct StormImage
    {
        public Mesh mesh;
        public Material[] materials;
        public int flags;
    }
}
