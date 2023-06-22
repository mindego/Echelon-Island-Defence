using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DWORD = System.UInt32;

/*===========================================================================*\
|  Multireferenced deleteble object                                           |
\*===========================================================================*/

public interface IRefMem : IMemory
{
    //IID(0x0E1095B3);
    new public const DWORD ID = 0x0E1095B3;
    public void AddRef();

    public int RefCount() { AddRef(); return Release(); }
};


