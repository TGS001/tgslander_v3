using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SFX))]
public class FxMaterialProperty : MonoBehaviour {
    public MeshRenderer meshRenderer;
    public string property;
    public uint submat = 0;
    public SFX.ScalarField input;
    public float inputScale = 1;
    
    SFX fx;
    int propertyID;
    
	void OnEnable () {
        fx = GetComponent<SFX>();
        propertyID = Shader.PropertyToID(property);
	}

    // Update is called once per frame
    void LateUpdate () {
		if (meshRenderer && meshRenderer.materials.Length > submat) {
            meshRenderer.materials[submat].SetFloat(propertyID, fx.getScalar(input) * inputScale);
        }
	}
}
