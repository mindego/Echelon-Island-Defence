using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXPLOSION_INFO:STORM_DATA
{
    //  struct EXPLOSION_DATA *explosion;
    //float XDamage, XRadius;

    //  EXPLOSION_INFO();
    //  EXPLOSION_INFO(EXPLOSION_INFO*);
    //  void Set(float xDamage, float xRadius) { XDamage = xDamage; XRadius = xRadius; }
    //  void Zero() { explosion = 0; XDamage = XRadius = 0; }
    //  void Load(READ_TEXT_FILE&);
    //  void MakeLinks();

    //  static EXPLOSION_INFO* SafeExplosionInfoLoad(EXPLOSION_INFO* old, READ_TEXT_FILE& f, float XD, float XR);
    //  static void SafeExplosionInfoMakeLinks(EXPLOSION_INFO** info,const char* s);
    //  static EXPLOSION_INFO* SafeExplosionInfoCopy(EXPLOSION_INFO* info);
    //  static void SafeExplosionInfoDelete(EXPLOSION_INFO* info);

    public EXPLOSION_DATA explosion;
    public float XDamage, XRadius;

    public void Set(float xDamage, float xRadius) { XDamage = xDamage; XRadius = xRadius; }
    public void Zero() { explosion = null; XDamage = XRadius = 0; }
    //public void Load(READ_TEXT_FILE&);
    //public void MakeLinks();
}
