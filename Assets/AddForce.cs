using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AddForce : MonoBehaviour
{
    public Vector3 force;
    public float DesiredAltitude;
    private Rigidbody myRigidbody;
    
    public double x, y, z;

    #region StartCoords
    [Header("Start Coordinates")]
    public double sX;
    public double sY;
    public double sZ;
    #endregion

    public int KickForce = 500;
    public List<Waypoint> waypoints;
    private BaseObject stdObject;
    public Text HUD;
    public Text LocalCoords;
    public myForceMode ForceModeSelect;

    // Start is called before the first frame update
    void Start()
    {
        if (!TryGetComponent<BaseObject>(out stdObject)) return;
        myRigidbody = GetComponent<Rigidbody>();

       
        stdObject.pos = new Vector3d(sX, sY, sZ);
        
        switch (ForceModeSelect)
        {
            case myForceMode.waypoints:
                LoadWaypoints();
                kickToWaypoint(GetCurrentWaypoint());
                break;
            case myForceMode.staticForce:
                AddForceToRigidbody(force);
                break;
        }
    }

    public enum myForceMode
    {
        waypoints,
        staticForce
    }
    private void kickToWaypoint(Waypoint wp)
    {
        
        myRigidbody.velocity = Vector3.zero;
        Vector3 dirToWaypoint = wp.coords - stdObject.pos.ToVector3();
        AddForceToRigidbody(dirToWaypoint.normalized * KickForce);
        Debug.Log("Moving to " + wp.name + " -> " +  dirToWaypoint.normalized * KickForce);
    }
    private async void LoadWaypoints()
    {
        string filename = "Assets/waypoints.xml";
        if (File.Exists(filename))
        {
            Debug.Log("Loading XML");
            waypoints = StormFileUtils.LoadXML<List<Waypoint>>(filename);
            return;
        }
        string url = "https://berendeevdom.ru/Storm/Unity/GetWaypoints.php";
        waypoints = new List<Waypoint>();
        var w = UnityWebRequest.Get(url);
        w.SendWebRequest();

        while (!w.isDone)
        {
            await Task.Yield();
        }
        Debug.Log(w.result);
        byte[] fileContent = w.downloadHandler.data;

        //MemoryStream ms=new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        MemoryStream ms = new MemoryStream(fileContent);
        StreamReader sr = new StreamReader(ms);
        while (!sr.EndOfStream)
        {
            string[] waypointDataArray = sr.ReadLine().Split(',');
            Waypoint waypoint = new Waypoint();
            waypoint.name = waypointDataArray[0];
            waypoint.category = 0;
            waypoint.passed = false;
            waypoint.coords = new Vector3(float.Parse(waypointDataArray[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(waypointDataArray[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse(waypointDataArray[3], CultureInfo.InvariantCulture.NumberFormat));
            waypoints.Add(waypoint);
        }
        StormFileUtils.SaveXML(filename, waypoints);
        sr.Close();
        ms.Close();
    }

    public struct Waypoint
    {
        public string name;
        public int category;
        public bool passed;
        public Vector3 coords;
    }
    // Update is called once per frame

    private void FixedUpdate()
    {
        HoldAltitude();

        switch (ForceModeSelect)
        {
            case myForceMode.waypoints:
                HandleWaypoints();
                break;
            case myForceMode.staticForce:
                break;
        }
    }

    public void SetLocalCoordinates(Vector3 coords)
    {
        LocalCoords.text = coords.ToString();
    }
    private void HandleWaypoints()
    {
        Waypoint wp = GetCurrentWaypoint();
        if (wp.category == 0xFF)
        {
            myRigidbody.velocity = Vector3.zero;
            return;
        }


        float distance = Vector2.Distance(new Vector2(wp.coords.x, wp.coords.z), new Vector2((float)stdObject.pos.x, (float)stdObject.pos.z));
        //if (distance % 100 ==0) Debug.Log("Distance " + distance);
        HUD.text = distance.ToString();
        
        if (distance < 100)
        {
            if (waypoints.Contains(wp))
            {
                int index=waypoints.IndexOf(wp);
                wp.passed = true;
                waypoints[index] = wp;
            }
            Debug.Log("Arrived at " + wp.name);
            kickToWaypoint(GetCurrentWaypoint());
        }
    }

    private Waypoint GetCurrentWaypoint()
    {
        Waypoint wpDone = new Waypoint();
        wpDone.name = "Done";
        wpDone.category = 0xFF;
        foreach (Waypoint wp in waypoints)
        {
            if (wp.passed == false) return wp;
        }
        return wpDone;        
    }
    private void AddForceToRigidbody(Vector3 force)
    {
        if (myRigidbody == null) return;

        myRigidbody.MoveRotation(Quaternion.LookRotation(force, Vector3.up));
        myRigidbody.AddForce(force,ForceMode.VelocityChange);
    }

    private void HoldAltitude()
    {
        if (myRigidbody == null) return;
        RaycastHit hit;
        Ray rayDown = new Ray(transform.position, Vector3.down);
        Ray rayUp = new Ray(transform.position, Vector3.up);
        Physics.Raycast(rayDown, out hit);
        Debug.DrawRay(transform.position, Vector3.down * DesiredAltitude, Color.red);

        
        if (hit.collider != null)
        {
            Vector3 pos = transform.position;
            pos.y += DesiredAltitude - hit.distance;
            myRigidbody.MovePosition(pos);
            Vector3 localUp = Vector3.ProjectOnPlane(hit.normal, transform.right);
            Vector3 localForward = Vector3.Lerp(myRigidbody.velocity,Vector3.ProjectOnPlane(myRigidbody.velocity, localUp),0.1f);
            
            Debug.DrawRay(transform.position, localUp * 100,Color.green); 
            Debug.DrawRay(transform.position, localForward * 100, Color.blue);
            this.transform.rotation=Quaternion.LookRotation(localForward,localUp);
        } else
        {
            Physics.Raycast(rayUp, out hit);
            if (hit.collider != null)
            {
                Debug.Log("hit Up!");
                Vector3 pos = transform.position;
                pos.y += DesiredAltitude + hit.distance;
                myRigidbody.MovePosition(pos);
            }
        }
    }
}
