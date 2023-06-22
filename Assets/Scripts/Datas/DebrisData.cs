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

    //  // ��������� ������
    //  LIST ExplsOnStart;
    //  // ������
    //  EXPLOSION_INFO* ExplOnEnd;
    //  EXPLOSION_INFO* ExplOnTarget;
    //  EXPLOSION_INFO* ExplOnGround[8];

    //  float XDamage, XRadius;

    //  int CollisionMethod;                                  // ������������ �� ������� � ������� ���������
    //  int DisappearType;                                    // ��� ������������
    //  int AlwaysLie;
    //  float MinDisappearTimer, MaxDisappearTimer;            // ������� ������������
    //  float MaxAppearTimer, MinAppearTimer;

    //  int VisibleDisappearFlag;                             // ��� ������������
    //  float Massa;                                          // �����
    //  float RotateOffset;                                   // �������� ������
    //  float WaterGravity;                                   // ����������
    //  int JumpOnCreate;                                   // ������� �� �� ����� ����� ��������
    //  VECTOR RotateAxis;                                    // ��� ��������
    //  int RandomRotateAxis;                                 // ��������� �� ��� ��������
    //  int RotateType;                                       // ��� ��������
    //  float RotateMaxSpeed, RotateAccel, RotateMinSpeed;      // �������� ��������
    //  int CheckCollisionType;                               // ��� �������� �� ������������
    //  float AirFriction;                                    // �������� ������
    //  int AirFrictionAffected;                             // ������ �� �� ��� ������ �� ������
    //  int RotateSlowing;                                   // ����������� �� ��������
    //  float MaxSmokeTimer, MinSmokeTimer;                    // ��������� ���������� ����
    //  int ShadowFlag;                                       // ����������� �� ����
    //  VECTOR SmokeOffset;                                   // �������� ��� ����
    //  float DisappearSpeed;                                 // �������� ��������� ��� �����
    //  float EffectsProbability;                             // ����������� ��������� ��������( ��������, ����)
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

    // ��������� ������
    //LIST ExplsOnStart;
    // ������
    public EXPLOSION_INFO ExplOnEnd;
    public EXPLOSION_INFO ExplOnTarget;
    public EXPLOSION_INFO[] ExplOnGround = new EXPLOSION_INFO[8];

    public float XDamage, XRadius;

    public int CollisionMethod;                                  // ������������ �� ������� � ������� ���������
    public int DisappearType;                                    // ��� ������������
    public int AlwaysLie;
    public float MinDisappearTimer, MaxDisappearTimer;            // ������� ������������
    public float MaxAppearTimer, MinAppearTimer;

    public int VisibleDisappearFlag;                             // ��� ������������
    public float Massa;                                          // �����
    public float RotateOffset;                                   // �������� ������
    public float WaterGravity;                                   // ����������
    public int JumpOnCreate;                                   // ������� �� �� ����� ����� ��������
    public Vector3 RotateAxis;                                    // ��� ��������
    public int RandomRotateAxis;                                 // ��������� �� ��� ��������
    public int RotateType;                                       // ��� ��������
    public float RotateMaxSpeed, RotateAccel, RotateMinSpeed;      // �������� ��������
    public int CheckCollisionType;                               // ��� �������� �� ������������
    public float AirFriction;                                    // �������� ������
    public int AirFrictionAffected;                             // ������ �� �� ��� ������ �� ������
    public int RotateSlowing;                                   // ����������� �� ��������
    public float MaxSmokeTimer, MinSmokeTimer;                    // ��������� ���������� ����
    public int ShadowFlag;                                       // ����������� �� ����
    public Vector3 SmokeOffset;                                   // �������� ��� ����
    public float DisappearSpeed;                                 // �������� ��������� ��� �����
    public float EffectsProbability;                             // ����������� ��������� ��������( ��������, ����)
                                                          // data access

}
