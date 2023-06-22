using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iMovementSystemCarrier
{
    const uint ID = 0xF11AAF27;

    // управление автопилотом
    public void MoveTo(Vector3 v, float time, float max_speed);
    //void NearUnit(iContact*,const VECTOR& Delta)=0;
    public void Pause(bool pause);
    public bool IsStopped();
}
