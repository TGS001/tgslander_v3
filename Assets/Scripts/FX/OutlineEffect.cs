using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Outline")]
public class OutlineEffect : UnityStandardAssets.ImageEffects.PostEffectsBase {
    public Color lineColor = Color.black;
    public float depthComponent = 1.0f;
    public float normalComponent = 1.0f;
    public float size = 1;
    public Shader shader = null;
    private Material material;

    int lineColorID;
    int depthComponentID;
    int normalComponentID;
    int sizeID;

    void Setup()
    {
        Camera cam = GetComponent<Camera>();
        cam.depthTextureMode |= DepthTextureMode.DepthNormals;
        lineColorID = Shader.PropertyToID("lineColor");
        depthComponentID = Shader.PropertyToID("depthComponent");
        normalComponentID = Shader.PropertyToID("normalComponent");
        sizeID = Shader.PropertyToID("size");
    }

    public override bool CheckResources()
    {
        CheckSupport(true);
        material = CheckShaderAndCreateMaterial(shader, material);
        return isSupported && material;
    }

    private void OnEnable()
    {
        Setup();
    }

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (CheckResources() == false)
        {
            Graphics.Blit(source, destination);
        }
        else
        {
            material.SetColor(lineColorID, lineColor);
            material.SetFloat(depthComponentID, depthComponent);
            material.SetFloat(normalComponentID, normalComponent);
            material.SetFloat(sizeID, size);
            Graphics.Blit(source, destination, material);
        }
    }
}
