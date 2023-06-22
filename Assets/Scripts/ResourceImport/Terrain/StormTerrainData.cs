using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class TERRAIN_DATA
{
    public T_HEADER Header;
    public T_MAT Materials;

    /*   TerrainFile<T_SQUARE, SQUARES_PAGE_SIZE> Squares;
       TerrainFile<T_BOX, BOXES_PAGE_SIZE> Boxes;
       TerrainFile<T_VBOX, VBOXES_PAGE_SIZE> VBoxes;*/
    //TerrainFile<T_LIGHT, LIGHT_PAGE_SIZE> Lights[2];
    //TerrainFile<T_LIGHT, LIGHT_PAGE_SIZE>[] Lights; //2
    //public TerrainFileSQ Squares;
    public TerrainFile<T_SQUARE> Squares;
    public TerrainFileBX Boxes;
    //public TerrainFileVB VBoxes;
    public TerrainFile<T_VBOX> VBoxes;

    private string ext;
    private string name;

    public TERRAIN_DATA(string name)
    {
        this.name = name;
        //Squares = new TerrainFileSQ();
        Squares = new TerrainFile<T_SQUARE>();
        VBoxes = new TerrainFile<T_VBOX>();
    }

    public bool Open(string name) {
        this.name = name;
        return OpenHdr();
    }
    
    public bool OpenVb(bool OpenOld, bool CanWrite)
    {
        int Bx = Header.SizeXBPages, Bz = Header.SizeZBPages;
        ext = ".vb";

        if (!VBoxes.Open(TerrainDefs.MAPDIR + name + ext, OpenOld, CanWrite, Bx * TerrainDefs.BPAGE_IN_VBPAGES, Bz * TerrainDefs.BPAGE_IN_VBPAGES,TerrainDefs.VBOXES_PAGE_SIZE))
        {
            return false;
        }
        return true;
        //if (V)
        /*
         * bool TERRAIN_DATA::OpenVb( bool OpenOld, bool CanWrite ) {
  int Bx=Header.SizeXBPages,Bz=Header.SizeZBPages;

  _strcpy(Ext,".vb");
  if (!VBoxes.Open(Name, OpenOld, CanWrite, Bx*BPAGE_IN_VBPAGES, Bz*BPAGE_IN_VBPAGES )) {
    VBoxes.Close();
    return false;
  }
  return true;
}

         */
    }

    public bool OpenBx (bool OpenOld,bool CanWrite)
    {
        int Bx = Header.SizeXBPages, Bz = Header.SizeZBPages;
        ext = ".bx";

        //if (!Boxes.Open)
        //{
            
        //    return false;
        //}
        return true;
    }
    /*
     * bool TERRAIN_DATA::OpenBx( bool OpenOld, bool CanWrite ) {
int Bx=Header.SizeXBPages,Bz=Header.SizeZBPages;

_strcpy(Ext,".bx");
if (!Boxes.Open(Name, OpenOld, CanWrite, Bx, Bz)) {
Boxes.Close();
return false;
}
return true;
}

     * */
    public bool OpenSQ(bool OpenOld, bool CanWrite)
    {
        int Bx = Header.SizeXBPages, Bz = Header.SizeZBPages;
        ext = ".sq";
        if (!Squares.Open(TerrainDefs.MAPDIR + name + ext,OpenOld,CanWrite,Bx*TerrainDefs.SQUARES_IN_BOX,Bz*TerrainDefs.SQUARES_IN_BOX, TerrainDefs.SQUARES_PAGE_SIZE)) {
            //Squares.Close();
            return false;
        }
        return true;
    }
    public bool OpenHdr()
    {
        ext = ".hd";

        Header = new T_HEADER(0, 0, 0);
        string filename = TerrainDefs.MAPDIR + name + ext;
        if (!File.Exists(filename))
        {
            Debug.Log("No file found " + filename);
            return false;
        }

        FileStream stream = File.OpenRead(filename);
        byte[] buffer = new byte[4];
        stream.Read(buffer);
        Header.SizeXBPages = BitConverter.ToInt32(buffer);

        stream.Read(buffer);
        Header.SizeZBPages = BitConverter.ToInt32(buffer);

        float[] tmpDir = new float[3];
        for (int i = 0; i < TerrainDefs.T_MAX_LIGHTMAPS; i++)
        {

            for (int j = 0; j < 3; j++)
            {
                stream.Read(buffer);
                tmpDir[j] = BitConverter.ToInt32(buffer);

            }
            Vector3 dir = new Vector3(tmpDir[0], tmpDir[1], tmpDir[2]);
            //Debug.Log(dir);

            stream.Read(buffer);
            int isValid = BitConverter.ToInt32(buffer);
            //Debug.Log(isValid);

            Header.light_maps[i] = new T_LightDesc(dir, isValid);
        }

        stream.Read(buffer);
        Header.nMaterials = BitConverter.ToInt32(buffer);

        Materials = new T_MAT(32);
        for (int i=0;i<Header.nMaterials;i++)
        {
            stream.Read(buffer);
            Materials.SurType[i] = BitConverter.ToInt32(buffer);
        }
        return true;
    }

    public override string ToString()
    {
        return Header.SizeXBPages + "x" + Header.SizeZBPages + " nMaterials " + Header.nMaterials;
    }
}

