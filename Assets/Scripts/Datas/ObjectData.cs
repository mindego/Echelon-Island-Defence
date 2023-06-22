using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DWORD = System.UInt32;
public class OBJECT_DATA : STORM_DATA
{
    // маски
    const uint OF_CLASS_MASK = 0x000000FF;
    const uint OF_FLAGS_MASK = 0x0000FF00;
    // классы подобъектов =
    const uint OC_STATIC = 0x00000010;
    const uint OC_HANGAR = 0x00000011;
    const uint OC_SFG = 0x00000012;
    const uint OC_VEHICLE = 0x00000020;
    const uint OC_CRAFT = 0x00000040;
    public const uint OC_AIRSHIP = 0x00000080;
    const uint OC_SEASHIP = 0x00000081;

    public string Description;
    public string DescriptionShort;
    public uint Flags;
    public CampaignDefines.SideTable Side; //int
    public int UnitDataIndex;
    public float SensorsRange;
    public float SensorsVisibility;
    public float CriticalDamagedTime;
    public bool SpawnShadow;
    //SUBOBJ_DATA* RootData;
    //TLIST<LAYOUT_DATA> Layouts;
    //Tab<AnimationPackage*> myAnimations;
    //Tab<crc32> myLinkedSubObj;
    public OBJECT_DATA pOtherSideData;
    // data access
    public void SetFlag(DWORD Flag) { Flags |= Flag; }
    public void ClearFlag(DWORD Flag) { Flags &= ~Flag; }
    DWORD GetFlag(DWORD Flag) { return (Flags&Flag); }
    public DWORD GetClass() {return (Flags&OF_CLASS_MASK); }
    public bool IsClass(DWORD Flag) { return ((Flags&Flag) == 0)? false:true; }

}
