using System.Collections;
using System.Collections.Generic;
using DWORD = System.UInt32;

/// <summary>
/// базовый класс переменных
/// </summary>
public interface iUnifiedVariable : IObject
{
    //IID(0x0EEE232E);
    new public const DWORD ID = 0x0EEE232E;
    public DWORD GetClassId();
    public bool Delete();
    public bool ExportToFile(string filename);
    public bool ImportFromFile(string filename);
    public int GetNameLength();
    public string GetName(string value);
    //public string GetName();

    public string getNameShort()
    {
        return GetName(string.Empty);
    }

    public string getNameLong()
    {
        return GetName(string.Empty);
    }

};
