using DWORD = System.UInt32;
/// <summary>
/// iUnifiedVariableBlock - хранение блока двоичных данных
/// </summary>
internal interface iUnifiedVariableBlock : iUnifiedVariable
{
    new public const DWORD ID = 0x75710EAA;
    public int GetLength();
    public void GetValue(byte[] dst, int dst_length);
    public bool SetValue(byte[] src,int src_length);
};
