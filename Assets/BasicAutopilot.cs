using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAutopilot : MonoBehaviour
{
    public float TargetAltitude;
    public float TargetSpeed;
    public Vector3 TargetPosition;
    public Quaternion TargetRotation;
    public bool levelHorizon = false;
    public bool showForces = false;
    private Rigidbody myRigidbody;
    
    private Vector3 currentForce = Vector3.zero;
    private Vector3 currentTorque = Vector3.zero;
    private Vector3 hoverForce = Vector3.zero;
    private Vector3 gForce = Vector3.zero;
    
    private PIDControllerVector pidMovement;
    private PIDControllerVector pidRotation;
    public float proportionalGain, integralGain, derivativeGain;
    public GameObject target;

    public GameObject explosion;
    public GameObject smoke;

    private BaseCarrier Carrier;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        TryGetComponent<BaseCarrier>(out Carrier);
        if (TryGetComponent<Rigidbody>(out myRigidbody))
        {

            currentForce = Physics.gravity * myRigidbody.mass * -1;
            hoverForce = currentForce;

            //pid = new PIDcontroller()
            //{
            //    proportionalGain = proportionalGain,
            //    integralGain = integralGain,
            //    derivativeGain = derivativeGain
            //};
            pidMovement = new PIDControllerVector()
            {
                proportionalGain = proportionalGain,
                integralGain = integralGain,
                derivativeGain = derivativeGain
            };

            pidRotation = new PIDControllerVector()
            {
                proportionalGain = proportionalGain,
                integralGain = integralGain,
                derivativeGain = derivativeGain
            };
        }
        Debug.Log("Data class: " + Carrier._carrierData.GetClass() + " Flags " + Carrier._carrierData.Flags.ToString("X8"));
    }

    private void FixedUpdate()
    {
        UpdateTargetPosition();
        UnitControl();
    }

    private void UnitControl()
    {
        if (myRigidbody == null) return;


        UnitControlTorque();
        UnitControlThrust();

        myRigidbody.AddForce(currentForce);
        myRigidbody.AddTorque(currentTorque);

        if (showForces) {
            Debug.DrawRay(transform.position, currentTorque.normalized * currentTorque.magnitude, Color.red);
            Debug.DrawRay(transform.position, currentForce.normalized * currentForce.magnitude, Color.cyan);
        }
        //Debug.Log(currentTorque);
    }
    private void UpdateTargetPosition()
    {
        if (TargetPosition == target.transform.position) return;
        TargetPosition = target.transform.position;
    }
    private Vector3 GetLeveledDir(Vector3 WorldTargetPosition)
    {
        Vector3 TargetUp = Vector3.up;
        
        Vector3 direction = (WorldTargetPosition - transform.position).normalized;
        Vector3 TargetDir = Vector3.ProjectOnPlane(direction, TargetUp);

        return TargetDir;
    }

    private Vector3 GetLeveledPosition(Vector3 WorldTargetPosition)
    {
        Vector3 TargetUp = Vector3.up;

        Vector3 direction = WorldTargetPosition - transform.position;
        Vector3 TargetDir = Vector3.ProjectOnPlane(direction, TargetUp);

        return transform.position + TargetDir;
    }

    private void UnitControlTorque() {
        currentTorque = Vector3.zero;
        //Quaternion tmpTargetRotation = Quaternion.identity;
        Vector3 tmpTorque = Vector3.zero;
        if (levelHorizon)
        {
            Vector3 TargetUp = Vector3.up;
            Vector3 TargetDir = Vector3.ProjectOnPlane(transform.forward, TargetUp).normalized;
            Vector3 TargetRight = Vector3.ProjectOnPlane(transform.right, TargetUp).normalized;

            Debug.DrawLine(transform.position, transform.position + 1000 * TargetDir, Color.blue);
            Debug.DrawLine(transform.position, transform.position + 1000 * TargetUp, Color.green);

            //Debug! Use the force, Luke!
            //myRigidbody.MoveRotation(Quaternion.LookRotation(TargetDir, TargetUp));

            TargetRotation *= Quaternion.LookRotation(TargetDir, TargetUp);
            //myRigidbody.MoveRotation(TargetRotation);

            //currentTorque += Vector3.Cross(transform.forward, TargetDir)*1000;
            //currentTorque += Vector3.Cross(transform.right, TargetRight) * 1000;

            tmpTorque += Vector3.Cross(transform.forward, TargetDir);
            tmpTorque += Vector3.Cross(transform.right, TargetRight);
        }
        if (target == null) return;
        //Debug! Use the force, Luke!
        //myRigidbody.MoveRotation(transform.localRotation * Quaternion.FromToRotation(transform.forward, direction.normalized));
        //transform.rotation = Quaternion.LookRotation(direction);
        //myRigidbody.MoveRotation(Quaternion.LookRotation(direction));

        //Vector3 direction = (target.transform.position - transform.position).normalized;
        //Debug.DrawRay(transform.position, direction*1000, Color.white);
        //Debug.DrawLine(transform.position, GetLeveledPosition(target.transform.position),Color.red);

        //TargetRotation = Quaternion.LookRotation(GetLeveledDir(target.transform.position));

        // currentTorque = Vector3.Cross(transform.forward, GetLeveledDir(TargetPosition)*1000);
        //transform.rotation = TargetRotation;

        tmpTorque += pidRotation.Update(Time.deltaTime,transform.forward+transform.right,tmpTorque);
       // currentTorque = tmpTorque * myRigidbody.mass * 1000;
    }
    private void UnitControlThrust()
    {
        UpdateForce();
        if (TryGetComponent<EngineVisualizer>(out EngineVisualizer vEngine))
        {
            vEngine.SetForce(currentForce);
        }
    }
    private void UpdateForce()
    {
        if (target == null) return;
        Vector3 tmpForce = pidMovement.Update(Time.deltaTime, transform.position, TargetPosition);

        Vector3 horizontalForce = Vector3.ProjectOnPlane(tmpForce, Vector3.up)*myRigidbody.mass;
        //Vector3 verticalForce = Vector3.ProjectOnPlane(tmpForce, Vector3.forward);
        Vector3 verticalForce = Vector3.Project(tmpForce, Vector3.up)*myRigidbody.mass;
        var accelerationXZ = horizontalForce.magnitude / myRigidbody.mass;
        var accelerationY = verticalForce.magnitude / myRigidbody.mass;
        
        // F=ma
        // a=F/m
        // AccelZ = F * myRigidBody.mass
        // F=AccelZ/Mass
        // accelZ (2) = F*m (10000)
        // F=AccelZ/m
        currentForce = Vector3.zero;

        //if (Mathf.Abs(accelerationXZ) > carrierData.MaxAccelZ) currentForce+= horizontalForce.normalized * carrierData.MaxAccelZ;
        //if (Mathf.Abs(accelerationY) > carrierData.MaxAccelY) currentForce+= verticalForce.normalized * carrierData.MaxAccelY;
        int multi = 1;

        currentForce += (Mathf.Abs(accelerationXZ) > Carrier._carrierData.MaxAccelZ/multi) ? horizontalForce.normalized * Carrier._carrierData.MaxAccelZ/multi : horizontalForce.normalized* accelerationXZ;
        currentForce += (Mathf.Abs(accelerationY) > Carrier._carrierData.MaxAccelY/multi) ? verticalForce.normalized * Carrier._carrierData.MaxAccelY/multi : verticalForce.normalized* accelerationY;

        currentForce *= myRigidbody.mass;
        
        if (myRigidbody.useGravity) currentForce += hoverForce;

        if (showForces)
        {
            Vector3 tmpHorizontalForce = Vector3.ProjectOnPlane(currentForce, Vector3.up);
            Vector3 tmpVerticalForce = Vector3.Project(currentForce, Vector3.up);
            var resAccelerationXZ = tmpHorizontalForce.magnitude / myRigidbody.mass;
            var resAccelerationY = tmpVerticalForce.magnitude / myRigidbody.mass;

            Debug.DrawRay(transform.position, horizontalForce * myRigidbody.mass, Color.blue);
            Debug.DrawRay(transform.position, verticalForce * myRigidbody.mass, Color.green);

            Debug.Log("Horizontal force " + horizontalForce);
            Debug.Log("Vertical force   " + verticalForce);
            Debug.Log("H-force result   " + tmpHorizontalForce);
            Debug.Log("V-force result   " + tmpVerticalForce);
            Debug.Log("Result force     " + currentForce);
            Debug.Log("Horizontal acc   " + accelerationXZ + " max " + Carrier._carrierData.MaxAccelZ / multi);
            Debug.Log("Vertical acc     " + accelerationY + " max " + Carrier._carrierData.MaxAccelY / multi);
            Debug.Log("H-acc result     " + resAccelerationXZ + " max " + Carrier._carrierData.MaxAccelZ / multi);
            Debug.Log("V-acc result     " + resAccelerationY + " max " + Carrier._carrierData.MaxAccelY / multi);
        }
    }

    
    public void MoveTo(Vector3 target)
    {
        currentForce = Vector3.zero;
        
    }

    private float GetAltitude()
    {
        //Заглушка! Проверять высоту, бросая луч вниз

        return transform.position.y;
    }
    private float getSign(float value)
    {
        if (value == 0) return 0f;

        return (value < 0) ? -1 : 1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision on " + gameObject.name);
        foreach (ContactPoint contact in collision.contacts)
        {
            GameObject tmpExplosion = Instantiate(explosion);
            tmpExplosion.name = "Kaboom!";
            tmpExplosion.transform.localScale = Vector3.one * 10;
            tmpExplosion.transform.position = contact.point;
            Destroy(tmpExplosion, 2f);

            GameObject tmpSmoke = Instantiate(smoke);
            tmpSmoke.transform.position = contact.point;
            tmpSmoke.transform.parent = contact.thisCollider.transform;

        }
    }
}

