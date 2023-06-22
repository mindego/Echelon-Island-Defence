using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    public Transform MainCamera;
    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (MainCamera == null) return;
        transform.rotation = MainCamera.transform.rotation;
    }
}
