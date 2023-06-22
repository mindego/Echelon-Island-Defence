using DWORD = System.UInt32;
/*===========================================================================*\
|  Deleteble object                                                           |
\*===========================================================================*/
public interface IMemory
{
    //IID(0xEF03808B);
    public const DWORD ID = 0xEF03808B;
    public int Release();
};


