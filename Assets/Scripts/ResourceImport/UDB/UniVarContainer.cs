using System;
using DWORD = System.UInt32;

/// <summary>
/// Служебная структура для UniVarContainer
/// </summary>
public struct UniVarContainerItem
{
    public DWORD mClassId;
    public DWORD mMemId;
    public DWORD mCode;
    public iUnifiedVariable mpVar;
    public string mpName;

    public void Set(UniVarContainerItemSave pSrc, string pBase)
    {
        mClassId = pSrc.mClassId;
        mMemId = pSrc.mMemId;
        mpVar = null;
        //int l = StrLen(pBase + pSrc->mOffset) + 1;
        //mpName = new char[l];
        //MemCpy(mpName, pBase + pSrc->mOffset, l);
        mpName = String.Empty; //STUB!
        Storm.CRC32 crc = new Storm.CRC32();
        mCode = crc.HashString(mpName);
    }
    public void Set(DWORD ClassId, DWORD Code, iUnifiedVariable pVar, string pName, DWORD MemId = 0)
    {
        mClassId = ClassId;
        mMemId = MemId;
        mCode = Code;
        mpVar = pVar;
        //int l = StrLen(pName) + 1;
        //mpName = new char[l];
        //MemCpy(mpName, pName, l);
        mpName = pName;

    }
    //public void Free() { if (mpName != 0) delete[] mpName; mpName = 0; }
}

/// <summary>
/// Служебная структура хранения данных в файле ресурса
/// </summary>
public struct UniVarContainerItemSave
{
    public DWORD mClassId;
    public DWORD mMemId;
    public DWORD mOffset;
    public void Set(UniVarContainerItem pSrc, DWORD Off)
    {
        mClassId = pSrc.mClassId;
        mMemId = pSrc.mMemId;
        mOffset = Off;
    }
};
/// <summary>
/// UniVarContainer - реализация iUnifiedVariableContainer
/// </summary>
public class UniVarContainer : iUniVarParent, iUnifiedVariableContainer
{
    private int mCounter;
    private iUniVarParent mpParent;
    private iUniVarMemManager mpMemMgr;
    private DWORD mMemID;
    private int mArraySize;
    private UniVarContainerItem[] mpArray;
    private bool mIsDeleting;
    private void ReAlloc(int srcl1, int dst2, int src2)
    {

    }
    private DWORD GetHandle(DWORD Code)
    {
        if (mpArray[0].mCode == Code) return 1;
        if (mpArray[mArraySize - 1].mCode == Code) return (DWORD) mArraySize;

        int _1 = 0, _2 = mArraySize - 1;
        while ((_2 - _1) > 1)
        {
            int _3 = (_1 + _2) >> 1;
            if (mpArray[_3].mCode == Code) return (DWORD) _3 + 1;
            if (mpArray[_3].mCode < Code) _1 = _3; else _2 = _3;
        }
        return 0;
    }

    public UniVarContainer(iUniVarParent par, DWORD memid) : this (1,par,par.GetMemManager(),memid,0,new UniVarContainerItem[0],false)
    {
        if (memid == 0) return;

        byte[] data = mpMemMgr.GetDataByID(memid);
        mArraySize = BitConverter.ToInt32(data);
        if (mArraySize <= 0) return;

        mpArray = new UniVarContainerItem[mArraySize];
        for (int i =0;i<mArraySize;i++)
        {
            UniVarContainerItemSave tmp = StormFileUtils.ReadStruct<UniVarContainerItemSave>(data,4+i*3*4); //4 - 3*4 - размер структуры
            mpArray[i] = new UniVarContainerItem();
            mpArray[i].Set(tmp,String.Empty);
        }
    }

     ~UniVarContainer()
    {
        if (mArraySize <= 0) return;
        mpArray = null;
        mArraySize = 0;
    }

    public UniVarContainer(int mCounter, iUniVarParent mpParent, iUniVarMemManager mpMemMgr, uint mMemID, int mArraySize, UniVarContainerItem[] mpArray, bool mIsDeleting)
    {
        this.mCounter = mCounter;
        this.mpParent = mpParent;
        this.mpMemMgr = mpMemMgr;
        this.mMemID = mMemID;
        this.mArraySize = mArraySize;
        this.mpArray = mpArray;
        this.mIsDeleting = mIsDeleting;
    }

    //~UniVarContainer();

    public void AddRef()
    {
        mCounter++;
    }

