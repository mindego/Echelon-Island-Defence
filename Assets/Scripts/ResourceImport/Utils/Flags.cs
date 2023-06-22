using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class Flags
{
     int flags;

    public Flags(int flags)
    {
        this.flags = flags;
    }

    public Flags() : this(0)
    {
    }

    public int Get(int f)  
    {
        return flags & f; 
    }
    public void Set(int f, bool st = true) 
    {
        flags = st ? flags | f : flags & ~f;
        
    }

    public void set(int f)
    {
        Set(f, true);
    }
    public void clear(int f)
    {
        Set(f, false);
    }

    public int get(int f)
    {
        return Get(f);
    }
  
    public int GetFlags()
    {
        return flags;
    }

    public int ToInt()
    {
        return flags;
    }
    public override string ToString()
    {
        return "Flags: " + flags;
    }
}
