using DWORD = System.UInt32;
/// <summary>
/// iUnifiedVariableArray - массив переменных, доступ по индексу
/// </summary>
internal interface iUnifiedVariableArray : iUnifiedVariableContainer
{
    new public const DWORD ID = 0x5766773F;
    public DWORD SetSize(DWORD NewSize);
    public iUnifiedVariable CreateVariable(int ClassID, DWORD index);
    //template<class T> T* CreateVariableTpl(DWORD index);
    //template<class T> T* GetVariableTpl(DWORD index);
    public bool SwapVariables(DWORD index1, DWORD index2);
    public DWORD Shrink(DWORD NewSize);
        
}
