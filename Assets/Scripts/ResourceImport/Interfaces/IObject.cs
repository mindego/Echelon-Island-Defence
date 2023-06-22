using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DWORD = System.UInt32;

/*===========================================================================*\
|  Object - refferenced memory queries each Object *vftbl                     |
\*===========================================================================*/

public interface IObject : IRefMem
{
    //IID(0xADC31F52);
    new public const DWORD ID = 0xADC31F52;
    public virtual void Query(int value) { return; }
    //template<class C> C* Query() { return (C*)Query(C::ID); }
};

