using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCarrier : BaseObject, iMovementSystemCarrier
{
    private Vector3 MoveTarget;
    private Transform AttackTarget;
    public int Voice = 1;
    private RadioTransceiver transceiver;
    private System.Random rand = new System.Random();
    private Rigidbody myRigidBody;
    public TextAsset carrierDataXML;
    private CARRIER_DATA carrierData;
    public CARRIER_DATA _carrierData { get { return carrierData; } }


    private void Awake()
    {
        Initialize();
    }

    new public void Initialize()
    {
        base.Initialize();
        transceiver = GetComponent<RadioTransceiver>();
        myRigidBody = GetComponent<Rigidbody>();

        if (carrierDataXML != null)
            carrierData = StormFileUtils.LoadXML<CARRIER_DATA>(carrierDataXML);
    }

    public bool IsStopped()
    {
        if (speed.magnitude < 10) return true;
        return false;
    }

    public void MoveTo(Vector3 v, float time, float max_speed)
    {
        Rigidbody rb;
        if (!TryGetComponent<Rigidbody>(out rb)) return;
        MoveTarget = v;
        Vector3 dir = (v - transform.position).normalized;
        rb.velocity = Vector3.zero;
        rb.AddForce(dir * max_speed, ForceMode.VelocityChange);
        rb.MoveRotation(Quaternion.LookRotation(dir, Vector3.up));
        string[] Replies = new string[]
        {
            "Выполняю.\r\nвыполняю",
            "Есть!\r\nесть",
            "Вас понял!\r\nвас_понял",
            "Вас понял!\r\nвас_понял",
            "Понял.\r\nпонял",
            "Понял.\r\nпонял",
            "Принято.\r\nпринято",
            "Принято.\r\nпринято",
             "Есть!\r\nесть"
                                };
        RadioMessage rm = new RadioMessageOrder
        {
            //messageText = "Вас понял!\r\nвас_понял",
            messageText = Replies[rand.Next(Replies.Length)],
            messageVoice = Voice
        };
        //RadioSystem.SendRadioMessage(rm);
        transceiver.SendRadioMessage(rm);

    }

    new private void FixedUpdate()
    {
        base.FixedUpdate();
        if (!TryGetComponent<Rigidbody>(out myRigidBody)) return;
        if (Vector3.Distance(transform.position, MoveTarget) < (1.5 * transform.position.y)) myRigidBody.velocity = Vector3.zero;
    }

    private void OnMouseDown()
    {
        RadioMessage rm = new RadioMessageReport
        {
            messageText = "В строю.\r\nв_строю",
            dstId = RadioSystem.ForGUI,
            messageVoice = Voice
        };
        transceiver.SendRadioMessage(rm);
    }

    public void Pause(bool pause)
    {
        throw new System.NotImplementedException();
    }

    public void SetTarget(GameObject target)
    {
        Debug.Log("Attacking " + target);
    }

    public void OnRadioMessage(RadioMessage radioMessage)
    {
        //switch (radioMessage.messageClass)
        //{
        //    case 
        //}
        Debug.Log("Unit " + this.name + " Reporting!");
    }
}
