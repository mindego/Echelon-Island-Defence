using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXPLOSION_DATA : STORM_DATA
{
    //  // internal part
    //  EXPLOSION_DATA(const char*,float);
    //virtual void ProcessToken(READ_TEXT_FILE&,const char*);
    //  virtual void Reference(const EXPLOSION_DATA* ref);
    //virtual void MakeLinks();
    //  // data section
    //  float LifeTime, Timer;
    //  int Vertical;
    //  VECTOR Delta;
    //  float SpeedCoeff;
    //  float Probability;
    //  bool LoopedSound;
    //  float VDist2;
    //  unsigned int Particle;
    //  VisLightData LightData1;
    //  float LightD;
    //  VisDecalData DecalData1;
    //  float myFlareProbability;
    //  Bool<true> myHashed;

    //  TLIST<DEBRIS_SET> DebrisSetsList;
    //  TLIST<EXPLOSION_DATA> ExplChain;
    //  static const int FlareLen = 256;
    //  char Flare[FlareLen];
    //  // data access
    //  static STORM_DATA_API EXPLOSION_DATA* __cdecl GetByName(const char* Name,bool MustExist = true);
    //  static STORM_DATA_API EXPLOSION_DATA* __cdecl GetByCode(unsigned int Code, bool MustExist = true);
    //  static STORM_DATA_API EXPLOSION_DATA* __cdecl GetFirstItem();
    //  static STORM_DATA_API int __cdecl nItems();

    // internal part

    // data section
    public float LifeTime, Timer;
    public int Vertical;
    public Vector3 Delta;
    public float SpeedCoeff;
    public float Probability;
    public bool LoopedSound;
    public float VDist2;
    public uint Particle;
    //public VisLightData LightData1;
    public float LightD;
    //public VisDecalData DecalData1;
    public float myFlareProbability;
    //public Bool<true> myHashed;

    //TLIST<DEBRIS_SET> DebrisSetsList;
    //TLIST<EXPLOSION_DATA> ExplChain;
    //static const int FlareLen = 256;
    //char Flare[FlareLen];
    
}
