using System;
using System.IO;
using DWORD = System.UInt32;

/// <summary>
/// UniVarInt - реализация iUnifiedVariableInt
/// </summary>
public class UniVarInt : iUnifiedVariableInt
{

    private int mCounter;
    private iUniVarParent mpParent;
    private DWORD mMemID;

    public UniVarInt(iUniVarParent par, DWORD memid) : this(1, par, memid) { }

    public UniVarInt(int mCounter, iUniVarParent mpParent, DWORD mMemID)
    {
        this.mCounter = mCounter;
        this.mpParent = mpParent;
        this.mMemID = mMemID;
    }
    
    public void AddRef()
    {
        mCounter++;
    }

    public bool Delete()
    {
        if (mCounter > 1 || mpParent.IsReadOnly()) return false;
        mpParent.GetMemManager().Free(mMemID);
        mpParent.OnDelete(this);
        //delete this;
        return true;
    }

    public bool ExportToFile(string filename)
    {
        TextWriter Out = File.CreateText(filename);
        //if (!Out.) return false;

        Out.Write(GetValue());
        return true;
    }

    public uint GetClassId()
    {
        return iUnifiedVariableInt.ID;
    }

    public string GetName(string value)
    {
        return mpParent.GetName(value, this);
    }

    public int GetNameLength()
    {
        return mpParent.GetNameLength(this);
    }

    public int GetValue()
    {
        if (mMemID == 0) return 0;
        byte[] data = mpParent.GetMemManager().GetDataByID(mMemID);
        if (data.Length < 4) return 0;
        return BitConverter.ToInt32(data);
    }

    public bool ImportFromFile(string filename)
    {
        return false;
    }

    public int Release()
    {
        if (--mCounter > 0) return mCounter;
        mpParent.OnRelease(this, mMemID);
        //delete this;
        return 0;
    }

    public int SetValue(int value)
    {
        if (mpParent.IsReadOnly()) return GetValue();
        return value;
    }
}

/// <summary>
/// UniVarDB - реализация
/// </summary>
public class UniVarDB :  iUnifiedVariableDB,  iUniVarParent
{
    private iUniVarMemManager mpMemMgr;
    private UniVarDBItem mpRootData;
    private iUnifiedVariable mpRoot;
    private DWORD mCounter;
    
  public UniVarDB(string filename, iUniVarMemManager pMemMgr)
    {
        mpMemMgr = pMemMgr;
        //mpRootData = (UniVarDBItem*)mpMemMgr->GetPtrByID(1);
        mpRootData = StormFileUtils.ReadStruct<UniVarDBItem>(mpMemMgr.GetDataByID(1));
        mpRootData.Set(0xFFFFFFFF, 0);
    }

    public void AddRef()
    {
        mCounter++;
    }

    public iUnifiedVariable CreateRoot(int ClassID)
    {
        throw new NotImplementedException();
    }

    public iUnifiedVariable CreateVariableByName(int ClassID, string name)
    {
        throw new NotImplementedException();
    }

    public bool Delete()
    {
        return false;
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
        throw new NotImplementedException();
    }

    public iUniVarMemManager GetMemManager()
    {
        throw new NotImplementedException();
    }

    public string GetName(string value)
    {
        throw new NotImplementedException();
    }

    public string GetName(string lVar, iUnifiedVariable rVar)
    {
        throw new NotImplementedException();
    }

    public string GetNameByHandle(string buffer, uint Handle)
    {
        throw new NotImplementedException();
    }

    public int GetNameLength()
    {
        throw new NotImplementedException();
    }

    public int GetNameLength(iUnifiedVariable var)
    {
        throw new NotImplementedException();
    }

    public int GetNameLengthByHandle(uint Handle)
    {
        throw new NotImplementedException();
    }

    public uint GetNextHandle(uint Handle)
    {
        throw new NotImplementedException();
    }

    public iUnifiedVariable GetRoot()
    {
        throw new NotImplementedException();
    }

    public uint GetRootDataSize()
    {
        throw new NotImplementedException();
    }

    public uint GetSize()
    {
        throw new NotImplementedException();
    }

    public int GetSizeByHandle(uint Handle)
    {
        throw new NotImplementedException();
    }

    public iUnifiedVariable GetVariableByHandle(uint Handle)
    {
        throw new NotImplementedException();
    }

    public iUnifiedVariable GetVariableByName(string name, uint crc = uint.MaxValue)
    {
        throw new NotImplementedException();
    }

    public bool ImportFromFile(string filename)
    {
        throw new NotImplementedException();
    }

    public bool IsReadOnly()
    {
        throw new NotImplementedException();
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
        if (--mCounter > 0) return (int) mCounter;
        return 0;

    }

    public bool Rename(string pSrcName, string pDstName)
    {
        throw new NotImplementedException();
    }

    public bool SaveToFile(string filename)
    {
        throw new NotImplementedException();
    }
    //public ~UniVarDB();

}

/// <summary>
/// UniVarDBItem - служебная структура
/// </summary>
public struct UniVarDBItem
{
    public DWORD mClassId;
    public DWORD mMemId;
    public void Set(DWORD c, DWORD m) { mClassId = c; mMemId = m; }
};



