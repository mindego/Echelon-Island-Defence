using DWORD = System.UInt32;
// *********************************************************************************************************
// iUnifiedVariableFloat - хранение float
interface iUnifiedVariableFloat : iUnifiedVariable
{
    new public const DWORD ID = 0x3FCFC71D;
    public float GetValue();
    public float SetValue(float value);
};

