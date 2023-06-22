using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjSpawnerTimer : MonoBehaviour
{
    public float ttl;

    public ObjSpawnerTimer(float ttl)
    {
        this.ttl = ttl;
    }

    private void FixedUpdate()
    {
        ttl -= Time.deltaTime;
        if (ttl < 0) Destroy(gameObject);
    }
}
