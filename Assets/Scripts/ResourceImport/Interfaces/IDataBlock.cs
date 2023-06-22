interface IDataBlock : IObject
{
    public int getLength();
    public void getValue(byte[] buf, int len);
};

