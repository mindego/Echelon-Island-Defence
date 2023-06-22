using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SUBOBJ_DATA : STORM_DATA
{
    // маски
    const uint SF_CLASS_MASK = 0x000000FF;
    const uint SF_FLAGS_MASK = 0x0000FF00;
    // классы подобъектов
    const uint SC_SUBOBJ = 0x00000000;
    const uint SC_TURRET = 0x00000001;
    const uint SC_CRAFT_PART = 0x00000002;
    const uint SC_WEAPON_SLOT = 0x00000003;
    const uint SC_DEBRIS_PART = 0x00000004;
    const uint SC_RADAR = 0x00000005;
    const uint SC_FOOT = 0x00000006;
    const uint SC_DETACHED = 0x00000007;
    const uint SC_HANGAR = 0x00000008;
    const uint SC_SFG = 0x00000009;
    // флаги подобъекта
    const uint SF_DETACHED = 0x00000100;
    const uint SF_CRITICAL = 0x00000200;

    /*    SUBOBJ_DATA(const char*);
        virtual void ProcessToken(READ_TEXT_FILE&,const char*);
        virtual void Reference(const SUBOBJ_DATA* ref);
        virtual void MakeLinks();
        // data section
        const char* Description;
        const char* DescriptionShort;
        unsigned int Flags;
        const char* FileName;
        unsigned int CodedFileName;
        float Armor;
        TLIST<SUBOBJ_DATA> SubobjDatas;
        Tab<AnimationPackage*> myAnimations;
        DEBRIS_DATA* Debris;
        DEBRIS_DATA* DetachedDebris;
        DEBRIS_DATA* SubobjDebris;
        float DeltaY;
        int UnitDataIndex;
        // data access
        void SetFlag(unsigned int Flag) { Flags |= Flag; }
        void ClearFlag(unsigned int Flag) { Flags &= ~Flag; }
        unsigned int GetFlag(unsigned int Flag) const { return (Flags&Flag); }
    unsigned int GetClass()                  const { return (Flags&SF_CLASS_MASK); }
        static STORM_DATA_API SUBOBJ_DATA*  __cdecl GetByName(const char *Name,bool MustExist=true);
    static STORM_DATA_API SUBOBJ_DATA*  __cdecl GetByCode(unsigned int Code,bool MustExist=true);
    static STORM_DATA_API SUBOBJ_DATA*  __cdecl GetFirstItem();
    static STORM_DATA_API int __cdecl nItems();
    */

    //virtual void ProcessToken(READ_TEXT_FILE&,const char*);
    //virtual void Reference(const SUBOBJ_DATA* ref);
    //public virtual void MakeLinks();
    // data section
    public string Description;
    public string DescriptionShort;
    public uint Flags;
    public string FileName;
    uint CodedFileName;
    public float Armor;
    //TLIST<SUBOBJ_DATA> SubobjDatas;
    //Tab<AnimationPackage*> myAnimations;
    public DEBRIS_DATA Debris;
    public DEBRIS_DATA DetachedDebris;
    public DEBRIS_DATA SubobjDebris;
    public float DeltaY;
    public int UnitDataIndex;
    // data access
    public void SetFlag(uint Flag) { Flags |= Flag; }
    public void ClearFlag(uint Flag) { Flags &= ~Flag; }
    public uint GetFlag(uint Flag) { return (Flags&Flag); }
    public uint GetClass() { return (Flags&SF_CLASS_MASK); }
}
