using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadUtils
{
    //void parseMultiData(IMappedDb* db, cstr msg, cstr block_name, cstr txt_name, cstr header, cstr* keys, InsertCall* calls)
    //{
    //    LoadingMessage(msg);
    //    char* data = db ? db->GetBlock(block_name).Convert<char>() : 0;
    //    READ_TEXT_FILE f(txt_name,';',true, data);
    //    const char* c = f.ReadLine();
    //    if (StrCmp(c, header)) PARSE_ERROR;
    //    while (1)
    //    {
    //        c = f.GetNextItem(false, true);
    //        if (!c) break;
    //        int i = 0;
    //        while (keys[i])
    //        {
    //            Recognize(keys[i]) { (*calls[i])(f); break; }
    //            i++;
    //        }
    //        if (keys[i])
    //            continue;
    //        PARSE_ERROR;
    //    }
    //    LogAppendSucceeded();
    //    STORM_DATA::crc ^= f.GetCrc();
    //}

    public void parseMultiData(Stream st,string msg,string block_name,string txt_name,string header, string[] keys, CARRIER_DATA.CarrierLoadCallbacks[] calls)
    {
        Debug.Log(msg);
        var f = new READ_TEXT_STREAM(st, ';', true, 0);
        string c = f.ReadLine(true);
        if (c != header) return;
        c = f.GetNextItem(false, true);
        Debug.Log(c);
    }

}
