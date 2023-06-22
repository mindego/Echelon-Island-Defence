using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseObject : MonoBehaviour,BaseUnit
{
    public Vector3d pos;
    public Vector3 posV3;
    
    public Quaternion rotation;
    public Vector3 TransformPosition;
    public Vector3 prevTransformPosition;
    protected Vector3 speed;

    public Text LocalCoordinates;

    public void SetHUDLocalCoordinates(Vector3 coords)
    {
        if (LocalCoordinates == null) return;
        LocalCoordinates.text = coords.ToString();
    }
    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        pos = new Vector3d(transform.position);
        prevTransformPosition = transform.position;
    }
    private void Update()
    {
        pos.Add(transform.position - prevTransformPosition);
        //TransformPosition = transform.position;
        prevTransformPosition = transform.position;
    }

    public void FixedUpdate()
    {
        posV3 = pos.ToVector3();
    }

    public void Init(Vector3d pos, Vector3 realPos)
    {
        this.pos = pos;
        this.prevTransformPosition = realPos;
    }

    protected void OnBecameInvisible()
    {
        if (!TryGetComponent<Rigidbody>(out Rigidbody rb)) return;

        speed = rb.velocity;
        Debug.Log($"{name} invisible!");
    }

    protected void OnBecameVisible()
    {
        if (!TryGetComponent<Rigidbody>(out Rigidbody rb)) return;
        rb.velocity = speed;
        Debug.Log($"{name} visible!");
    }
}


public class Vector3d
{
    public double x, y, z;
    public static Vector3d zero = new Vector3d(0, 0, 0);
    public Vector3d() : this(0, 0, 0) {}

    public Vector3d(Vector3 vector3)
    {
        this.x = vector3.x;
        this.y = vector3.y;
        this.z = vector3.z;
    }
    public Vector3d(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public void Add(Vector3 vector3)
    {
        x += vector3.x;
        y += vector3.y;
        z += vector3.z;
    }
    public static Vector3d operator +(Vector3d lVector3d,Vector3d rVector3d)
    {
        Vector3d res = new Vector3d
        {
            x = lVector3d.x + rVector3d.x,
            y = lVector3d.y + rVector3d.y,
            z = lVector3d.z + rVector3d.z
        };

        return res;
    }

    public static Vector3d operator -(Vector3d lVector3d, Vector3d rVector3d)
    {
        Vector3d res = new Vector3d();
        res.x = lVector3d.x - rVector3d.x;
        res.y = lVector3d.y - rVector3d.y;
        res.z = lVector3d.z - rVector3d.z;

        return res;
    }

    public static Vector3d operator *(Vector3d vector3D,float multipier)
    {
        vector3D.x *= multipier;
        vector3D.y *= multipier;
        vector3D.z *= multipier;
        return vector3D;
    }

    public Vector3 ToVector3()
    {
        return new Vector3((float) x, (float) y, (float) z);
    }

    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }
}
