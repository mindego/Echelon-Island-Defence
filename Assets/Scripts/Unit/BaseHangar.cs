using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHangar : MonoBehaviour
{
    private Transform pDoor;
    private float currentAngle;
    private float targetAngle,startAngle;
    private Quaternion closedRotation;
    private bool setClosed = false;
    private const float doorSpeed= 10f;
    private float stime;
    float totalTime;
    private Status status=Status.CLOSED;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        processDoor();
    }

    private Transform getDoor(Transform parent)
    { 
        if (parent.childCount == 0) return null;
        
        foreach (Transform child in parent)
        {
            Debug.Log(child.name.ToLower());
            if (child.name.ToLower() == "door")
            {
                pDoor = child;
                //Debug.Log($"Door found {child.name}");
                return pDoor;
            }
            pDoor = getDoor(child);
            if (pDoor != null) return pDoor;
            //if (child.childCount > 0) return getDoor(child);
        }

        return null;
    }
    private Transform getDoor()
    {
        if (pDoor != null) return pDoor;

        pDoor = getDoor(transform);
        if (!setClosed)
        {
            setClosed = true;
            closedRotation = pDoor.localRotation;
        }
        return pDoor;
    }
    private void processDoor()
    {
        if (currentAngle == targetAngle) return;
        
        pDoor = getDoor();
        if (pDoor == null) return;
        
       float timePass = (Time.time - stime)/totalTime;

        Debug.Log($"dTime {Time.time - stime} / {totalTime} = {timePass}");
        //Debug.Log($"Rotating from {startAngle} to {targetAngle} (diff {diff}) {timePass}");
        currentAngle = Mathf.LerpAngle(startAngle, targetAngle, timePass);
        pDoor.localRotation = closedRotation * Quaternion.Euler(0, currentAngle, 0);
        if (currentAngle == targetAngle)
        {
            switch (status)
            {
                case Status.OPENING:
                {
                        status = Status.OPEN;
                        break;
                }
                case Status.CLOSING:
                    {
                        status = Status.CLOSED;
                        break;
                    }
            }
            Debug.Log(status);
        }
    }
    private enum Status{
        OPEN,
        CLOSED,
        OPENING,
        CLOSING
    }
    public void OpenDoor()
    {
        if (status==Status.OPEN) return;
        status = Status.OPENING;
        targetAngle = 90f;
        startAngle = currentAngle;
        totalTime = Mathf.Abs(targetAngle - startAngle) / doorSpeed;
        stime = Time.time;
    }

    public void CloseDoor()
    {
        if (status==Status.CLOSED) return;
        status = Status.CLOSING;
        targetAngle = 0f;
        startAngle = currentAngle;
        totalTime = Mathf.Abs(targetAngle - startAngle) / doorSpeed;
        stime = Time.time;
    }

    public void ToggleDoor()
    {
        switch (status)
        {
            case Status.CLOSED:
                OpenDoor();
                break;
            case Status.OPEN:
                CloseDoor();
                break;
        }
    }

    public void OnRadioMessage(RadioMessageOrder message)
    {
        if (message.messageId == 0xFF) ToggleDoor();
    }
}
