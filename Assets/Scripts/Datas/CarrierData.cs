using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// ����� �������� ������� �������� � ��������� ������
/// </summary>
public class CARRIER_DATA : OBJECT_DATA
{
    /// <summary>
    /// TRUE=������� ����
    /// </summary>
    public bool IsSeaUnit;                   // TRUE=������� ����
                                             // �������� ��������
    /// <summary>
    /// ������������ �������������� ��������
    /// </summary>
    public float MaxSpeedZ;                   // ������������ �������������� ��������
    /// <summary>
    /// ������������ ������������ ��������
    /// </summary>
    public float MaxSpeedY;                   // ������������ ������������ ��������
    /// <summary>
    /// �������� �������������� ��������
    /// 1.f/MaxSpeedZ 
    /// </summary>
    public float OOMaxSpeedZ;                 // 1.f/MaxSpeedZ

    /// <summary>
    /// ������������ �������������� ���������
    /// </summary>
    public float MaxAccelZ;                   // ������������ �������������� ���������
    /// <summary>
    /// ������������ ������������ ���������
    /// </summary>
    public float MaxAccelY;                   // ������������ ������������ ���������

    /// <summary>
    /// ������������ ������� �������� �������� � �������������� ���������
    /// </summary>
    public float ASpeedX;                     // ������������ ������� �������� �������� � �������������� ���������
    /// <summary>
    /// ������������ ������� �������� �������� � ������������ ���������
    /// </summary>
    public float ASpeedY;                     // ������������ ������� �������� �������� � ������������ ���������

    /// <summary>
    /// ������������ ������� ��������� � �������������� ���������
    /// </summary>
    public float AAccelX;                     // ������������ ������� ��������� � �������������� ���������
    /// <summary>
    /// ������������ ������� ��������� � ������������ ���������
    /// </summary>
    public float AAccelY;                     // ������������ ������� ��������� � ������������ ���������

    /// <summary>
    /// ������������ ���� ������� (������� ������ ��� "������")
    /// </summary>
    public float PitchLimit;                  // ������������ ���� �������

    public delegate void CarrierLoadCallbacks(Stream stream);
    
    public void insertSeaCarrierData(Stream stream)
    {
        Debug.Log("Loading sea carrier");
    }
    public void insertAirCarrierData(Stream stream)
    {
        Debug.Log("Loading air carrier");
    }
    public void loadCarrierData(Stream stream)
    {
        string[] keys = { "AirCarrier", "SeaCarrier" };
        CarrierLoadCallbacks[] callbacks = { insertAirCarrierData, insertSeaCarrierData };
        LoadUtils lu = new LoadUtils();
        lu.parseMultiData(stream, "carrier", "Carriers", "Carriers.txt", "[STORM CARRIERS DATA FILE V1.0]", keys, callbacks);
    }
}