    public iUnifiedVariable CreateVariableByName(int ClassID, string name)
    {
        throw new NotImplementedException("И не надо!");
    }

    public bool Delete()
    {
        throw new NotImplementedException("И не надо!");
    }

    public bool ExportToFile(string filename)
    {
        throw new NotImplementedException();
    }

    public uint GetClassId()
    {
        return iUnifiedVariableContainer.ID;
    }

    public uint GetHandleByName(string Name)
    {
        Storm.CRC32 crc = new Storm.CRC32();
        return crc.HashString(Name);
    }

    public iUniVarMemManager GetMemManager()
    {
        return mpMemMgr;
    }

    public string GetName(string lVar, iUnifiedVariable rVar)
    {
        throw new NotImplementedException();
    }

    public string GetName(string value)
    {
        throw new NotImplementedException();
    }

    public string GetNameByHandle(string buffer, uint Handle)
    {
        if (Handle > mArraySize) return String.Empty;
        return mpArray[Handle - 1].mpName;

    }

    public int GetNameLength(iUnifiedVariable pVar)
    {
        // ищем запись для этой переменной
        DWORD i;
        for (i = 0; i < mArraySize; i++)
        {
            if (mpArray[i].mpVar == pVar)
            {
                // возвращаем длину своего имени + '\' + длину имени этой переменной
                return (GetNameLength() + 1 + GetNameLengthByHandle(i + 1));
            }
        }
        // не наша переменная - возвращаем 0
        return 0;
    }

    public int GetNameLength()
    {
        return mpParent.GetNameLength(this);
    }

    public int GetNameLengthByHandle(uint Handle)
    {
        return (Handle <= mArraySize ? mpArray[Handle - 1].mpName.Length : 0);
    }

    public uint GetNextHandle(uint Handle)
    {
        return Handle < mArraySize ? Handle + 1 : 0;
    }

    public uint GetSize()
    {
        return (uint) mArraySize;
    }

    public int GetSizeByHandle(uint Handle)
    {
        return (Handle <= mArraySize ? (int) mpMemMgr.GetSizeByID(mpArray[Handle - 1].mMemId) : 0);
    }

    public iUnifiedVariable CreateByClassID(int ClassId, iUniVarParent pParent, DWORD MemId)
    {
        switch ((uint)ClassId)
        {
            case iUnifiedVariableInt.ID: return new UniVarInt(pParent, MemId);
            case iUnifiedVariableFloat.ID: return new UniVarFloat(pParent, MemId);
            case iUnifiedVariableVector.ID: return new UniVarVector(pParent, MemId);
            case iUnifiedVariableString.ID: return new UniVarString(pParent, MemId);
            //case iUnifiedVariableBlock.ID: return new UniVarBlock(pParent, MemId);
            case iUnifiedVariableReference.ID: return new UniVarReference(pParent, MemId);
            case iUnifiedVariableContainer.ID: return new UniVarContainer(pParent, MemId);
            //case iUnifiedVariableArray.ID: return new UniVarArray(pParent, MemId);
            default: return null;
        }
    }
 
    public iUnifiedVariable GetVariableByHandle(uint Handle)
    {
        if (Handle == 0 || (Handle > mArraySize)) return null;
        // если переменной не создано
        //UniVarContainerItem dst = mpArray + (Handle - 1);
        UniVarContainerItem dst = mpArray[Handle - 1];
        if (dst.mpVar == null)
        {
            dst.mpVar = CreateByClassID((int) dst.mClassId, this, dst.mMemId);
            if (dst.mpVar != null) AddRef();
        }
        else
        {
            dst.mpVar.AddRef();
        }
        return dst.mpVar;

    }

    public iUnifiedVariable GetVariableByName(string name, uint crc)
    {
        throw new NotImplementedException();
    }

    public bool ImportFromFile(string filename)
    {
        throw new NotImplementedException();
    }

    public bool IsReadOnly()
    {
        return mpMemMgr.IsReadOnly();
    }

    public void OnDelete(iUnifiedVariable var)
    {
        throw new NotImplementedException();
    }

    public void OnRelease(iUnifiedVariable var, uint MemId)
    {
        throw new NotImplementedException();
    }

    public void Query(int ClassID)
    {
        throw new NotImplementedException();
    }

    public int Release()
    {
        throw new NotImplementedException();
    }

    public bool Rename(string pSrcName, string pDstName)
    {
        throw new NotImplementedException();
    }
}

