using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crc32 = System.UInt32;
using DWORD = System.UInt32;
using WORD = System.UInt16;
//using PrimitiveType = System.UInt32;
using PrimitiveType = StormMesh.D3DPRIMITIVETYPE;

using System.IO;
using System;
using System.Runtime.InteropServices;

public class StormMeshImport
{
    private const string DEFAULT_MATERIAL = "Universal Render Pipeline/Lit";
    private static StormMeshImport instance;
    
    public static Mesh GetMesh(Stream ms,string name)
    {
        if (ms.Length == 16) throw new Exception("Name is lodlist");
        //Debug.Log("Loading mesh " + name);
        StormMesh image = LoadStormMesh(ms, name);
        return ExtractMesh(image,name);
    }

    public static StormMeshImport getInstance()
    {
        if (instance == null) instance = new StormMeshImport();
        return instance;
    }
    public static Material[] GetMaterials(StormMesh image)
    {
        Material[] materials = new Material[image.meshData.n_maters];
        for (int i = 0; i < image.meshData.n_maters; i++)
        {
            materials[i] = GameDataHolder.GetResource<Material>(PackType.MaterialsDB, image.materialData[i].mtl_id);
            materials[i].mainTexture = GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, image.materialData[i].bmp_id);
        }

        return materials;
    }
    public static GameObject GetModel(Stream ms,string name)
    {
        Debug.Log("Loading textured mesh " + name);

        StormMesh image = LoadStormMesh(ms, name);

        GameObject res = new GameObject();
        MeshFilter mf = res.AddComponent<MeshFilter>();
        mf.mesh = ExtractMesh(image);

        MeshRenderer mr = res.AddComponent<MeshRenderer>();

        mr.materials = GetMaterials(image);
        res.name = name;
        return res;
    }


    public static StormMesh LoadStormMesh(Stream ms, string name)
    {
        {
            StormMesh image = new StormMesh();
            ms.Seek(0, SeekOrigin.Begin);

            image.meshData = StormFileUtils.ReadStruct<StormMesh.MeshData>(ms);

            if (image.meshData.type != StormMesh.MeshDataType.MDT_STD) throw new Exception("Incorrect type");
            image.materialData = new StormMesh.MaterialData[image.meshData.n_maters];
            int i;
            for (i = 0; i < image.meshData.n_maters; i++)
            {
                image.materialData[i] = StormFileUtils.ReadStruct<StormMesh.MaterialData>(ms, (int)ms.Position);
            }

            image.facetGroups = new StormMesh.FacetGroup[image.meshData.n_groups];

            int totalIndices = 0;
            for (i = 0; i < image.meshData.n_groups; i++)
            {
                image.facetGroups[i] = StormFileUtils.ReadStruct<StormMesh.FacetGroup>(ms, (int)ms.Position);
                totalIndices += image.facetGroups[i].GetNumIndices();
                //Debug.Log(image.facetGroups[i]);
            }
            image.vertGroups = new StormMesh.VertGroup[image.meshData.n_vertgroups];
            for (i = 0; i < image.meshData.n_vertgroups; i++)
            {
                image.vertGroups[i] = StormFileUtils.ReadStruct<StormMesh.VertGroup>(ms, (int)ms.Position);
            }

            image.indices = new short[totalIndices];
            int j = 0;
            foreach (StormMesh.FacetGroup facetGroup in image.facetGroups)
            {
                for (i = 0; i < facetGroup.GetNumIndices(); i++)
                {
                    image.indices[j++] = StormFileUtils.ReadStruct<short>(ms, (int)ms.Position);
                }
            }


            return image;
        }
    }
    public static GameObject GetModelByIndex(MemoryStream ms,int index)
    {
        return new GameObject();
    }

    public static Mesh ExtractMesh(StormMesh stormMesh,string name="Generated Mesh")
    {
        Mesh mesh = new Mesh();

        int i;
        int[] indices;
        
        Vector3[] vertexes = new Vector3[stormMesh.vertGroups[0].num]; 
        Vector3[] normals = new Vector3[stormMesh.vertGroups[0].num]; 
        Vector2[] uv = new Vector2[stormMesh.vertGroups[0].num];

        ResourcePack rp = GameDataHolder.GetMeshDB();
        Stream stream = rp.GetStreamById(stormMesh.vertGroups[0].Vertices);
        for (i = 0; i < stormMesh.vertGroups[0].num;i++)
        {
            vertexes[i] = StormFileUtils.ReadStruct<Vector3>(stream,(int) stream.Position);
            vertexes[i].y =  0 - vertexes[i].y;
            normals[i] = StormFileUtils.ReadStruct<Vector3>(stream, (int) stream.Position);
            normals[i].y =  0 - normals[i].y; 
            uv[i] = StormFileUtils.ReadStruct<Vector2>(stream, (int) stream.Position);
            uv[i].y = 1 - uv[i].y; //Flip UV coordinate to accomodate with correct texture orientation
        }
        stormMesh.FlipTrianglesFaces();
        mesh.Clear();
        mesh.subMeshCount = stormMesh.facetGroups.Length;
        mesh.name = name;
        mesh.vertices = vertexes;
        mesh.normals = normals;
        mesh.uv = uv;
        int offset =0;

        foreach(StormMesh.FacetGroup facetGroup in stormMesh.facetGroups)
        {
            indices = new int[facetGroup.GetNumIndices()];

            Array.Copy(stormMesh.indices, offset, indices, 0, indices.Length);
            offset += indices.Length;
            mesh.SetIndices(indices, MeshTopology.Triangles, facetGroup.mtl_number,true, facetGroup.start_vert);
        }

        
        return mesh;
    }
}

