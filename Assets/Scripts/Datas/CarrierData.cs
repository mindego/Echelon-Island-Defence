using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Класс описания крупных летающих и плавающих юнитов
/// </summary>
public class CARRIER_DATA : OBJECT_DATA
{
    /// <summary>
    /// TRUE=морской юнит
    /// </summary>
    public bool IsSeaUnit;                   // TRUE=морской юнит
                                             // линейные значения
    /// <summary>
    /// максимальная горизонтальная скорость
    /// </summary>
    public float MaxSpeedZ;                   // максимальная горизонтальная скорость
    /// <summary>
    /// максимальная вертикальная скорость
    /// </summary>
    public float MaxSpeedY;                   // максимальная вертикальная скорость
    /// <summary>
    /// Обратная горизонтальная скорость
    /// 1.f/MaxSpeedZ 
    /// </summary>
    public float OOMaxSpeedZ;                 // 1.f/MaxSpeedZ

    /// <summary>
    /// максимальное горизонтальное ускорение
    /// </summary>
    public float MaxAccelZ;                   // максимальное горизонтальное ускорение
    /// <summary>
    /// максимальное вертикальное ускорение
    /// </summary>
    public float MaxAccelY;                   // максимальное вертикальное ускорение

    /// <summary>
    /// максимальная угловая скорость поворота в горизонтальной плоскости
    /// </summary>
    public float ASpeedX;                     // максимальная угловая скорость поворота в горизонтальной плоскости
    /// <summary>
    /// максимальная угловая скорость поворота в вертикальной плоскости
    /// </summary>
    public float ASpeedY;                     // максимальная угловая скорость поворота в вертикальной плоскости

    /// <summary>
    /// максимальное угловое ускорение в горизонтальной плоскости
    /// </summary>
    public float AAccelX;                     // максимальное угловое ускорение в горизонтальной плоскости
    /// <summary>
    /// максимальное угловое ускорение в вертикальной плоскости
    /// </summary>
    public float AAccelY;                     // максимальное угловое ускорение в вертикальной плоскости

    /// <summary>
    /// максимальный угол тангажа (поворот вокруг оси "Вправо")
    /// </summary>
    public float PitchLimit;                  // максимальный угол наклона

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


