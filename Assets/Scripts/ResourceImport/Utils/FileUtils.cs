using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.IO.MemoryMappedFiles;
using System.Xml.Serialization;

public class StormFileUtils
{
    public static object ReadStruct(Stream st, Type t, long offset = 0)
    {
        byte[] buffer = new byte[Marshal.SizeOf(t)];
        st.Seek(offset, SeekOrigin.Begin);
        st.Read(buffer, 0, Marshal.SizeOf(t));
        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        object temp = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), t);
        handle.Free();
        return temp;
    }

    public static T ReadStruct<T>(MemoryMappedFile mmf, long offset = 0) where T : struct
    {
        int dataSize = Marshal.SizeOf(typeof(T));
        MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor(offset, dataSize);

        if (!accessor.CanRead) return default(T);

        T res;

        accessor.Read<T>(0, out res);
        return res;
    }

    public static T[] ReadStructs<T>(byte[] data, int offset = 0, int count = 1)
    {
        byte[] filteredData = new byte[data.Length - offset];
        int dataSize = filteredData.Length / count;
        Array.Copy(data, offset, filteredData, 0, filteredData.Length);
        T[] res = new T[count];
        byte[] buffer = new byte[dataSize];
        for (int i = 0; i < count; i++)
        {
            Array.Copy(filteredData, i * dataSize, buffer, 0, dataSize);
            res[i] = ReadStruct<T>(buffer);
        }
        return res;
    }
    public static T[] ReadStructs<T>(MemoryMappedFile mmf, int offset = 0, int count = 1) where T : struct
    {
        int dataSize = count * GetSize<T>();

        MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor(offset, dataSize);
        T[] buffer = new T[count];

        accessor.ReadArray<T>(0, buffer, 0, count);
        return buffer;
    }

    public static Stream GetStream(MemoryMappedFile mmf, int offset, int size)
    {
        var accessor = mmf.CreateViewStream(offset, size);
        return accessor;
    }

    public static byte[] ReadBytes(MemoryMappedFile mmf, int offset, int size) {
        MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor(offset, size);

        byte[] buffer = new byte[size];
        accessor.ReadArray<byte>(0, buffer, 0, size);
        return buffer;
    }
    public static int GetSize<T>()
    {
        return Marshal.SizeOf<T>();
    }
    public static T ReadStruct<T>(Stream st, long offset = 0)
    {
        return (T)ReadStruct(st, typeof(T), offset);
    }
    /// <summary>
    /// Создаёт класс или структуру из массива байтов
    /// </summary>
    /// <typeparam name="T">Тип создаваемого класса или структуры</typeparam>
    /// <param name="byteArray">Массив байтов</param>
    /// <returns></returns>
    public static T ReadStruct<T>(byte[] byteArray,int offet = 0)
    {
        byte[] buffer = new byte[byteArray.Length - offet];
        Array.Copy(byteArray, offet, buffer, 0, buffer.Length);
        //MemoryStream ms = new MemoryStream(byteArray);
        MemoryStream ms = new MemoryStream(byteArray);
        T data = ReadStruct<T>(ms);
        ms.Close();
        return data;
    }
    public static T LoadXML<T>(Stream ms)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));

        var schema = (T)serializer.Deserialize(ms);
        return schema;
    }
    public static T LoadXML<T>(TextAsset xml)
    {
        MemoryStream ms = new MemoryStream(xml.bytes);

        var schema = LoadXML<T>(ms);
        ms.Close();
        return schema;
    }

    public static T LoadXML<T>(string filename) {
        FileStream fs = new FileStream(filename, FileMode.Open);
        var schema = LoadXML<T>(fs);
        fs.Close();
        return schema;
    }

    public static bool SaveXML<T>(string filename, T myObject)
    {
        if (File.Exists(filename)) File.Delete(filename);
        FileStream fs = File.Open(filename, FileMode.OpenOrCreate);
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        serializer.Serialize(fs, myObject); 
        fs.Close(); 
        Debug.Log("File saved: " + filename);
        return true;
    }
}
