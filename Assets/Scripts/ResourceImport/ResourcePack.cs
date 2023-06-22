using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using UnityEngine;
using System.IO.MemoryMappedFiles;


public class ResourcePack
{
    public DbFile RAT; //SQUEAK!
    private FileStream dbFile;
    public string[] names;
    public MemoryMappedFile mmf;
    public ResourcePack(string filename)
    {
        Init(filename);
        LoadRAT();
    }

    public ResourcePack()
    {
    }




    /// <summary>
    /// Функция получения MemoryStream по идентификатору ресурса
    /// </summary>
    /// <param name="id">Идентификатор ресурса</param>
    /// <returns>MemoryStream с данными ресурса или null, если идентификатор не найден</returns>
    public virtual Stream GetStreamById(uint id)
    {
        int index = GetIndexById(id);
        return GetStreamByIndex(index);
    }
    /// <summary>
    /// Функция получения MemoryStream по имени ресурса
    /// </summary>
    /// <param name="name">Имя ресурса</param>
    /// <returns>MemoryStream с данными ресурса или null, если имя не найдено</returns>
    public virtual Stream GetStreamByName(string name)
    {
        int index = GetIndexByName(name);
        return GetStreamByIndex(index);

    }
    /// <summary>
    /// Функция получения индекса в паке ресурсов по имени ресурса.
    /// </summary>
    /// <param name="name">Имя ресурса</param>
    /// <returns>int Индекс ресурса или -1, если ресурс не найден</returns>

    public int GetIndexById(uint id)
    {
        for (int i = 0; i < RAT.num; i++)
        {
            if (RAT.index[i].object_id == id)
            {
                return i;
            }
        }
        return -1;
    }
    public int GetIndexByName(string name)
    {
        for (int i = 0; i < RAT.num; i++)
        {
            if (names[i] == name)
            {
                return i;
            }
        }
        return -1;
    }
    /// <summary>
    /// Функция получения MemoryStream по индексу в паке ресурсов
    /// </summary>
    /// <param name="index">Индекс записи в паке ресурсов</param>
    /// <returns>MemoryStream или null, если такого индекса нет</returns>
    public Stream GetStreamByIndex(int index)
    {
        if (index >= RAT.index.Length) return null;
        if (index < 0) return null;

        DbIndex dataIndex = RAT.index[index];
        return StormFileUtils.GetStream(mmf,(int) dataIndex.file_offset, (int) dataIndex.data_size);
        //dbFile.Seek(dataIndex.file_offset, SeekOrigin.Begin);
        //MemoryStream ms = new MemoryStream();
        //byte[] buffer = new byte[dataIndex.data_size];
        //dbFile.Read(buffer);
        //ms.Write(buffer);
        //ms.Seek(0, SeekOrigin.Begin);

        //return ms;
    }
    /// <summary>
    /// Получение идентификатора ресурса из имени ресурса
    /// </summary>
    /// <param name="name">Имя ресурска</param>
    /// <returns>Идентификатор ресурса</returns>
    public virtual uint GetIdByName(string name)
    {
        int i = GetIndexByName(name);
        uint id = 0xFFFFFFFF;
        try
        {
            id= RAT.index[i].object_id;
        } catch {
            Debug.Log($"Object {name} id {id.ToString("X8")}  not found");
        }
        return id;
    }
    public string GetName(int index)
    {
        if (index >= RAT.index.Length) return "UNKNOWN RESOURCE NAME";
        if (index < 0) return "UNKNOWN RESOURCE NAME";
        return names[index];
    }

    public string GetNameById(uint id)
    {
        for (int i = 0; i < RAT.index.Length; i++)
        {
            if (RAT.index[i].object_id == id) return names[i];
        }

        return "Unnamed Resource " + id.ToString("X8");
    }

    public bool Init(string filename)
    {
        if (!File.Exists(filename)) return false;
        string hash = Hash128.Compute(filename).ToString();
        try
        {
            mmf = MemoryMappedFile.CreateFromFile(filename, FileMode.Open, hash);
        } 
        catch (IOException)
        {
            mmf = MemoryMappedFile.OpenExisting (hash);
        }

        return true;
    }
    public bool InitFile(string filename)
    {
        if (!File.Exists(filename)) return false;
        dbFile = File.OpenRead(filename);
        if (!dbFile.CanRead) return false;
        return true;
    }

    public struct PackHeader {
        public int db_id;
        public int num;
        public int data_size;
        public int name_size;
    }

