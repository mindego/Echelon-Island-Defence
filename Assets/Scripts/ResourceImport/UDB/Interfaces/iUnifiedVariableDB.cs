using DWORD = System.UInt32;
/// <summary>
/// iUnifiedVariableDB - root
/// </summary>
internal interface iUnifiedVariableDB : iUnifiedVariableContainer
{
    new public const DWORD ID = 0xBAADF00D;
    public DWORD GetRootDataSize();
    public iUnifiedVariable GetRoot();
    public iUnifiedVariable CreateRoot(int ClassID);
    public bool SaveToFile(string filename);

    //template<class T> T* GetRootTpl();
    //template<class T> T* CreateRootTpl();
};
