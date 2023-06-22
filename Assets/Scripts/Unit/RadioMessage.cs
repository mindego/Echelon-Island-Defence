using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioMessage
{
    public int srcId =  -1, dstId=-1;
    public int messageId;
    public int messageClass;
    public string messageText;
    public int messageVoice;

    public int channel;
};
public class RadioMessageReport : RadioMessage
{

}

public class RadioMessageOrder : RadioMessage
{
    public Vector3 target;

}

public class RadioMessageQuery : RadioMessage
{

}
