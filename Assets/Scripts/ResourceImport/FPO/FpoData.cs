using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crc32 =System.UInt32;
using System.Runtime.InteropServices;

public class FpoData 
{
    public Flags flags;
    public crc32 name;
    public uint num_slots;
    public Position pos;
    public ImageData[] images; //STUB!
    public uint tree_next;
    public uint tree_sub;
    public SlotData[] slots; //STUB

    public override string ToString()
    {
        string res = "";
        res+="Name: " + name.ToString("X8") + "\n";
        res += "Num_Slots: " + num_slots.ToString() +"\n";
        res += "Images: \n";
        
        foreach (ImageData image in images)
        {
            res += image + "\n";
        }
        res += "Tree_next: " + tree_next + "\n";
        res += "Tree_sub: " + tree_sub + "\n";
        res += "Slots: " + slots.Length;
        foreach (SlotData slot in slots)
        {
            res += "Slot: " + slot + "\n";
        }
        return res;
    }
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class ImageData
{
    public crc32 graph;
    public crc32 collision;
    public float radius;
    public Vector3 min, max;

    public override string ToString()
    {
        return graph.ToString("X8");
    }
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class SlotData
{
    public Flags flags;
    public crc32 name;
    public uint slot_id;
    public Position pos;

    public override string ToString()
    {
        string res = "";
        res += "Name: " + name.ToString("X8");
        res += "Id: " + slot_id.ToString();
        return res;

    }
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class Position
{
    public Vector3 org;
    public Vector3 e1;
    public Vector3 e2;

    public Vector3 GetE3()
    {
        return Vector3.Cross(e1, e2);
    }
}

public class FpoGraphData : GraphData
{
    public crc32[] lods; // meshdatas

    public override string ToString()
    {
        string res = "";
        res += "Type:" + type + "\n";
        foreach(crc32 lod in lods)
        {
            res += "Lod: " + lod.ToString("X8")+"\n";
        }
        return res;
    }

    public crc32 GetLod(int index)
    {
        if (index < 0) return default;
        if (index > lods.Length) return default;

        if (lods[index] == 0xFFFFFFFF) return GetLod(index - 1);
        return lods[index];
    }
}
 