public class StormMesh
{
    public const int MF2_TEXTURED = 0x0001;
    public const int MF2_TRANSPARENT = 0x0002;
    public const int MF2_ADDITIVE = 0x0004;
    public const int MF2_BUMP = 0x0010;
    public const int MF2_ENVMAP = 0x0020;
    public const int MF2_TWOSIDED = 0x0040;

    public MeshData meshData;
    public MaterialData[] materialData;
    public FacetGroup[] facetGroups;
    public VertGroup[] vertGroups;
    public short[] indices;
    

    public void FlipTrianglesFaces()
    {
        for (int i = 0; i < indices.Length; i += 3)
        {
            short[] buffer = new short[3];
            Array.Copy(indices, i, buffer, 0, 3);
            short tmp = buffer[0];
            buffer[0] = buffer[2];
            buffer[2] = tmp;
            Array.Copy(buffer, 0, indices, i, 3);
        }
    }
    public enum D3DPRIMITIVETYPE
    {
        D3DPT_POINTLIST = 1,
        D3DPT_LINELIST = 2,
        D3DPT_LINESTRIP = 3,
        D3DPT_TRIANGLELIST = 4,
        D3DPT_TRIANGLESTRIP = 5,
        D3DPT_TRIANGLEFAN = 6,
        D3DPT_FORCE_DWORD = 0x7fffffff
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MaterialData
    {         // datafile entry
        public Flags flags;
        public crc32 bmp_id;
        public crc32 mtl_id;
        //union? 4 bytes
        /*[FieldOffset(12)] float parameter;
        [FieldOffset(12)] crc32 bump_id;
        [FieldOffset(12)] crc32 envmap_id;*/
        public float parameter;

        public override string ToString()
        {
            string res = "";

            res += "Flags: " + flags + "\n";
            res += "Bmp: " + bmp_id + "\n";
            res += "Mtl: " + mtl_id + "\n";
            res += "Parms: " + parameter;
            return res;
        }
    }

    public struct VertGroup
    {
        public DWORD FVF;
        public int num;
        public crc32 Vertices;
        int GetVertDataSize()
        {
            //Assert(FVF == D3DFVF_VERTEX);
            return 32 * num;
        }

        public override string ToString()
        {
            string res = "";
            res += "FVF: " + FVF + "\n";
            res += "Num: " + num + "\n";
            res += "Vertices: " + Vertices;

            return res;
        }
        //void* NextPoints(void* p) { return ((char*)p) + GetVertDataSize(); };

    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FacetGroup
    {
        public PrimitiveType prim_type;

        public WORD mtl_number;

        public WORD vertgroup_number;
        public WORD start_vert;
        public WORD n_verts;

        private WORD n_indices;
        private WORD unused;

        public int GetNumIndices() 
        {
            return n_indices;//+(unused==1?0x10000:0);
        }

        void SetNumIndices(int NumIndices)
        {
            n_indices = (ushort) (NumIndices & 0xffff);
            unused = (ushort) ((NumIndices & 0x10000) !=0 ? 1 : 0);
        }

        public override string ToString()
        {
            string res = "";
            res += "Prim type " + prim_type + "\n";
            res += "mtl_number " + mtl_number + "\n";
            res+= "vertgroup_number " + vertgroup_number + "\n";
            res+= "start_vert " + start_vert + "\n";
            res+= "n_verts " + n_verts + "\n"; 
            res+="n_indices " +  n_indices + "\n";
            res+="unused " + unused;
            return res;
        }
    }

    public enum MeshDataType
        {
            MDT_STD,
        }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MeshData
    {
        public MeshDataType type;
        public int n_maters;
        public int n_groups;
        public int n_vertgroups;

        public Sphere b_sphere;   // bounding sphere
        
        //public char[] data;

        int GetSize()
        {
            return 0; //STUB!
                      //sizeof(MeshData) + GetMatersSize() + GetGroupsSize() + GetVertGroupsSize() + GetIndicesSize();
        }

        public override string ToString()
        {
            string res = "Type: " + type + "\n";
            res += "n_maters: " + n_maters + "\n";
            res += "n_groups: " + n_groups + "\n";
            res += "n_vertgroups: " + n_vertgroups + "\n";
            res += "Sphere: " + b_sphere;
            return res;
        }
    }

    public override string ToString()
    {
        string res = ""            ;
        res+= "Indices: " + indices.Length + "\n";
        res += "FacetGroups: " + facetGroups.Length + "\n";
        return res;
    }
}

public struct Vertex
{
    public Vector3 pos;
    public Vector3 norm;
    public Vector2 tc;
    //Vertex(const Vector3f &_pos,const Vector3f &_norm,const Vector2f  &_tc):
  //pos(_pos),norm(_norm),tc(_tc) { }

   // static const DWORD FVF = D3DFVF_VERTEX;
};


public struct Sphere
{
    public Vector3 o;
    public float r;

    public Sphere(Vector3 o, float r)
    {
        this.o = o;
        this.r = r;
    }

    public Vector3 Org()
    {
        return o;
    }
    public float Radius()
    {
        return r;
    }

    public Sphere Operator(Vector3 p) {
      return new Sphere(o+p, r);
    }

    public override string ToString()
    {
        return "Sphere O:" + o + " Radius: " + r;
    }
}


public enum GDType
{
    FGDT_STD,
};
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class GraphData
{
    public GDType type;
};

