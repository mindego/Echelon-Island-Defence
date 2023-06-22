using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Threading.Tasks;

public class PrefabBuilder : MonoBehaviour
{
    public TextAsset FPOData;
    public TextAsset MEOData;
    public string prefabName;
    public Vector3 WorldPosition;
}
[CustomEditor(typeof(PrefabBuilder))]
public class PrefabBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PrefabBuilder myScript = (PrefabBuilder)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load FPO xml"))
        {
            CreateGameobject(myScript.FPOData, myScript.prefabName);
        }
        if (GUILayout.Button("Load FPO & MEO"))
        {
            CreateGameobject(myScript.prefabName,myScript.WorldPosition);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load FPO"))
        {
            FpoBuilder fpoBuilder = new FpoBuilder();
            fpoBuilder.BuildFPO(myScript.prefabName);
        }

        if (GUILayout.Button("Load MEO"))
        {
            LoadMEO(myScript.prefabName);
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Gen test CarrierData"))
        {
            CARRIER_DATA cd = new CARRIER_DATA();
            //cd.Side = HUMANS;
            cd.Side = CampaignDefines.SideTable.HUMANS;
            //  PointForStart = "Appear" 
            //  PointForCraft = "BF_HVY"
            //  PointForINT = "IntTouch"
            //; Duza = "HumanGPlaneBigDuza"
            //  UnitData = "AShip"
            //  ; флайт - модель
            //; Weight = 300000
            cd.MaxSpeedY = 90; // макс.скорость вверх(км/ ч)        300 / 0
            cd.MaxSpeedZ = 300; // макс.скорость вперед(км/ ч)         450 / 600
            cd.MaxAccelZ = 2; // макс.ускорение вперед(м/ сек / сек)
            cd.MaxAccelY = 2;  //макс.ускорение вверх(м/ сек / сек)
                               //; угловые перемещения
            cd.ASpeedX = 10; // макс.угловая скорость  по оси тангажа(град / сек)
            cd.ASpeedY = 10;  //макс.угловая скорость  по оси рыскания(град / сек)
            cd.AAccelX = 2;  //макс.угловое ускорение по оси тангажа(град / сек / сек)
            cd.AAccelY = 2;  //макс.угловое ускорение по оси рыскания(град / сек / сек)

            cd.FullName = "Human Avia Carrier";

            cd.SetFlag(OBJECT_DATA.OC_AIRSHIP);
            StormFileUtils.SaveXML<CARRIER_DATA>("Assets/carrierdatatest.xml", cd);

        }
        if (GUILayout.Button("Get CRC"))
        {
            Storm.CRC32 crc32 = new Storm.CRC32();
            var hashsum = crc32.HashString(myScript.prefabName);
            Debug.Log("Calculated hash: " + hashsum.ToString("X8"));

            ResourcePack rp = new ResourcePack("Data/objects.dat");
            UInt32 id = rp.GetIdByName(myScript.prefabName);
            Debug.Log("Hash from datafile: " + id.ToString("X8"));

            var ms = rp.GetStreamById(hashsum);
            Debug.Log(ms.Length);
        }
    }

    private MEOData.MEO_DATA_HDR LoadMEO(string name)
    {
        Stream ms = GameDataHolder.GetResource<Stream>(PackType.MEODB, name);
        MEOData.MEO_DATA_HDR MEO = StormFileUtils.ReadStruct<MEOData.MEO_DATA_HDR>(ms);

        int pos = Marshal.SizeOf<MEOData.MEO_DATA_HDR>();
        //MEOData.MEO_DATA[] meodata = new MEOData.MEO_DATA[5];
        MEO.MeoData = new MEOData.MEO_DATA[5];
        ms.Seek(pos, SeekOrigin.Begin);
        for (int i = 0; i < MEO.MeoData.Length; i++)
        {
            MEO.MeoData[i] = StormFileUtils.ReadStruct<MEOData.MEO_DATA>(ms, (int)ms.Position);
            //Debug.Log(MEO.MeoData[i]);
        }

        //Debug.Log($"Of {MEO.OfPlanes}");
        //Debug.Log(MEO.MsnBounds[0]);

        //Debug.Log($"Stopped read at {ms.Position}");
        //Debug.Log("HDR "+Marshal.SizeOf<MEOData.MEO_DATA_HDR>());
        //Debug.Log("DATA "+Marshal.SizeOf<MEOData.MEO_DATA>());
        //Debug.Log("IMAGE " + Marshal.SizeOf<MEOData.IMAGE>());

        //Debug.Log(MEO.MeoData[0]);
        ms.Close();
        return MEO;
    }

    public GameObject CreateGameobject(string name,Vector3 WorldPosition = default)
    {
        FpoBuilder fpoBuilder = new FpoBuilder();
        GameObject GOBJ = fpoBuilder.BuildFPO(name);
        
        GOBJ.transform.position = WorldPosition;
        
        //GOBJ.transform.localRotation;
        MEOData.MEO_DATA_HDR MEO = LoadMEO(name);
        Vector3 Dir = MEO.MeoData[0].Dir;
        Vector3 Up = MEO.MeoData[0].Up;
        Transform corrector = GOBJ.transform.GetChild(0);
        Transform hull = corrector.GetChild(0);
        //hull.localRotation *= Quaternion.LookRotation(Dir, Up);
        corrector.localRotation = hull.localRotation * Quaternion.LookRotation(Vector3.forward, Vector3.up);
        corrector.localRotation = Quaternion.Inverse(corrector.localRotation);
        //hull.localRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        Vector3 hullPos = hull.localPosition;
        hullPos.y *= -1;
        hull.localPosition = hullPos;
        Debug.Log(hull.name);

        GOBJ.AddComponent<Rigidbody>();
        //AddCollider(GOBJ.transform);
        //PrefabUtility.SaveAsPrefabAsset(GOBJ, "Assets/Prefabs/" + name + ".prefab");
        return GOBJ;
    }

    private void AddCollider(Transform root)
    {
        Renderer r;
        //if (root.TryGetComponent<Renderer>(out r))
        //{
        //    Rigidbody rb = root.gameObject.AddComponent<Rigidbody>();
        //    rb.useGravity = true;
        //    BoxCollider collider = root.gameObject.AddComponent<BoxCollider>();
        //    collider.size = r.localBounds.size;
        //}

        foreach (Transform child in root)
        {
            //Debug.Log($"Adding to {child.name}");
            //Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
            //rb.useGravity = true;

            //if (child.parent.TryGetComponent<Rigidbody>(out Rigidbody lrb))
            //{
            //    FixedJoint joint = child.gameObject.AddComponent<FixedJoint>();
            //    joint.connectedBody = lrb;
            //    joint.enableCollision = false;
            //    //joint.breakForce = 50;

            //}
            if (child.TryGetComponent<Renderer>(out r))
            {
                BoxCollider collider = child.gameObject.AddComponent<BoxCollider>();
                collider.size = r.localBounds.size;
            }
            if (child.childCount > 0) AddCollider(child);
        }
    }

    
    private void CreateGameobject(TextAsset FPOdata, string name = null)
    {
        if (FPOdata == null) return;

        FpoBuilder TMPro = new FpoBuilder();
        
        
        GameObject GOBJ=TMPro.BuildFPO("STUB", FPOdata);

        //if (name != null) {
        //    //GOBJ.transform.localRotation;
        //    MEOData.MEO_DATA_HDR MEO = LoadMEO(name);
        //    Vector3 Dir = MEO.MeoData[0].Dir;
        //    Vector3 Up = MEO.MeoData[0].Up;
        //    Transform hull = GOBJ.transform.GetChild(0);
        //    hull.localRotation = hull.localRotation * Quaternion.LookRotation(Dir, Up);
        //    Debug.Log(hull.name);
        //}

    }
}
