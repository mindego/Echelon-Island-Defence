using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using crc32value = System.UInt32;

public struct TerrainCfg
{
    crc32value material;
    crc32value state;
    crc32value features;
    crc32value unused;
};

public struct SurfaceDesc
{
    public crc32value texture;
    public crc32value material;
    public crc32value rs;
    float unused;

    public override string ToString()
    {
        return "Surface texture " + texture.ToString("X8") ;
    }
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TerrainMtlCfg
{
    public crc32value main_rs;
    public crc32value blend_rs;

    public SurfaceDesc DetailSurface;
    public crc32value detail_rs;

    public SurfaceDesc WaterSurface;
    public crc32value water_rs;

    public float WavePeriod;

    const int MaxSurfaces = 32; //static
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0)]
    public SurfaceDesc[] GroundSurfaces; //32
};

