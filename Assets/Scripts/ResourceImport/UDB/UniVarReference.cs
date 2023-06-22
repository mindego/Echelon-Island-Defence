using System;
using System.IO;
using DWORD = System.UInt32;
/// <summary>
/// UniVarReference - реализация iUnifiedVariableReference
/// </summary>
public class UniVarReference : iUnifiedVariableReference
{
    private int mCounter;
    private iUniVarParent mpParent;
    private DWORD mMemID;

    public UniVarReference(iUniVarParent par, DWORD memid) : this(1, par, memid) { }

    public UniVarReference(int mCounter, iUniVarParent mpParent, uint mMemID)
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
        byte[] data = mpParent.GetMemManager().GetDataByID(mMemID);
        Out.Write(data);
        Out.Close();
        return true;
    }

    public uint GetClassId()
    {
        return iUnifiedVariableReference.ID;
    }

    public string GetName(string value)
    {
        return mpParent.GetName(value, this);
    }

    public int GetNameLength()
    {
        return mpParent.GetNameLength(this);
    }

    public iUnifiedVariable GetReference()
    {
        int l = (int) mpParent.GetMemManager().GetSizeByID(mMemID);
        if (l == 0) return null;

        string name = mpParent.GetMemManager().GetPtrByID(mMemID);
        return mpParent.GetVariableByName(name, 0xFFFFFFFF);
    }

    public void GetReferenceName(out string dst)
    {
        dst = String.Empty;
        int l = (int) mpParent.GetMemManager().GetSizeByID(mMemID);
        if (l > 0) dst = mpParent.GetMemManager().GetPtrByID(mMemID);

    }

    public int GetReferenceNameLength()
    {
        return (int) mpParent.GetMemManager().GetSizeByID(mMemID);
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

    public bool SetReferenceName(string src)
    {
        return false;
    }


}