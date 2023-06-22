using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormData 
{

}

public class STORM_DATA
{
    // internal part
    
    // data section
    //static STORM_DATA_API DWORD crc;
    //static STORM_DATA_API const float GAcceleration;
    public uint Name;
    public string FullName;
    // data access
    public uint GetName() { return Name; }

    public STORM_DATA(uint name=0xFFFFFFFF, string fullName=null)
    {
        Name = name;
        FullName = fullName;
    }
}
