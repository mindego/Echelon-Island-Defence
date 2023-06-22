using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;
using UnityEngine.UIElements;

public class RTSCameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera ObserverCamera;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    private Canvas GUIcanvas;
    public Vector2 mapSize;
    public float defaultCameraHeight = 200f;
    private Vector3 prevMousePosition;
    public bool controlEnabled;
    private BaseObject ObjData;
    private GameObject selectedUnit;
    private RTSGUIState currentState;
    private System.Random rand = new System.Random();
    public Label SMSHud;
    private RadioTransceiver transceiver;

    //public Button MoveButton;
    //public Image Minimap;
    //void Start()
    //{

    //}

    public void OnRadioMessage(RadioMessage radioMessage)
    {
        if (radioMessage.dstId != RadioSystem.ForGUI) return;

        SMSHud.text += radioMessage.messageText;
    }
    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (ObserverCamera == null) ObserverCamera = Camera.main;
        if (!ObserverCamera.TryGetComponent<BaseObject>(out ObjData))
        {
            ObjData = ObserverCamera.gameObject.AddComponent<BaseObject>();
        }
        //Fetch GUI canvas
        GUIcanvas = GetComponentInChildren<Canvas>();
        //Fetch the Raycaster from the GameObject (the Canvas)
        //m_Raycaster = GetComponent<GraphicRaycaster>();
        m_Raycaster = GetComponentInChildren<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();
        CameraInit();
        prevMousePosition = Input.mousePosition;
        currentState = RTSGUIState.Default;

        var root = GetComponent<UIDocument>().rootVisualElement;
        SMSHud = root.Q<Label>("SMSCenter");

        if (!TryGetComponent<RadioTransceiver>(out transceiver))
        {
            transceiver = gameObject.AddComponent<RadioTransceiver>();
            transceiver.myId = RadioSystem.ForGUI;
        }
    }
    private void CameraMove() {
        Vector3 diff = (Input.mousePosition - prevMousePosition) * Time.deltaTime * 10;
        Vector3 camPos = ObserverCamera.transform.position;
        camPos.x += diff.x;
        camPos.z += diff.y;
        //camPos.y = defaultCameraHeight;
        ObserverCamera.transform.position = camPos;
        //ObjData.pos.x += diff.x;
        //ObjData.pos.z += diff.x;

    }

    private void CameraInit()
    {
        if (ObserverCamera == null) return;
        //Vector3 camPos = ObserverCamera.transform.position;
        //camPos.y = defaultCameraHeight;
        //ObserverCamera.transform.position = camPos;
        ObjData.pos = new Vector3d(0, defaultCameraHeight, 0);
        ObserverCamera.transform.rotation = Quaternion.LookRotation(Vector3.forward + Vector3.down);

    }
    private void CameraMoveMinimap(GameObject minimap)
    {
        Vector2 observerCoords = MinimapCoords(minimap, m_PointerEventData.position);
        //ObserverCamera.transform.position = new Vector3(observerCoords.x * mapSize.x, defaultCameraHeight, observerCoords.y * mapSize.y);
//        Debug.Log(ObserverCamera.transform.position);
        ObjData.pos.x = observerCoords.x * mapSize.x * 64;
        ObjData.pos.z = observerCoords.y * mapSize.y * 64;
        Debug.Log(ObjData.pos);
    }

    private void CameraZoom(float mDelta)
    {
        Vector3 camPos = ObserverCamera.transform.position;
    //    float prev = camPos.y;
        camPos.y -= mDelta * Time.deltaTime * 1000;
       // Debug.Log("Zooming! " + prev + " " + camPos.y + " " +(mDelta* Time.deltaTime));
        ObserverCamera.transform.position = camPos;
    }

    private void HandleZoom()
    {
        float mDelta = Input.mouseScrollDelta.y;
        if (mDelta != 0)
        {
            //Debug.Log("Zooming! " + mDelta);
            CameraZoom(mDelta);
        }
    }

    private void HandleMouseMovement()
    {
        if (Input.GetMouseButtonDown(2))
        {
            prevMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            CameraMove();
            //prevMousePosition = Input.mousePosition;
            //Debug.Log("Mouse 2");
        }
    }

    private void HandleStates()
    {
        //if (Input.GetKeyDown(KeyCode.Mouse1))
        if (Input.GetMouseButtonDown(1))
        {
            switch (currentState)
            {
                case RTSGUIState.Default:
                    break;
                case RTSGUIState.UnitSelected:

                    RadioMessage rm = new RadioMessageOrder
                    {
                        messageText = "Выбор юнита очищен.\r\nSuggestedWaypoint",
                        messageVoice = 0
                    };
                    RadioSystem.SendRadioMessage(rm);
                    selectedUnit = null;
                    currentState = RTSGUIState.Default;
                    break;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {

        }
    }
    private void HandleInputs()
    {
        if (!controlEnabled) return;

        HandleZoom();
        HandleMouseMovement();
        HandleStates();

    }
    private void HandleInputsCanvas()
    {
        if (!controlEnabled) return;
        float mDelta = Input.mouseScrollDelta.y;
        if (mDelta != 0)
        {
            //Debug.Log("Zooming! " + mDelta);
            CameraZoom(mDelta);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) | Input.GetMouseButtonDown(2))
        {
            prevMousePosition = Input.mousePosition;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            switch (currentState)
            {
                case RTSGUIState.Default:
                    break;
                case RTSGUIState.UnitSelected:
                   
                    RadioMessage rm = new RadioMessageOrder
                    {
                        messageText = "Чисто. \r\nчисто",
                        messageVoice = 2
                    };
                    RadioSystem.SendRadioMessage(rm);
                    selectedUnit = null;
                    currentState = RTSGUIState.Default;
                    break;
            }
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            CameraMove();
        }

        if (Input.GetMouseButton(2))
        {
            CameraMove();
            //prevMousePosition = Input.mousePosition;
            //Debug.Log("Mouse 2");
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject target = null;
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            if (results.Count == 0)
            {
                Ray screenRay = ObserverCamera.ScreenPointToRay(m_PointerEventData.position);
                RaycastHit hit;

                if (Physics.Raycast(screenRay, out hit))
                {
                    //target = hit.point; 
                    target = hit.transform.gameObject;
                    Debug.Log("Hit " + target);
                    BaseUnit unit;
                    bool UnitHit = target.TryGetComponent(out unit);

                    switch (currentState)
                    {
                        case RTSGUIState.Default:
                            {
                                if (!UnitHit) break;
                                selectedUnit = target;
                                RadioMessage rm = new RadioMessageReport
                                {
                                    messageText = "В строю.\r\nв_строю",
                                    messageVoice=2
                                };
                                RadioSystem.SendRadioMessage(rm);
                                //AudioClip audioClip = GameDataHolder.GetResource<AudioClip>(PackType.Voice1DB, "все_системы_в_норме_жду_указаний");
                                //AudioSource aus;
                                //if (!selectedUnit.TryGetComponent<AudioSource>(out aus))
                                //    {

                                //        aus = selectedUnit.AddComponent<AudioSource>();
                                //    }
                                //aus.clip = audioClip;
                                //aus.Play();
                                currentState = RTSGUIState.UnitSelected;
                                break;
                            }
                        case RTSGUIState.UnitSelected:
                            {

                                selectedUnit.TryGetComponent(out BaseCarrier carrier);
                                if (!UnitHit)
                                {
                                    carrier.MoveTo(new Vector3(hit.point.x, selectedUnit.transform.position.y, hit.point.z), 0, 100);
                                } else
                                {
                                    carrier.SetTarget(target);
                                    carrier.MoveTo(target.transform.position, 0, 100);
                                }
                                Debug.Log("Move to " + hit.point);
                                selectedUnit = null;
                                currentState = RTSGUIState.Default;
                                break;
                            }

                    }
                    //if (target.TryGetComponent(out unit))
                    //{
                    //    selectedUnit = target;
                    //    currentState = new UnitSelectedState();
                    //    AudioClip audioClip = GameDataHolder.GetResource<AudioClip>(PackType.Voice1DB, "все_системы_в_норме_жду_указаний");
                    //    AudioSource aus;
                    //    if (!selectedUnit.TryGetComponent<AudioSource>(out aus))
                    //    {

                    //        aus = selectedUnit.AddComponent<AudioSource>();
                    //    }
                    //    aus.clip = audioClip;
                    //    aus.Play();
                    //}
                    //else if (selectedUnit != null)
                    //{
                    //    selectedUnit.TryGetComponent(out BaseCarrier carrier);
                    //    AudioClip audioClip = GameDataHolder.GetResource<AudioClip>(PackType.Voice1DB, "вас_понял_двигаюсь");
                    //    AudioSource aus;
                    //    if (!selectedUnit.TryGetComponent<AudioSource>(out aus))
                    //    {

                    //        aus = selectedUnit.AddComponent<AudioSource>();
                    //    }
                    //    aus.clip = audioClip;
                    //    aus.Play();
                    //    carrier.MoveTo(new Vector3(hit.point.x, selectedUnit.transform.position.y, hit.point.z), 0, 100);
                    //    Debug.Log("Move to " + hit.point);
                    //    selectedUnit = null;
                    //}
                }
            }

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                Debug.Log("Hit on" + result.gameObject.name);
                /*Vector2 observerCoords = MinimapCoords(result.gameObject,m_PointerEventData.position);
                ObserverCamera.transform.position = new Vector3(observerCoords.x * mapSize.x, defaultCameraHeight, observerCoords.y * mapSize.y);
                Debug.Log(ObserverCamera.transform.position);*/
                //if (result.gameObject.GetComponent<Image>() == Minimap) CameraMoveMinimap(result.gameObject);
            }
        }

    }
    // Update is called once per frame
    void Update()
    {
        HandleInputs();
    }

    private Vector2 MinimapCoords(GameObject minimap, Vector3 hitPos)
    {
        Vector2 minimapPosition;
        RectTransform rect = minimap.GetComponent<RectTransform>();
        Camera self = GetComponent<Camera>();

        //Vector3 minimapCameraPosition=self.WorldToScreenPoint(rect.position);
        //Debug.Log("mmap: " + minimapCameraPosition);
        //Debug.Log("hit: " + hitPos);
        //Debug.Log("size: " + rect.sizeDelta);

        //minimapPosition = (Vector2) (pos - minimapCameraPosition) + new Vector2(100, 100);
        //minimapPosition = (Vector2)(hitPos - minimapCameraPosition) + rect.sizeDelta;
        //minimapPosition /= 100;
        //Debug.Log(minimapPosition + new Vector2(50,50));
        //Debug.Log(minimapPosition);
        minimapPosition = new Vector2(hitPos.x / rect.sizeDelta.x, hitPos.y / rect.sizeDelta.y);
        Debug.Log(minimapPosition);
        return minimapPosition;
    }
}

public enum RTSGUIState
{
    Default,
    UnitSelected
}
public class RTSStateMachine {
    private State state;
}

public class State
{

}
public class DefaultState :State
{
    
}

public class UnitSelectedState : State
{

}
