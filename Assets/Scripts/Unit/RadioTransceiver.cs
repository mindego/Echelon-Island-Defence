using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RadioTransceiver : MonoBehaviour
{
    public int myId;
    private AudioSource Speakers;
    //private Text SMS;
    private Label SMSwindow;

    private void Awake()
    {
        myId = RadioSystem.Subscribe(this);
        Speakers = GetComponent<AudioSource>();
    }

    private void Start()
    {
        InitTextMessenger();
    }


    private void InitTextMessenger()
    {
        //var root = GetComponent<UIDocument>().rootVisualElement;
        if (!TryGetComponent<UIDocument>(out UIDocument myUIDocument)) return;
        var root = myUIDocument.rootVisualElement;

        SMSwindow = root.Q<Label>("SMSCenter");
        //SMSwindow.text += name + " subscribed";
    }
    private void OnDestroy()
    {
        RadioSystem.Unsubscribe(this);
    }
    internal void OnMessage(RadioMessage radioMessage)
    {
        //if (radioMessage.dstId != myId) return;
        if (Speakers != null)
        {
            VocalizeRadioMessage(radioMessage);
        }
        //Debug.Log($"I am {name}. Message received: {radioMessage.messageText}");
        Debug.Log($"Unit {name} id {myId} processing radio message from {radioMessage.srcId} to {radioMessage.dstId} ");
        //if (SMSwindow != null)  SMSwindow.text = radioMessage.messageText;
        if (radioMessage.dstId != myId) return;
        Debug.Log("Message sent!");
            SendMessage("OnRadioMessage",radioMessage,SendMessageOptions.DontRequireReceiver);
    }

    private void VocalizeRadioMessage(RadioMessage radioMessage)
    {
        string[] separators = new string[] {"\r\n"};
        string[] message = radioMessage.messageText.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        if (SMSwindow != null) SMSwindow.text = message[0];
        if (message.Length < 2) return;
        if (Speakers == null) return;

        PackType voicepack = PackType.Voice1DB;
        switch (radioMessage.messageVoice)
        {
            case 1:
                voicepack = PackType.Voice1DB;
                break;
            case 2:
                voicepack = PackType.Voice2DB;
                break;
            case 3:
                voicepack = PackType.Voice0DB;
                break;

        }
        //Debug.Log("using voice " + voicepack);
        AudioClip audioClip = GameDataHolder.GetResource<AudioClip>(voicepack, message[1]);
        if (audioClip == null) return;
        Speakers.clip = audioClip;
        Speakers.Play();
    }

    internal void SendRadioMessage(RadioMessage message)
    {
        if (message.srcId == -1) message.srcId = myId;
        RadioSystem.SendRadioMessage(message);
    }

    
}
