using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class READ_TEXT_FILE
{
    //  char filename[MAX_FILE_NAME];
    //  int string;
    //  int comment;
    //  int crc;
    //  bool count_crc;
    //  char Buffer[1024];
    //  char* Ptr;
    //  char* DataPtr;
    //  const char* DataPos;

    public string filename;
    public int stringField;
    public int comment;
    public int crc;
    public bool count_crc;
    public char[] Buffer = new char[1024];
    uint DataPos;

    public READ_TEXT_FILE(string name,int Comment,bool CountCrc,uint data)
    {
        filename = name;
        count_crc = CountCrc;
        comment = Comment;
        stringField = 0;
        DataPos = data;
        Buffer[0] = default;
        crc = -1;
    }

    public bool Ready(bool MustExist)
    {
        //if (DataPos != null) return true;
        return true;
    }

    public string ReadLine(bool CanBeEOF)
    {
        return default;
    }
}

public class READ_TEXT_STREAM
{
    private Stream stream;
    public int stringCount;
    public char comment;
    public int crc;
    public bool count_crc;
    public char[] Buffer = new char[1024];
    uint DataPos;

    public char[] Dels = { ' ','=', '\t','(',')','\r','\n'};

    private StreamReader reader;
    public READ_TEXT_STREAM(Stream stream, char Comment, bool CountCrc, uint data)
    {
        this.stream = stream;
        comment = Comment;
        count_crc = CountCrc;
        DataPos = data;
        stringCount = 0;

        reader=new StreamReader(stream, Encoding.GetEncoding(1251));
    }

    public bool Ready(bool MustExist)
    {
        //I'm always ready!
        return true;
    }

    public string ReadLine(bool CanBeEOF=true)
    {
        string rl = reader.ReadLine();
        if (rl == null) return null;

        Buffer = rl.ToCharArray();

        stringCount++;

        //if (comment!=null) { char* c = StrChr(Buffer, comment); if (c) *c = 0; }
        //if (count_crc) crc ^= HashString(Buffer);

        return new string(Buffer);
    }

    public string GetNextItem(bool ForceRead, bool CanBeEOF) {
        while (true)
        {
            string myString = ReadLine(CanBeEOF);
            if (myString == null) return null;

            string[] tokens = myString.Split(Dels);
            List<string> cleanup = new List<string>();
            foreach (string token in tokens)
            {
                if (token != "") cleanup.Add(token);
            }
            Debug.LogWarning("New set");
            foreach (string token in cleanup.ToArray())
            {
                Debug.Log($"[{token}]");
            }
        }
    }

}
