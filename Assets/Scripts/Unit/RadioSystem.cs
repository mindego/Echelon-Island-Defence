using System.Collections.Generic;
using UnityEngine;

public class RadioSystem
{
    public static RadioSystem instance;
    public static int StationsCount;
    public List<RadioTransceiver> radioTransceivers = new List<RadioTransceiver>();
    public const int ForGUI = -1;

    public static RadioSystem getInstance()
    {
        if (instance == null) instance = new RadioSystem();
        return instance;
    }

    public static int Subscribe(RadioTransceiver radioTransceiver)
    {
        RadioSystem instance = getInstance();
        if (instance.radioTransceivers.Contains(radioTransceiver)) instance.radioTransceivers.IndexOf(radioTransceiver);
        Debug.Log("New transceiver: "  + radioTransceiver.name);
        instance.radioTransceivers.Add(radioTransceiver);
        Debug.Log(radioTransceiver.name + " ID " + instance.radioTransceivers.IndexOf(radioTransceiver));
        return instance.radioTransceivers.IndexOf(radioTransceiver);
    }

    public static void Unsubscribe(RadioTransceiver radioTransceiver)
    {
        getInstance().radioTransceivers.Remove(radioTransceiver);
    }

    public static void SendRadioMessage(RadioMessage radioMessage)
    {
        foreach(RadioTransceiver radioTransceiver in getInstance().radioTransceivers)
        {
            radioTransceiver.OnMessage(radioMessage);
        }
    }
}
