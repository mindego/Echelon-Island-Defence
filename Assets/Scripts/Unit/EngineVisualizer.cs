using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineVisualizer : MonoBehaviour
{
    public GameObject DefaultDuza;
    public AudioClip sound;
    public List<GameObject> Duzas = new List<GameObject>();
    public Vector3 ResultForce;
    private Rigidbody myRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject Duza = Instantiate<GameObject>(DefaultDuza);
        //Duza.transform.parent = transform;
        //Duza.transform.localPosition = Vector3.zero;
        //Duzas.Add(Duza);
        
    }

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }
    public void AddDuza(FpoImport.FPOSlot Duza,Transform myParent=null)
    {
        GameObject tmpDuza = Instantiate<GameObject>(DefaultDuza);
        tmpDuza.name = "Duza at slot " + Duza.slotId.ToString();
        tmpDuza.transform.parent = (myParent != null) ? myParent:transform;
        tmpDuza.transform.position = myParent.position;
        tmpDuza.transform.rotation = tmpDuza.transform.parent.rotation;
        Duzas.Add(tmpDuza);
        Debug.Log("Added Duza " + Duza.slotId.ToString());
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetForce(Vector3 setForce)
    {
        ResultForce = setForce;
    }

    private void FixedUpdate()
    {
        var magnitude = ResultForce.magnitude;
        foreach (GameObject Duza in Duzas)
        {
            //Vector3 ResultForceNormalized = ResultForce.normalized;
            //Duza.transform.rotation = ResultForceNormalized == Vector3.zero ? transform.rotation: Quaternion.LookRotation(ResultForce.normalized);
            Duza.transform.localScale = Duza.transform.rotation * myRigidbody.velocity.normalized * 10;
        }
    }
}
