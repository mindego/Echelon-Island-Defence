using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float SunSpeed;
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.right * Time.deltaTime * SunSpeed);
    }
}
