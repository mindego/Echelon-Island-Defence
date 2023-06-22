using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Shit : MonoBehaviour
{
    public WaterSurface targetSurface = null;
    private Rigidbody myRigidBody;
    public float multiplier = 1;

    // Internal search params
    WaterSearchParameters searchParameters = new WaterSearchParameters();
    WaterSearchResult searchResult = new WaterSearchResult();

    private void Awake()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        //FloatShit();
        FloatShitAdvanced();
    }

    private void FloatShitAdvanced()
    {
        if (targetSurface == null) return;
        if (myRigidBody == null) {
            PlaceOnWater();
            return;
        };
        int offset = 50;

        //Vector3 bowPos = transform.position + transform.forward * offset*4;
        //Vector3 sternPos= transform.position - transform.forward * offset*4;
        //Vector3 starPos = transform.position + transform.right * offset;
        //Vector3 portPos = transform.position - transform.right * offset;
        Vector3 bowPos = transform.position + myRigidBody.centerOfMass + transform.forward * offset * 4;
        Vector3 sternPos = transform.position + myRigidBody.centerOfMass - transform.forward * offset * 4;
        Vector3 starPos = transform.position + myRigidBody.centerOfMass + transform.right * offset*2;
        Vector3 portPos = transform.position + myRigidBody.centerOfMass - transform.right * offset*2;
        Vector3 centerPos = transform.position + myRigidBody.centerOfMass;

        Vector3 centerForce = GetArchimedian(bowPos) * 1f;
        Vector3 bowForce = GetArchimedian(centerPos) * 0.05f;
        Vector3 sternForce = GetArchimedian(sternPos) * 0.05f;
        Vector3 starForce = GetArchimedian(starPos) * 0.05f;
        Vector3 portForce = GetArchimedian(portPos) * 0.05f;


        myRigidBody.AddForceAtPosition(centerForce, centerPos);
        myRigidBody.AddForceAtPosition(bowForce, bowPos);
        myRigidBody.AddForceAtPosition(sternForce, sternPos);
        myRigidBody.AddForceAtPosition(starForce, starPos);
        myRigidBody.AddForceAtPosition(portForce, portPos);


        Debug.DrawRay(centerPos, centerForce,Color.cyan);
        Debug.DrawRay(bowPos, bowForce);
        Debug.DrawRay(sternPos, sternForce);
        Debug.DrawRay(starPos, starForce);
        Debug.DrawRay(portPos, portForce);


    }

    private Vector3 GetArchimedian(Vector3 pos)
    {
        if (targetSurface == null) return Vector3.zero;
        searchParameters.startPosition = searchResult.candidateLocation;
        searchParameters.targetPosition = pos;
        searchParameters.error = 0.01f;
        searchParameters.maxIterations = 8;
        Vector3 force = Vector3.zero;

        if (targetSurface.FindWaterSurfaceHeight(searchParameters, out searchResult))
        {
            if (pos.y < searchResult.height) {
                // force = myRigidBody.mass * -Physics.gravity * Time.deltaTime * multiplier;
                //force = -Physics.gravity;
                force = myRigidBody.mass * -Physics.gravity * multiplier;
            }
        }

        return force;
    }

    private void PlaceOnWater()
    {
        if (targetSurface != null)
        {
            // Build the search parameters
            searchParameters.startPosition = searchResult.candidateLocation;
            searchParameters.targetPosition = gameObject.transform.position;
            searchParameters.error = 0.01f;
            searchParameters.maxIterations = 8;

            // Do the search
            if (targetSurface.FindWaterSurfaceHeight(searchParameters, out searchResult))
            {
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, searchResult.height, gameObject.transform.position.z);
            }
            //else Debug.LogError("Can't Find Height");
        }
    }
    private void FloatShit()
    {
        if (targetSurface != null)
        {
            // Build the search parameters
            searchParameters.startPosition = searchResult.candidateLocation;
            searchParameters.targetPosition = gameObject.transform.position;
            searchParameters.error = 0.01f;
            searchParameters.maxIterations = 8;

            // Do the search
            if (targetSurface.FindWaterSurfaceHeight(searchParameters, out searchResult))
            {
                //Debug.Log(searchResult.height);
                if (myRigidBody == null)
                {
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, searchResult.height, gameObject.transform.position.z);
                } else
                {
                    if (searchResult.height > transform.position.y) myRigidBody.AddForce(myRigidBody.mass * -Physics.gravity);
                }

            }
            else Debug.LogError("Can't Find Height");
        }
    }
}