    public bool LoadRAT()
    {
        PackHeader packHeader = (PackHeader) StormFileUtils.ReadStruct<PackHeader>(mmf);

        RAT.data_size = packHeader.data_size;
        RAT.db_id = packHeader.db_id;
        RAT.num = packHeader.num;
        RAT.name_size = packHeader.name_size;
        RAT.index = new DbIndex[RAT.num];

        int dataOffset = 0 + StormFileUtils.GetSize<PackHeader>();
        

        RAT.index = StormFileUtils.ReadStructs<DbIndex>(mmf, dataOffset, packHeader.num);

        int namesOffset = dataOffset + StormFileUtils.GetSize<DbIndex>() * packHeader.num + packHeader.data_size;

        byte[] buffer = StormFileUtils.ReadBytes(mmf, namesOffset, packHeader.name_size);
        Encoding encoding = Encoding.GetEncoding("windows-1251");
        string namesString = encoding.GetString(buffer);

        names = namesString.Trim('\0').Split('\0');

        return true;
    }
    public bool LoadRATFile()
    {

        byte[] buffer = new byte[4];

        dbFile.Read(buffer);
        RAT.db_id=BitConverter.ToInt32(buffer);
        dbFile.Read(buffer);
        RAT.num = BitConverter.ToInt32(buffer);
        dbFile.Read(buffer);
        RAT.data_size = BitConverter.ToInt32(buffer);
        dbFile.Read(buffer);
        RAT.name_size = BitConverter.ToInt32(buffer);

        RAT.index = new DbIndex[RAT.num];
        
        for (int i=0;i<RAT.num; i++)
        {
            DbIndex index = new DbIndex();
            dbFile.Read(buffer);
            index.object_id = BitConverter.ToUInt32(buffer);
            dbFile.Read(buffer);
            index.name_index= BitConverter.ToUInt32(buffer);
            dbFile.Read(buffer);
            index.data_size = BitConverter.ToUInt32(buffer);
            dbFile.Read(buffer);
            index.file_offset = BitConverter.ToUInt32(buffer);

            RAT.index[i] = index;
        }
        
        int names_offset = 4 * 4 + RAT.num * DbIndex.SIZE + RAT.data_size; // 4*4 - header size in bytes;

        buffer = new byte[RAT.name_size];
        dbFile.Seek(names_offset, SeekOrigin.Begin);
        dbFile.Read(buffer);

        Encoding encoding = Encoding.GetEncoding("windows-1251");
        string namesString = encoding.GetString(buffer);

        string[] namesArray = namesString.Trim('\0').Split('\0');
        //Debug.Log((namesArray.Length,RAT.num));
        names = new string[RAT.num];
        for (int i = 0; i < RAT.num; i++)
        {
            //DbIndex dataIndex = RAT.index[i];
            //string smallString = namesString.Substring((int)dataIndex.name_index);
            //string[] namesArray = smallString.Trim('\0').Split('\0');
            //names[i] = GetNameFromFile(i);
            names[i] = namesArray[i];
        } 
            return true;
    }    
}

public struct DbIndex 
{
    /*
    short, short int, signed short, signed short int

    unsigned short, unsigned short int

    int, signed, signed int

    unsigned, unsigned int

    long, long int, signed long, signed long int

    unsigned long, unsigned long int

    long long, long long int, signed long long, signed long long int

    unsigned long long, unsigned long long int

     */
    public uint object_id;
    public uint name_index;
    public uint data_size;
    public uint file_offset;
    public const int SIZE = 4 * 4;
    public uint GetCode()
    {
        return object_id;
    }

    public override string ToString()
    {
        return "Oid: " + object_id + " NI " + name_index + " DS " + data_size + " FO " + file_offset;
    }
}
public struct DbFile
{
    public int db_id;              // data id
    public int num;                // number of data entries
    public int data_size;          //
    public int name_size;
    public DbIndex[] index;

    public DbIndex GetRecord(int i) 
    {  
        return index[i];  
    }

    public byte[] GetNameTable()
    {
        return new byte[0];
    }
  /*  const DbIndex & GetRecord(int i) const {  return index[i];  }

char* GetNameTable() { return (char*)this + sizeof(DbFile) + sizeof(DbIndex) * num + data_size; }

char* GetData(int i) { return (char*)this + index[i].file_offset; }
MemBlock GetBlock(int i) { return MemBlock(GetData(i), index[i].data_size); }
ObjId GetObjId(int i) { return ObjId(GetNameTable() + index[i].name_index, index[i].object_id); }

int FileSize() const     { return sizeof(DbFile) +sizeof(DbIndex) * num + data_size + name_size; }
  */

};

public class ResourceStorage<T> : ResourcePack
{
    Dictionary<string, T> storage = new Dictionary<string, T>();
    Storm.CRC32 crc = new Storm.CRC32();

    public ResourceStorage(string filename) : base(filename)
    {
    }

    
    public override uint GetIdByName(string name)
    {
        if (!name.Contains('#')) return base.GetIdByName(name);
        crc = new Storm.CRC32();
        return crc.HashString(name);
    }

    public override Stream GetStreamById(uint id)
    {
        return base.GetStreamById(id);
    }

    public override Stream GetStreamByName(string name)
    {
        if (!name.Contains('#')) return base.GetStreamByName(name);

        return GetStreamById(GetIdByName(name));
    }
}

