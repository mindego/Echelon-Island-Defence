using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ObjSpawner : MonoBehaviour
{
    public string[] objectNames;
    //public GameObject[] Prefabs;
    private int index=0;
    public Vector3 objectDir;
    private Vector3 objectOrigin;
    public int objectTTL;
    public int objectSpeed;
    public int period;
    private float myTime,newTime;
    public bool spawnerEnabled = true;
    private Dictionary<string, GameObject> Prototypes = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        newTime = period;
        objectOrigin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void timedEvent()
    {
        newTime += Time.deltaTime;
        if ((newTime - myTime) > period)
        {
            GenObject();
            myTime = newTime;
        }
    }

    private void FixedUpdate()
    {
        timedEvent();
    }

    private GameObject GetGameObject(string name)
    {
        GameObject res;

        if (!Prototypes.ContainsKey(name))
        {
            PrefabBuilderEditor pbe = ScriptableObject.CreateInstance<PrefabBuilderEditor>();
            res = pbe.CreateGameobject(objectNames[index], objectOrigin);
            res.SetActive(false);
            Prototypes.Add(name, res);
        }

        return Instantiate(Prototypes[name]);
    }
    private void GenObject()
    {
        if (!spawnerEnabled) return;
//        Debug.Log("Generating ");
        System.Random random = new System.Random();
        //int index = random.Next(0, objectNames.Length);
        //GameObject FPO = GameDataHolder.GetResource<GameObject>(PackType.FPODB, objectNames[index]);
        //PrefabBuilderEditor pbe = new PrefabBuilderEditor();
        //GameObject FPO = pbe.CreateGameobject(objectNames[index], objectOrigin);
        GameObject FPO = GetGameObject(objectNames[index]);
        FPO.SetActive(true);
        //TrailRenderer trail = FPO.AddComponent<TrailRenderer>();
        //trail.material = new Material(Shader.Find("Universal Render Pipeline/Particles/Lit"));
        FPO.transform.position = objectOrigin;
        //FPO.transform.rotation = Quaternion.Euler(objectDir);
        //FPO.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 1), new Vector3(0, 1, -0));
        //FPO.transform.rotation = Quaternion.LookRotation(GetRotationStub(objectNames[index]));

        ObjSpawnerTimer timer;
        if (!FPO.TryGetComponent<ObjSpawnerTimer>(out timer))
        {
            timer = FPO.AddComponent<ObjSpawnerTimer>();
        }
        
        timer.ttl = objectTTL;
        //Rigidbody RG = FPO.AddComponent<Rigidbody>();
        Rigidbody RG = FPO.GetComponent<Rigidbody>();
        RG.useGravity = false;

        Vector3 spread = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
        //FPO.transform.rotation *= Quaternion.Euler(objectDir + spread);
        FPO.transform.rotation = Quaternion.LookRotation(objectDir + spread);
        RG.AddForce((objectDir + spread) * objectSpeed);
        index++;
        if (index >= objectNames.Length) index = 0;
    }
}
