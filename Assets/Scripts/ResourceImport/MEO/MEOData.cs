using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class MEOData
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SLOT_DATA
    {
        int OfName;
        Vector3 Org, Dir, Up;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IMAGE
    {
        //size=36
        public Vector3 Min, Max;
        public float Radius;
        public int nBoxes;
        public int OfPlanes;
        public int OfPlGrps;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MEO_DATA
    {
        //size=208
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public IMAGE[] Images = new IMAGE[4]; //4
        public Vector3 Org, Dir, Up;
        int reserved;
        public int Name;
        public int Flags;
        public int nSlots;
        public int Of_Slots; //union SLOT_DATA *pSlots
        public int nSubObjects;
        public int Of_SubObjects; //MEO_DATA* pSubObjects;

        public override string ToString()
        {
            
            string res="MEO_DATA\n";
            byte[] tmpNameInt = BitConverter.GetBytes(Name);
            string tmpName = BitConverter.ToString(tmpNameInt);

            //res += "Name " + Name.ToString("X8") + $" Slots {nSlots}" + $" SubObj {nSubObjects}" + "\n";
            res += "Name " + tmpName + $" Slots {nSlots}" + $" SubObj {nSubObjects}" + "\n";
            res += $"Org {Org}\n";
            res += $"Dir {Dir}\n";
            res += $"Up {Up}\n";

                return res;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MEO_DATA_HDR
    {
        public int OfPlanes; // union { PLANE* Planes;
        public int Of_Slots; //union { SLOT_DATA* _Slots;
        public int OfPlGrps;// union{ short* PlGrps;};
        public int Of_Chars; //union{ char* _Chars;;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] MsnBounds;// = new float[4];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0)]
        public MEO_DATA[] MeoData;
        //public MEO_DATA[] MeoData = new MEO_DATA[];
        



        public override string ToString()
        {

            return $"OfPlanes {OfPlanes} Of_Slots {Of_Slots} OfPLGrps {OfPlGrps} Of_Chars {Of_Chars} bounds size {MsnBounds.Length} MeoData size {MeoData}";
        }
    }
}