/*public class TerrainFile<T,Z>
{
    private FileStream file;
    private Pager<T,Z> pager;
    private string tfAccessMode;
    public bool Open(string name,bool OpenOld,bool Write,int SizeX,int SizeZ)
    {
        file=File.OpenRead(name);
        byte[] data = File.ReadAllBytes(name);
        
        pager = new Pager<T, Z>(null, SizeX, SizeZ);
        return true;
    }
}*/

public class TerrainFile<T>
{
    //private FileStream file;
    //private Stream file;
    public Pager<T, T> pager;


    public bool Open(string name, bool OpenOld, bool Write, int SizeX, int SizeZ,int dim)
    {
        if (!File.Exists(name))
        {
            Debug.Log("Terrainfile not found: " + name);
            return false;
        }
        MemoryStream ms = new MemoryStream();
        ms.Write(File.ReadAllBytes(name));

        //file = File.OpenRead(name);
        //pager = new Pager<T, T>(file, SizeX, SizeZ, dim);
        pager = new Pager<T, T>(ms, SizeX, SizeZ, dim);

        return true;
    }
    public bool Open(string name, bool OpenOld, bool Write, int SizeX, int SizeZ)
    {
        return Open(name, OpenOld, Write, SizeX, SizeZ, 0);
    }
}

public class TerrainFileVB
{
    private FileStream file;
    public Pager<T_VBOX, T_VBOX> pager;

    public bool Open(string name, bool OpenOld, bool Write, int SizeX, int SizeZ) {
        file = File.OpenRead(name);
        pager = new Pager<T_VBOX, T_VBOX>(file, SizeX, SizeZ, TerrainDefs.VBOXES_PAGE_SIZE);
        return true;
    }
}
public class TerrainFileBX
{
    private FileStream file;
    public Pager<T_BOX,T_BOX> pager;


}
public class TerrainFileSQ
{
    private FileStream file;
    public Pager<T_SQUARE, T_SQUARE> pager;

    public bool Open(string name, bool OpenOld, bool Write, int SizeX, int SizeZ)
    {
        //byte[] data = File.ReadAllBytes(name);
        file = File.OpenRead(name);
        
        //T_SQUARE[] data = Convert(file,SizeX,SizeZ);
        pager = new Pager<T_SQUARE, T_SQUARE>(file,SizeX,SizeZ,TerrainDefs.SQUARES_PAGE_SIZE);
        return true;
    }

    public void Close()
    {
        //pager.SetData(0, 0, 0);
        file.Close();

    }
    private T_SQUARE[] Convert(FileStream data,int SizeX,int SizeZ)
    {
        /*        int Bx = Header.SizeXBPages, Bz = Header.SizeZBPages;

                _strcpy(Ext, ".sq");
                if (!Squares.Open(Name, OpenOld, CanWrite, Bx * SQUARES_IN_BOX, Bz * SQUARES_IN_BOX))
                {
                    Squares.Close();
                    return false;
                }
                return true;*/
        T_SQUARE[] squares = new T_SQUARE[SizeX * SizeZ];
        byte[] HeightBytes = new byte[2];
        byte[] FlagBytes = new byte[2];
        for (int Bz=0;Bz<SizeZ;Bz++)
        {
            for (int Bx=0;Bx<SizeX;Bx++)
            {
                for (int Pz=0;Pz<TerrainDefs.SQUARES_PAGE_SIZE;Pz++)
                {
                    for (int Px=0;Px<TerrainDefs.SQUARES_PAGE_SIZE;Px++)
                    {
                        data.Read(HeightBytes);
                        short Height = BitConverter.ToInt16(HeightBytes);
                        ushort Flag = BitConverter.ToUInt16(FlagBytes);
                        int squareIndex = (Bx + Bz * SizeZ) + (Px + Pz * TerrainDefs.SQUARES_PAGE_SIZE);
                        Debug.Log((squareIndex, squares));
                        squares[squareIndex] = new T_SQUARE(Height, Flag);
                        //if (squareIndex > 100) return default;
                    }
                }
            }
        }
        return squares;
    }
}