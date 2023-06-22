using System;
using System.IO;
using System.Text;
using DWORD = System.UInt32;
/// <summary>
/// UniVarString - реализация iUnifiedVariableString
/// </summary>
class UniVarString : iUnifiedVariableString
{
    private int mCounter;
    private iUniVarParent mpParent;
    private DWORD mMemID;
    public UniVarString(iUniVarParent par, DWORD memid) : this(1, par, memid) { }
    public UniVarString(int mCounter, iUniVarParent mpParent, uint mMemID)
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
        return iUnifiedVariableString.ID;
    }

    public string GetName(string value)
    {
        return mpParent.GetName(value, this);
    }

    public int GetNameLength()
    {
        return mpParent.GetNameLength(this);
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

    public bool SetValue(string src)
    {
        if (mpParent.IsReadOnly()) return false;
        return false;
    }

    public void StrCpy(out string dst)
    {
        int l = (int) mpParent.GetMemManager().GetSizeByID(mMemID);
        if (l == 0) dst = String.Empty;

        byte[] data = mpParent.GetMemManager().GetDataByID(mMemID);
        Encoding enc = Encoding.GetEncoding("windows-1251");
        dst = enc.GetString(data);
    }

    public int StrLen()
    {
        return (int) mpParent.GetMemManager().GetSizeByID(mMemID);
    }


    public void StrnCpy(out string dst, int n)
    {
       StrCpy(out dst);
    }
}