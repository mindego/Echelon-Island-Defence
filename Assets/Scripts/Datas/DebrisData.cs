using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBRIS_DATA : STORM_DATA
{
    const uint DISAPPEAR_AFTER_LAUNCH = 1;
    const uint DISAPPEAR_AFTER_TOUCH = 2;
    const uint DEBRIS_ROTATE_NOTHING = 0;
    const uint DEBRIS_ROTATE_IMMEDIATELY = 1;
    const uint DEBRIS_ROTATE_CONTINUES = 2;
    const uint DEBRIS_COLLTYPE_ROUGH = 0;
    const uint DEBRIS_COLLTYPE_PRECISE = 1;
    const uint DEBRIS_DEFAULT_LIFE = 5;

    //  // internal part
    //  DEBRIS_DATA(const char*);
    //  virtual void ProcessToken(READ_TEXT_FILE&,const char*);
    //  virtual void Reference(const DEBRIS_DATA* ref);
    //virtual void MakeLinks();
    //  virtual ~DEBRIS_DATA();
    //  // data section
    //  float AppearProbability;
    //  float GoukeMin, FrictionCoeff, GoukeMax;
    //  const char* FileName;
    //  unsigned int ParticleOnFly;

    //  // случайные взрывы
    //  LIST ExplsOnStart;
    //  // взрывы
    //  EXPLOSION_INFO* ExplOnEnd;
    //  EXPLOSION_INFO* ExplOnTarget;
    //  EXPLOSION_INFO* ExplOnGround[8];

    //  float XDamage, XRadius;

    //  int CollisionMethod;                                  // сталкивается ли обломок с другими объектами
    //  int DisappearType;                                    // тип исчезновения
    //  int AlwaysLie;
    //  float MinDisappearTimer, MaxDisappearTimer;            // времена исчезновения
    //  float MaxAppearTimer, MinAppearTimer;

    //  int VisibleDisappearFlag;                             // тип исчезновения
    //  float Massa;                                          // масса
    //  float RotateOffset;                                   // смещение пивота
    //  float WaterGravity;                                   // плавучесть
    //  int JumpOnCreate;                                   // ставить ли на землю после создания
    //  VECTOR RotateAxis;                                    // ось вращения
    //  int RandomRotateAxis;                                 // случайная ли ось вращения
    //  int RotateType;                                       // тип вращения
    //  float RotateMaxSpeed, RotateAccel, RotateMinSpeed;      // скорости вращения
    //  int CheckCollisionType;                               // тип проверки на столкновение
    //  float AirFriction;                                    // величина трения
    //  int AirFrictionAffected;                             // влияет ли на нас трение об воздух
    //  int RotateSlowing;                                   // замедляется ли вращение
    //  float MaxSmokeTimer, MinSmokeTimer;                    // настройки испускания пыли
    //  int ShadowFlag;                                       // отбрасываем ли тень
    //  VECTOR SmokeOffset;                                   // смещения для дыма
    //  float DisappearSpeed;                                 // скорость исчезания под землю
    //  float EffectsProbability;                             // вероятность появления эффектов( партикль, звук)
    //                                                        // data access
    //  static STORM_DATA_API DEBRIS_DATA*  __cdecl GetByName(const char* Name,bool MustExist = true);
    //  static STORM_DATA_API DEBRIS_DATA*  __cdecl GetByCode(unsigned int Code, bool MustExist = true);
    //  static STORM_DATA_API DEBRIS_DATA*  __cdecl GetFirstItem();
    //  static STORM_DATA_API int __cdecl nItems();
    // internal part
    
    //virtual void ProcessToken(READ_TEXT_FILE&,const char*);
    //virtual void Reference(const DEBRIS_DATA* ref);
  //virtual void MakeLinks();

    // data section
    public float AppearProbability;
    public float GoukeMin, FrictionCoeff, GoukeMax;
    public string FileName;
    uint ParticleOnFly;

    // случайные взрывы
    //LIST ExplsOnStart;
    // взрывы
    public EXPLOSION_INFO ExplOnEnd;
    public EXPLOSION_INFO ExplOnTarget;
    public EXPLOSION_INFO[] ExplOnGround = new EXPLOSION_INFO[8];

    public float XDamage, XRadius;

    public int CollisionMethod;                                  // сталкивается ли обломок с другими объектами
    public int DisappearType;                                    // тип исчезновения
    public int AlwaysLie;
    public float MinDisappearTimer, MaxDisappearTimer;            // времена исчезновения
    public float MaxAppearTimer, MinAppearTimer;

    public int VisibleDisappearFlag;                             // тип исчезновения
    public float Massa;                                          // масса
    public float RotateOffset;                                   // смещение пивота
    public float WaterGravity;                                   // плавучесть
    public int JumpOnCreate;                                   // ставить ли на землю после создания
    public Vector3 RotateAxis;                                    // ось вращения
    public int RandomRotateAxis;                                 // случайная ли ось вращения
    public int RotateType;                                       // тип вращения
    public float RotateMaxSpeed, RotateAccel, RotateMinSpeed;      // скорости вращения
    public int CheckCollisionType;                               // тип проверки на столкновение
    public float AirFriction;                                    // величина трения
    public int AirFrictionAffected;                             // влияет ли на нас трение об воздух
    public int RotateSlowing;                                   // замедляется ли вращение
    public float MaxSmokeTimer, MinSmokeTimer;                    // настройки испускания пыли
    public int ShadowFlag;                                       // отбрасываем ли тень
    public Vector3 SmokeOffset;                                   // смещения для дыма
    public float DisappearSpeed;                                 // скорость исчезания под землю
    public float EffectsProbability;                             // вероятность появления эффектов( партикль, звук)
                                                          // data access

}
