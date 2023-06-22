using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempSkySetter : MonoBehaviour
{
    public string SkyboxTextureName;
    public SkyModeEnum SkyMode;
    // Start is called before the first frame update
    void Start()
    {
        SetSkybox(SkyboxTextureName);

    }

    public void SetSkybox(string textureName)
    {
        Material skyboxMaterial = GenDefaultSkybox(textureName);
        if (!TryGetComponent<Skybox>(out Skybox skyboxComponent))
        {
            skyboxComponent = gameObject.AddComponent<Skybox>();
        }

        skyboxComponent.material = skyboxMaterial;
    }
    private Material GenDefaultSkybox(string textureName)
    {
        Material skybox = default;
        switch (SkyMode)
        {
            case SkyModeEnum.Panoramic:
                skybox = new Material(Shader.Find("Skybox/Panoramic"));
                skybox.SetTexture("_MainTex", GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, textureName));
                break;
            case SkyModeEnum.Sided6:
                skybox = new Material(Shader.Find("Skybox/6 Sided"));
                skybox.SetTexture("_UpTex", GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, textureName));
                skybox.SetTexture("_FrontTex", GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, textureName));
                skybox.SetTexture("_BackTex", GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, textureName));
                skybox.SetTexture("_LeftTex", GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, textureName));
                skybox.SetTexture("_RightTex", GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, textureName));

                break;
            default:
                return default;
        }
        

        return skybox;
    }

    public enum SkyModeEnum
    {
        Panoramic,
        Cubemap,
        Sided6
    }
}
