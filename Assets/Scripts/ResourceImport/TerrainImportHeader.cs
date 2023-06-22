using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class TerrainImportHeader
{
    public static T_HEADER GetHeader(string mapName)
    {
        T_HEADER header = new T_HEADER(0,0,0);
        string filename = TerrainDefs.MAPDIR + mapName + ".hd";
        if (!File.Exists(filename)) 
        { 
            Debug.Log("No file found " + filename);
            return header;
        }

        FileStream stream = File.OpenRead(filename);
        byte[] buffer = new byte[4];
        stream.Read(buffer);
        header.SizeXBPages = BitConverter.ToInt32(buffer);
        
        stream.Read(buffer);
        header.SizeZBPages = BitConverter.ToInt32(buffer);

        float[] tmpDir = new float[3];
        for (int i=0; i< TerrainDefs.T_MAX_LIGHTMAPS;i++)
        {
            stream.Read(buffer);
            
            for (int j=0;j<3;j++)
            {
                stream.Read(buffer);
                tmpDir[j] = BitConverter.ToInt32(buffer);

            }
            Vector3 dir = new Vector3(tmpDir[0], tmpDir[1], tmpDir[2]);
            stream.Read(buffer);
            int isValid = BitConverter.ToInt32(buffer); 

            header.light_maps[i] = new T_LightDesc(dir,isValid);
        }

        stream.Read(buffer);
        header.nMaterials = BitConverter.ToInt32(buffer);
        return header;
    }
}
