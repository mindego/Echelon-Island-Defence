using DWORD = System.UInt32;
/// <summary>
/// iUnifiedVariableContainer - базовый класс для переменных-контейнеров
/// </summary>
internal interface iUnifiedVariableContainer : iUnifiedVariable
{
    new public const DWORD ID = 0xF705FAA8;
    new public void Query(int ClassID);
    //template<class C> C* Query() { return (C*)Query(C::ID); }
    public DWORD GetSize();
    public DWORD GetNextHandle(DWORD Handle);
    public int GetNameLengthByHandle(DWORD Handle);
    public string GetNameByHandle(string buffer, DWORD Handle);
    public DWORD GetHandleByName(string Name);
    public int GetSizeByHandle(DWORD Handle);
    public iUnifiedVariable GetVariableByHandle(DWORD Handle);
    public iUnifiedVariable GetVariableByName(string name, DWORD crc = 0xFFFFFFFF);
    public iUnifiedVariable CreateVariableByName(int ClassID, string name);
    public bool Rename(string pSrcName, string pDstName);

    //template<class T> T* CreateVariableTpl(const char* name);
    //template<class T> T* GetVariableTpl(DWORD Handle);
    //template<class T> T* GetVariableTpl(const char* name, DWORD crc=0xFFFFFFFF);
}