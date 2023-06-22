using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    float forwardInput, rotationInput;
    bool jump;
    bool run;
    bool fire;
    
    private float prevFired = 0;
    public GameObject explosion;
    public bool controlEnabled;
    private bool prevControlEnabled;
    private float xRotation = 0f; 
    private float yRotation = 0f;
    Rigidbody rb;
    public float jumpSpeed = 10;
    public float defaultSpeed=100;
    public float defaultRotationSpeed = 100;
    public float runSpeedMod = 100;

    public float sensX = 100f;
    public float sensY = 100f;
    public Camera observerCamera;
    public Transform cameraSlot;
    public Transform cannonSlot;
    // Start is called before the first frame update
    void Start()
    {
        Init();


    }
    private void Init()
    {
        observerCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        if (!controlEnabled)
        {
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        prevControlEnabled = controlEnabled;
        Cursor.lockState = CursorLockMode.Locked;

        if (cameraSlot == null)
        {
            observerCamera.transform.parent = transform;
            observerCamera.transform.localPosition = Vector3.up;
            observerCamera.transform.rotation = transform.rotation;
        } else
        {
            observerCamera.transform.parent = cameraSlot;
            //observerCamera.transform.position = cameraSlot.position + Vector3.up * 0.5f - Vector3.forward * 0.5f;
            observerCamera.transform.position = cameraSlot.position;
            observerCamera.transform.rotation = cameraSlot.rotation;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
        if (prevControlEnabled == false) Init();
        HandleInputs();
        if (!controlEnabled) return;
        HandleMovement();
        HandleFire();
    }

    private void FixedUpdate()
    {
        //HandleMovement();
        //HandleFire();
    }

    protected void HandleInputs()
    {
        forwardInput = Input.GetAxis("Vertical");
        rotationInput = Input.GetAxis("Horizontal");
        jump = Input.GetKey(KeyCode.Space);
        run = Input.GetKey(KeyCode.LeftShift);
        fire = Input.GetKey(KeyCode.Mouse0);
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Debug.Log((controlEnabled, !controlEnabled));
            controlEnabled = !controlEnabled;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RadioTransceiver radioTransceiver;
            if (!TryGetComponent<RadioTransceiver>(out radioTransceiver))
            {
                radioTransceiver = gameObject.AddComponent<RadioTransceiver>();
            }
            RadioMessage message = new RadioMessageOrder()
            {
                dstId = 1,
                srcId = 1,
                messageId = 0xFF,
                messageText = "Open,Sesame!"
            };
            radioTransceiver.SendRadioMessage(message);
        }

        float mouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime; 
        float mouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime; 
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        yRotation += mouseX;
    }

    private void HandleFire()
    {
        if (!fire) return;
        float time = Time.time;
        if ((time - prevFired)< 1f) return;

        prevFired = time;
        GameObject cannonball = Cannonball.GetCannonball(explosion);
        Collider cannonballCollider = cannonball.GetComponent<Collider>();
        Transform shooter = (cannonSlot == null) ? transform : cannonSlot.transform;
        if (shooter.TryGetComponent<Collider>(out Collider shooterCollider)) Physics.IgnoreCollision(cannonballCollider, shooterCollider);
        
        var cbScript = cannonball.GetComponent<Cannonball>();

        //cbScript.Fire(shooter.TransformPoint(shooter.localPosition - 15*Vector3.up), shooter.rotation);
        cbScript.Fire(shooter.transform.position, shooter.rotation);

        Destroy(cannonball, 30);
    }

    
    protected void HandleMovement()
    {
        float force = (run) ? defaultSpeed * runSpeedMod : defaultSpeed;
        Vector3 forwardForce = forwardInput * transform.forward * Time.deltaTime * force;
        Vector3 strafeForce = rotationInput * transform.right * Time.deltaTime * force;
        Vector3 jumpForce = Vector3.zero;
        Vector3 hoverForce = Vector3.zero;


        //Quaternion wantedRotation = transform.rotation * Quaternion.Euler(Vector3.up * (defaultRotationSpeed * rotationInput * Time.deltaTime));
        Quaternion wantedRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        Quaternion baseRotation = Quaternion.Euler(0f, yRotation, 0f);
        rb.MoveRotation(baseRotation);
        cameraSlot.rotation = wantedRotation;
        //rb.AddTorque(Quaternion.Euler(0,rotationInput * Time.deltaTime,0));
        //rb.AddTorque(new Vector3(0, 1, 0));
        if (jump)
        {
            jumpForce = transform.up * Time.deltaTime * jumpSpeed;
        }

        if (rb.useGravity)
        {
            // rb.AddForce(-Physics.gravity, ForceMode.Acceleration);
            hoverForce = rb.mass * Physics.gravity * Time.deltaTime * -1;
        }

        //Debug.Log((jumpForce,hoverForce,forwardForce,strafeForce));
        rb.AddForce(forwardForce + strafeForce + jumpForce + hoverForce);
        
    }
}

public class Cannonball : MonoBehaviour
{
    public GameObject explosion;
    private Rigidbody rg;
    private GameObject cannonball;
    public static int count = 0;

    public Cannonball(GameObject explosion)
    {
        this.explosion = explosion;
    }

    public void Fire(Vector3 pos,Quaternion dir)
    {
        if (rg == null) return;

        cannonball.transform.position = pos;
        cannonball.transform.rotation = dir;
        rg.AddForce(cannonball.transform.forward * 1000);
    }

    public static GameObject GetCannonball(GameObject explosion)
    {
        GameObject cannonball = new GameObject
        {
            name = "Cannonball " + count++,
        };
        Mesh mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
        MeshFilter mf = cannonball.AddComponent<MeshFilter>();
        mf.mesh = mesh;

        SphereCollider mc = cannonball.AddComponent<SphereCollider>();
        var mr = cannonball.AddComponent<MeshRenderer>();

        //Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        Shader shader = Shader.Find("HDRP/Lit");
        mr.material = new Material(shader);

        Cannonball cbScript = cannonball.AddComponent<Cannonball>();
        cbScript.explosion = explosion;

        cbScript.rg = cannonball.AddComponent<Rigidbody>();
        cbScript.rg.useGravity = false;
        cbScript.cannonball = cannonball.gameObject;
        return cannonball;
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach(ContactPoint contactPoint in collision.contacts)
        {
            var tmpExplosion = Instantiate<GameObject>(explosion);
            tmpExplosion.transform.position = contactPoint.point;
            //tmpExplosion.transform.rotation = Quaternion.LookRotation(Vector3.up);
            tmpExplosion.transform.rotation = Quaternion.LookRotation(contactPoint.normal);

            Destroy(tmpExplosion, 10);
        }

        rg.useGravity = true;
    }

    
}
