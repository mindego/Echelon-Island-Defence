using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class MoonTextureSetter : MonoBehaviour
{
    public string MoonTexture;
    public Transform Sun;
    // Start is called before the first frame update
    void Start()
    {
        HDAdditionalLightData lightData;
        lightData = GetComponent<HDAdditionalLightData>();
        lightData.surfaceTexture = GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, MoonTexture);
    }

    private void FixedUpdate()
    {
        if (Sun == null) return;
        Vector3 SunVector3 = Sun.transform.rotation.eulerAngles;
        Vector2 SunPos = new Vector2(SunVector3.x, SunVector3.y);
        Vector3 MoonVector3 = transform.rotation.eulerAngles;
        Vector2 MoonPos = new Vector2(transform.rotation.x, transform.rotation.y);
        Quaternion rotation = this.transform.rotation;
        MoonVector3.z = Vector2.SignedAngle(MoonPos,SunPos);

        transform.rotation = Quaternion.Euler(MoonVector3);
    }



}