public class PIDControllerVector : PIDcontroller
{
    private Vector3 lastError=Vector3.zero;
    private Vector3 lastValue=Vector3.zero;
    private Vector3 integrationStored;

    public Vector3 Update(float dt,Vector3 CurrentValue,Vector3 TargetValue)
    {
        Vector3 error = TargetValue - CurrentValue;
        Vector3 P = proportionalGain * error;

        Vector3 valueRateOfChange = (CurrentValue - lastValue) / dt;
        Vector3 D = derivativeGain * -valueRateOfChange;
        lastValue = CurrentValue;

        integrationStored += error * dt;
        Vector3 I = integralGain * integrationStored;

        return P + I + D;
    }
}
public class PIDcontroller 
{
    public float proportionalGain;
    public float integralGain;
    public float derivativeGain;

    private float lastError;
    private float lastValue = 0;
    private float integrationStored;

    public void setGain(float pg,float ig,float dg)
    {
        proportionalGain = pg;
        integralGain = ig;
        derivativeGain = dg;
    }

    public float Update(float dt, float CurrentValue, float TargetValue)
    {
        float error = TargetValue - CurrentValue;
        float P = proportionalGain * error;

        //float errorRateOfChange = (error - lastError) / dt;
        //float D = derivativeGain * errorRateOfChange;
        //lastError = error;
        float valueRateOfChange = (CurrentValue - lastValue) / dt;
        float D = derivativeGain * -valueRateOfChange;
        lastValue = CurrentValue;

        integrationStored = integrationStored + (error * dt);
        float I = integralGain * integrationStored;

        float result = P + I + D;
        Debug.Log((P, I, D));
        return result;
    }
}
